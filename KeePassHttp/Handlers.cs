using System.Security.Cryptography;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Threading;

using KeePass.Plugins;
using KeePassLib.Collections;
using KeePassLib.Security;
using KeePassLib.Utility;
using KeePassLib;

using Newtonsoft.Json;

namespace KeePassHttp {
    public sealed partial class KeePassHttpExt : Plugin {
        private string GetHost(string uri)
        {
            var host = uri;
            try
            {
                var url = new Uri(uri);
                host = url.Host;
            }
            catch
            {
                // ignore exception, not a URI, assume input is host
            }
            return host;
        }
        private void GetAllLoginsHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            var list = new PwObjectList<PwEntry>();

            var root = host.Database.RootGroup;

            var parms = MakeSearchParameters();

            parms.SearchString = @"^[A-Za-z0-9:/-]+\.[A-Za-z0-9:/-]+$"; // match anything looking like a domain or url

            root.SearchEntries(parms, list, true);
            foreach (var entry in list)
            {
                var name = entry.Strings.ReadSafe(PwDefs.TitleField);
                var login = GetUserPass(entry)[0];
                var uuid = entry.Uuid.ToHexString();
                var e = new ResponseEntry(name, login, null, uuid);
                resp.Entries.Add(e);
            }
            resp.Success = true;
            resp.Id = r.Id;
            SetResponseVerifier(resp, aes);
            foreach (var entry in resp.Entries)
            {
                entry.Name = CryptoTransform(entry.Name, false, true, aes, CMode.ENCRYPT);
                entry.Login = CryptoTransform(entry.Login, false, true, aes, CMode.ENCRYPT);
                entry.Uuid = CryptoTransform(entry.Uuid, false, true, aes, CMode.ENCRYPT);
            }
        }
        private IEnumerable<PwEntry> FindMatchingEntries(Request r, Aes aes)
        {
            string submithost = null;
            string realm = null;
            var list = new PwObjectList<PwEntry>();
            string formhost, searchHost;
            formhost = searchHost = GetHost(CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT));
            if (r.SubmitUrl != null) {
                submithost = GetHost(CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT));
            }
            if (r.Realm != null)
                realm = CryptoTransform(r.Realm, true, false, aes, CMode.DECRYPT);

            var origSearchHost = searchHost;
            var parms = MakeSearchParameters();

            var root = host.Database.RootGroup;

            while (list.UCount == 0 && (origSearchHost == searchHost || searchHost.IndexOf(".") != -1))
            {
                parms.SearchString = String.Format("^{0}$|/{0}/?", searchHost);
                root.SearchEntries(parms, list, true);
                searchHost = searchHost.Substring(searchHost.IndexOf(".") + 1);
                if (searchHost == origSearchHost)
                    break;
            }
            Func<PwEntry, bool> filter = delegate(PwEntry e)
            {
                var title = e.Strings.ReadSafe(PwDefs.TitleField);
                var entryUrl = e.Strings.ReadSafe(PwDefs.UrlField);
                var c = GetEntryConfig(e);
                if (c != null)
                {
                    if (c.Allow.Contains(formhost) && (submithost == null || c.Allow.Contains(submithost)))
                        return true;
                    if (c.Deny.Contains(formhost) || (submithost != null && c.Deny.Contains(submithost)))
                        return false;
                    if (realm != null && c.Realm != realm)
                        return false;
                }

                if (title.StartsWith("http://") || title.StartsWith("https://"))
                {
                    var u = new Uri(title);
                    if (formhost.Contains(u.Host))
                        return true;
                }
                if (entryUrl != null && entryUrl.StartsWith("http://") || entryUrl.StartsWith("https://"))
                {
                    var u = new Uri(entryUrl);
                    if (formhost.Contains(u.Host))
                        return true;
                }
                return formhost.Contains(title) || (entryUrl != null && formhost.Contains(entryUrl));
            };

            return from e in list where filter(e) select e;
        }
        private void GetLoginsCountHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            resp.Success = true;
            resp.Id = r.Id;
            var items = FindMatchingEntries(r, aes);
            SetResponseVerifier(resp, aes);
            resp.Count = items.ToList().Count;
        }
        private void GetLoginsHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            string submithost = null;
            var host = GetHost(CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT));
            if (r.SubmitUrl != null)
                submithost = GetHost(CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT));

            var items = FindMatchingEntries(r, aes);
            if (items.ToList().Count > 0)
            {
                Func<PwEntry, bool> filter = delegate(PwEntry e)
                {
                    var c = GetEntryConfig(e);

                    var title = e.Strings.ReadSafe(PwDefs.TitleField);
                    var entryUrl = e.Strings.ReadSafe(PwDefs.UrlField);
                    if (c != null)
                    {
                        return title != host && entryUrl != host && !c.Allow.Contains(host) || (submithost != null && !c.Allow.Contains(submithost) && submithost != title && submithost != entryUrl);
                    }
                    return title != host && entryUrl != host || (submithost != null && title != submithost && entryUrl != submithost);
                };

                var needPrompting = from e in items where filter(e) select e;
  
                if (needPrompting.ToList().Count > 0)
                {
                    var wait = new ManualResetEvent(false);
                    var clicked = false;
                    var delegated = false;
                    EventHandler onclick = delegate { delegated = true; clicked = true; wait.Set(); };
                    EventHandler onclose = delegate { delegated = true; wait.Set(); };

                    ShowNotification(String.Format(
                            "{0}: {1} is requesting access, click to allow or deny",
                            r.Id, submithost != null ? submithost : host), onclick, onclose);
                    wait.WaitOne(GetNotificationTime() + 5000); // give a little time to fade
                    if (!delegated)
                        resp.Error = "Notification bubble did not appear";

                    if (clicked)
                    {
                        var win = this.host.MainWindow;
                        using (var f = new AccessControlForm())
                        {
                            win.Invoke((MethodInvoker)delegate
                            {
                                f.Icon = win.Icon;
                                f.Plugin = this;
                                f.Entries = needPrompting.ToList();
                                f.Host = submithost != null ? submithost : host;
                                f.Load += delegate { f.Activate(); };
                                f.ShowDialog(win);
                                if (f.Remember && (f.Allowed || f.Denied))
                                {
                                    foreach (var e in needPrompting)
                                    {
                                        var c = GetEntryConfig(e);
                                        if (c == null)
                                            c = new KeePassHttpEntryConfig();
                                        var set = f.Allowed ? c.Allow : c.Deny;
                                        set.Add(host);
                                        if (submithost != null && submithost != host)
                                            set.Add(submithost);
                                        SetEntryConfig(e, c);

                                    }
                                }
                                if (!f.Allowed)
                                    items = items.Except(needPrompting);
                            });
                        }
                    }
                    else
                    {
                        items = items.Except(needPrompting);
                    }
                }

                foreach (var entry in items)
                {
                    var name = entry.Strings.ReadSafe(PwDefs.TitleField);
                    var loginpass = GetUserPass(entry);
                    var login = loginpass[0];
                    var passwd = loginpass[1];
                    var uuid = entry.Uuid.ToHexString();
                    var e = new ResponseEntry(name, login, passwd, uuid);
                    resp.Entries.Add(e);
                }

                if (items.ToList().Count > 0)
                {
                    var names = (from e in resp.Entries select e.Name).Distinct<string>();
                    var n = String.Join("\n    ", names.ToArray<string>());
                    ShowNotification(String.Format("{0}: {1} is receiving credentials for:\n    {2}", r.Id, host, n));
                }

                resp.Success = true;
                resp.Id = r.Id;
                SetResponseVerifier(resp, aes);

                foreach (var entry in resp.Entries)
                {
                    entry.Name = CryptoTransform(entry.Name, false, true, aes, CMode.ENCRYPT);
                    entry.Login = CryptoTransform(entry.Login, false, true, aes, CMode.ENCRYPT);
                    entry.Uuid = CryptoTransform(entry.Uuid, false, true, aes, CMode.ENCRYPT);
                    entry.Password = CryptoTransform(entry.Password, false, true, aes, CMode.ENCRYPT);
                }
            }
        }
        private void SetLoginHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;
            string submithost = null;
            PwUuid uuid = null;
            string username, password;
            string realm = null;
            var formhost = GetHost(CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT));
            if (r.SubmitUrl != null)
                submithost = GetHost(CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT));
            if (r.Realm != null)
                realm = CryptoTransform(r.Realm, true, false, aes, CMode.DECRYPT);

            username = CryptoTransform(r.Login, true, false, aes, CMode.DECRYPT);
            password = CryptoTransform(r.Password, true, false, aes, CMode.DECRYPT);
            if (r.Uuid != null)
            {
                uuid = new PwUuid(MemUtil.HexStringToByteArray(
                        CryptoTransform(r.Uuid, true, false, aes, CMode.DECRYPT)));
            }
            if (uuid != null)
            {
                // modify existing entry
                PwEntry entry = host.Database.RootGroup.FindEntry(uuid, true);
                string[] up = GetUserPass(entry);
                var u = up[0];
                var p = up[1];
                if (u != username || p != password)
                {
                    ShowNotification(String.Format(
                        "{0}:  You have an entry change prompt waiting, click to activate", r.Id),
                        (s, e) => host.MainWindow.Activate());
                    var result = MessageBox.Show(host.MainWindow,
                        String.Format("Do you want to update the information in {0} - {1}?", formhost, u),
                        "Update Entry", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, username));
                        entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, password));
                        entry.Touch(true);
                        UpdateUI(entry.ParentGroup);
                    }
                }
            }
            else
            {
                var root = host.Database.RootGroup;
                var group = root.FindCreateGroup(KEEPASSHTTP_GROUP_NAME, false);
                if (group == null)
                {
                    group = new PwGroup(true, true, KEEPASSHTTP_GROUP_NAME, PwIcon.WorldComputer);
                    root.AddGroup(group, true);
                    UpdateUI(null);
                }

                PwEntry entry = new PwEntry(true, true);
                entry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, formhost));
                entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, username));
                entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, password));
                
                if ((submithost != null && formhost != submithost) || realm != null)
                {
                    var config = new KeePassHttpEntryConfig();
                    if (submithost != null)
                        config.Allow.Add(submithost);
                    if (realm != null)
                        config.Realm = realm;

                    var serializer = NewJsonSerializer();
                    var writer = new StringWriter();
                    serializer.Serialize(writer, config);
                    entry.Strings.Set(KEEPASSHTTP_NAME, new ProtectedString(false, writer.ToString()));
                }
                group.AddEntry(entry, true);
                UpdateUI(group);
            }
            resp.Success = true;
            resp.Id = r.Id;
            SetResponseVerifier(resp, aes);
        }
        private void AssociateHandler(Request r, Response resp, Aes aes)
        {
            if (!TestRequestVerifier(r, aes, r.Key))
                return;

            // key is good, prompt user to save
            using (var f = new ConfirmAssociationForm())
            {
                var win = host.MainWindow;
                win.Invoke((MethodInvoker)delegate
                {
                    ShowNotification("New key association requested", (s, e) => f.Activate());
                    f.Icon = win.Icon;
                    f.Key = r.Key;
                    f.Load += delegate { f.Activate(); };
                    f.ShowDialog(win);
                    if (f.KeyId != null)
                    {
                        var entry = GetConfigEntry(true);
                        entry.Strings.Set(ASSOCIATE_KEY_PREFIX + f.KeyId, new ProtectedString(true, r.Key));
                        entry.Touch(true);
                        resp.Id = f.KeyId;
                        resp.Success = true;
                        SetResponseVerifier(resp, aes);
                        UpdateUI(null);
                    }
                });
            }
        }

        private void TestAssociateHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            resp.Success = true;
            resp.Id = r.Id;
            SetResponseVerifier(resp, aes);
        }

        private KeePassHttpEntryConfig GetEntryConfig(PwEntry e)
        {
            var serializer = NewJsonSerializer();
            if (e.Strings.Exists(KEEPASSHTTP_NAME))
            {
                var json = e.Strings.ReadSafe(KEEPASSHTTP_NAME);
                using (var ins = new JsonTextReader(new StringReader(json)))
                {
                    return serializer.Deserialize<KeePassHttpEntryConfig>(ins);
                }
            }
            return null;
        }
        private void SetEntryConfig(PwEntry e, KeePassHttpEntryConfig c)
        {
            var serializer = NewJsonSerializer();
            var writer = new StringWriter();
            serializer.Serialize(writer, c);
            e.Strings.Set(KEEPASSHTTP_NAME, new ProtectedString(false, writer.ToString()));
            e.Touch(true);
            UpdateUI(e.ParentGroup);
        }
    }
}
