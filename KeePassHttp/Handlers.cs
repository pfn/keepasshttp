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
using KeePassLib;

using Newtonsoft.Json;

namespace KeePassHttp {
    public sealed partial class KeePassHttpExt : Plugin {
        private void GetAllLoginsHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            var list = new PwObjectList<PwEntry>();

            var root = host.Database.RootGroup;

            var parms = MakeSearchParameters();

            parms.SearchString = "^[A-Za-z0-9:/-]+.[A-Za-z0-9:/-]+$"; // match anything looking like a domain or url

            root.SearchEntries(parms, list, false);
            foreach (var entry in list)
            {
                var name = entry.Strings.Get(PwDefs.TitleField).ReadString();
                var login = entry.Strings.Get(PwDefs.UserNameField).ReadString();
                var uuid = entry.Uuid.ToHexString();
                var e = new ResponseEntry(name, login, null, uuid);
                resp.Entries.Add(e);
            }
            resp.Success = true;
            resp.Id = r.Id;
            SetResponseVerifier(resp, aes);
            using (var enc = aes.CreateEncryptor())
            {
                foreach (var entry in resp.Entries)
                {
                    entry.Name = CryptoTransform(entry.Name, false, true, enc);
                    entry.Login = CryptoTransform(entry.Login, false, true, enc);
                    entry.Uuid = CryptoTransform(entry.Uuid, false, true, enc);
                }
            }
        }
        private IEnumerable<PwEntry> FindMatchingEntries(Request r, Aes aes)
        {
            Uri url;
            string submithost = null;
            using (var dec = aes.CreateDecryptor())
            {
                url = new Uri(CryptoTransform(r.Url, true, false, dec));
                if (r.SubmitUrl != null)
                    submithost = new Uri(CryptoTransform(r.SubmitUrl, true, false, dec)).Host;
            }
            var formhost = url.Host;
            var searchHost = url.Host;
            var parms = MakeSearchParameters();

            var list = new PwObjectList<PwEntry>();
            var root = host.Database.RootGroup;

            while (list.UCount == 0 && searchHost.IndexOf(".") != -1)
            {
                parms.SearchString = String.Format("^{0}$|{0}/", searchHost);
                root.SearchEntries(parms, list, false);
                searchHost = searchHost.Substring(searchHost.IndexOf(".") + 1);
            }
            Func<PwEntry, bool> filter = delegate(PwEntry e)
            {
                var title = e.Strings.Get(PwDefs.TitleField).ReadString();
                var c = GetEntryConfig(e);
                if (c != null)
                {
                    if (c.Allow.Contains(formhost) && (submithost == null || c.Allow.Contains(submithost)))
                        return true;
                    if (c.Deny.Contains(formhost) || (submithost != null && c.Deny.Contains(submithost)))
                        return false;
                }

                if (title.StartsWith("http://") || title.StartsWith("https://"))
                {
                    var u = new Uri(title);
                    return url.Host.Contains(u.Host);
                }
                return url.Host.Contains(title);
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
            ShowNotification("Count: " + resp.Count + " : " + resp.Verifier);
        }
        private void GetLoginsHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            String host, submithost;
            using (var dec = aes.CreateDecryptor()) {
                Uri url = new Uri(CryptoTransform(r.Url, true, false, dec));
                Uri submiturl = new Uri(CryptoTransform(r.SubmitUrl, true, false, dec));
                host = url.Host;
                submithost = submiturl.Host;
            }

            var items = FindMatchingEntries(r, aes);
            IEnumerable<PwEntry> needPrompting = null;
            if (items.ToList().Count > 0)
            {
                Func<PwEntry, bool> filter = delegate(PwEntry e)
                {
                    var c = GetEntryConfig(e);

                    if (c != null)
                    {
                        var title = e.Strings.Get(PwDefs.TitleField).ReadString();
                        return title != host && title != submithost && !c.Allow.Contains(host) && !c.Allow.Contains(submithost);
                    }

                    return true;
                };

                needPrompting = from e in items where filter(e) select e;
  
                if (needPrompting != null)
                {
                    var wait = new ManualResetEvent(false);
                    var clicked = false;
                    EventHandler onclick = delegate { clicked = true; wait.Set(); };
                    EventHandler onclose = delegate { wait.Set(); };

                    ShowNotification(String.Format(
                            "{0}: {1} is requesting access to credentials, click to allow/disallow",
                            r.Id, submithost), onclick, onclose);
                    wait.WaitOne();
                    if (clicked)
                    {
                        // TODO bring up form to allow/deny/remember
                        var win = this.host.MainWindow;
                        using (var f = new AccessControlForm())
                        {
                            win.Invoke((MethodInvoker)delegate
                            {
                                f.Icon = win.Icon;
                                f.Entries = needPrompting.ToList();
                                f.Load += delegate { f.Activate(); };
                                f.ShowDialog(win);
                                if (f.Remember && (f.Allowed || f.Denied))
                                {
                                    var serializer = NewJsonSerializer();
                                    foreach (var e in needPrompting)
                                    {
                                        var c = GetEntryConfig(e);
                                        if (c == null)
                                            c = new KeePassHttpEntryConfig();
                                        var set = f.Allowed ? c.Allow : c.Deny;
                                        set.Add(host);
                                        if (submithost != host)
                                            set.Add(submithost);
                                        var writer = new StringWriter();
                                        serializer.Serialize(writer, c);
                                        e.Strings.Set(KEEPASSHTTP_NAME, new ProtectedString(false, writer.ToString()));
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
                    var name = entry.Strings.Get(PwDefs.TitleField).ReadString();
                    var login = entry.Strings.Get(PwDefs.UserNameField).ReadString();
                    var passwd = entry.Strings.Get(PwDefs.PasswordField).ReadString();
                    var uuid = entry.Uuid.ToHexString();
                    var e = new ResponseEntry(name, login, passwd, uuid);
                    resp.Entries.Add(e);
                }

                if (items.ToList().Count > 0)
                {
                    var names = (from e in resp.Entries select e.Name).Distinct<string>();
                    var n = String.Join(", ", names.ToArray<string>());
                    ShowNotification(String.Format("{0}: {1} is receiving credentials for {2}", r.Id, host, n));
                }

                resp.Success = true;
                resp.Id = r.Id;
                SetResponseVerifier(resp, aes);

                using (var enc = aes.CreateEncryptor())
                {
                    foreach (var entry in resp.Entries)
                    {
                        entry.Name = CryptoTransform(entry.Name, false, true, enc);
                        entry.Login = CryptoTransform(entry.Login, false, true, enc);
                        entry.Uuid = CryptoTransform(entry.Uuid, false, true, enc);
                        entry.Password = CryptoTransform(entry.Password, false, true, enc);
                    }
                }
            }
        }
        private void SetLoginHandler(Request r, Response resp, Aes aes)
        {
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
                        resp.Id = f.KeyId;
                        resp.Success = true;
                        SetResponseVerifier(resp, aes);
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
                var json = e.Strings.Get(KEEPASSHTTP_NAME).ReadString();
                using (var ins = new JsonTextReader(new StringReader(json)))
                {
                    return serializer.Deserialize<KeePassHttpEntryConfig>(ins);
                }
            }
            return null;
        }
    }
}