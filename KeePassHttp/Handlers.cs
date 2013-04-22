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
using Microsoft.Win32;
using KeePass.UI;
using KeePass;
using KeePassLib.Cryptography.PasswordGenerator;

namespace KeePassHttp {
    public sealed partial class KeePassHttpExt : Plugin
    {
        private string GetHost(string uri)
        {
            var host = uri;
            try
            {
                var url = new Uri(uri);
                host = url.Host;

                if (!url.IsDefaultPort)
                {
                    host += ":" + url.Port.ToString();
                }
            }
            catch
            {
                // ignore exception, not a URI, assume input is host
            }
            return host;
        }

        private string GetScheme(string uri)
        {
            var scheme = "";
            try
            {
                var url = new Uri(uri);
                scheme = url.Scheme;
            }
            catch
            {
                // ignore exception, not a URI, assume input is host
            }
            return scheme;
        }

        private bool canShowBalloonTips()
        {
            // tray icon is not visible --> no balloon tips for it
            if (Program.Config.UI.TrayIcon.ShowOnlyIfTrayed && !host.MainWindow.IsTrayed())
            {
                return false;
            }

            // only use balloon tips on windows machines
            if (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == System.PlatformID.Win32S || Environment.OSVersion.Platform == System.PlatformID.Win32Windows)
            {
                int enabledBalloonTipsMachine = (int)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                        "EnableBalloonTips",
                        1);
                int enabledBalloonTipsUser = (int)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
                     "EnableBalloonTips",
                     1);
                return (enabledBalloonTipsMachine == 1 && enabledBalloonTipsUser == 1);
            }

            return false;
        }

        private void GetAllLoginsHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            var list = new PwObjectList<PwEntry>();

            var root = host.Database.RootGroup;

            var parms = MakeSearchParameters();

            parms.SearchString = @"^[A-Za-z0-9:/-]+\.[A-Za-z0-9:/-]+$"; // match anything looking like a domain or url

            root.SearchEntries(parms, list);
            foreach (var entry in list)
            {
                var name = entry.Strings.ReadSafe(PwDefs.TitleField);
                var login = GetUserPass(entry)[0];
                var uuid = entry.Uuid.ToHexString();
                var e = new ResponseEntry(name, login, null, uuid, null);
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

        private IEnumerable<PwEntryDatabase> FindMatchingEntries(Request r, Aes aes)
        {
            string submitHost = null;
            string realm = null;
            var listResult = new List<PwEntryDatabase>();
            var url = CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT);
            string formHost, searchHost;
            formHost = searchHost = GetHost(url);
            string hostScheme = GetScheme(url);
            if (r.SubmitUrl != null) {
                submitHost = GetHost(CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT));
            }
            if (r.Realm != null)
                realm = CryptoTransform(r.Realm, true, false, aes, CMode.DECRYPT);

            var origSearchHost = searchHost;
            var parms = MakeSearchParameters();

            List<PwDatabase> listDatabases = new List<PwDatabase>();

            var configOpt = new ConfigOpt(this.host.CustomConfig);
            if (configOpt.SearchInAllOpenedDatabases)
            {
                foreach (PwDocument doc in host.MainWindow.DocumentManager.Documents)
                {
                    if (doc.Database.IsOpen)
                    {
                        listDatabases.Add(doc.Database);
                    }
                }
            }
            else
            {
                listDatabases.Add(host.Database);
            }

            int listCount = 0;
            foreach (PwDatabase db in listDatabases)
            {
                searchHost = origSearchHost;
                //get all possible entries for given host-name
                while (listResult.Count == listCount && (origSearchHost == searchHost || searchHost.IndexOf(".") != -1))
                {
                    parms.SearchString = String.Format("^{0}$|/{0}/?", searchHost);
                    var listEntries = new PwObjectList<PwEntry>();
                    db.RootGroup.SearchEntries(parms, listEntries);
                    foreach (var le in listEntries)
                    {
                        listResult.Add(new PwEntryDatabase(le, db));
                    }
                    searchHost = searchHost.Substring(searchHost.IndexOf(".") + 1);
                    
                    //searchHost contains no dot --> prevent possible infinite loop
                    if (searchHost == origSearchHost)
                        break;
                }
                listCount = listResult.Count;
            }
            

            Func<PwEntry, bool> filter = delegate(PwEntry e)
            {
                var title = e.Strings.ReadSafe(PwDefs.TitleField);
                var entryUrl = e.Strings.ReadSafe(PwDefs.UrlField);
                var c = GetEntryConfig(e);
                if (c != null)
                {
                    if (c.Allow.Contains(formHost) && (submitHost == null || c.Allow.Contains(submitHost)))
                        return true;
                    if (c.Deny.Contains(formHost) || (submitHost != null && c.Deny.Contains(submitHost)))
                        return false;
                    if (realm != null && c.Realm != realm)
                        return false;
                }

                if (entryUrl != null && (entryUrl.StartsWith("http://") || entryUrl.StartsWith("https://") || title.StartsWith("ftp://") || title.StartsWith("sftp://")))
                {
                    var uHost = GetHost(entryUrl);
                    if (formHost.EndsWith(uHost))
                        return true;
                }

                if (title.StartsWith("http://") || title.StartsWith("https://") || title.StartsWith("ftp://") || title.StartsWith("sftp://"))
                {
                    var uHost = GetHost(title);
                    if (formHost.EndsWith(uHost))
                        return true;
                }
                return formHost.Contains(title) || (entryUrl != null && formHost.Contains(entryUrl));
            };

            Func<PwEntry, bool> filterSchemes = delegate(PwEntry e)
            {
                var title = e.Strings.ReadSafe(PwDefs.TitleField);
                var entryUrl = e.Strings.ReadSafe(PwDefs.UrlField);

                if (entryUrl != null)
                {
                    var entryScheme = GetScheme(entryUrl);
                    if (entryScheme == hostScheme)
                    {
                        return true;
                    }
                }

                var titleScheme = GetScheme(title);
                if (titleScheme == hostScheme)
                {
                    return true;
                }

                return false;
            };

            var result = from e in listResult where filter(e.entry) select e;

            if (configOpt.MatchSchemes)
            {
                result = from e in result where filterSchemes(e.entry) select e;
            }

            return result;
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

                var configOpt = new ConfigOpt(this.host.CustomConfig);
                var config = GetConfigEntry(true);
                var autoAllowS = config.Strings.ReadSafe("Auto Allow");
                var autoAllow = autoAllowS != null && autoAllowS.Trim() != "";
                autoAllow = autoAllow || configOpt.AlwaysAllowAccess;
                var needPrompting = from e in items where filter(e.entry) select e;

                if (needPrompting.ToList().Count > 0 && !autoAllow)
                {
                    var win = this.host.MainWindow;

                    using (var f = new AccessControlForm())
                    {
                        win.Invoke((MethodInvoker)delegate
                        {
                            f.Icon = win.Icon;
                            f.Plugin = this;
                            f.Entries = (from e in items where filter(e.entry) select e.entry).ToList();
                            //f.Entries = needPrompting.ToList();
                            f.Host = submithost != null ? submithost : host;
                            f.Load += delegate { f.Activate(); };
                            f.ShowDialog(win);
                            if (f.Remember && (f.Allowed || f.Denied))
                            {
                                foreach (var e in needPrompting)
                                {
                                    var c = GetEntryConfig(e.entry);
                                    if (c == null)
                                        c = new KeePassHttpEntryConfig();
                                    var set = f.Allowed ? c.Allow : c.Deny;
                                    set.Add(host);
                                    if (submithost != null && submithost != host)
                                        set.Add(submithost);
                                    SetEntryConfig(e.entry, c);

                                }
                            }
                            if (!f.Allowed)
                            {
                                items = items.Except(needPrompting);
                            }
                        });
                    }
                }

                //if (r.SortSelection == "true" || configOpt.SpecificMatchingOnly)
                //{
                    string sortHost = CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT);
                    if (sortHost.EndsWith("/"))
                        sortHost = sortHost.Substring(0, sortHost.Length - 1);

                    string sortSubmiturl = null;
                    if(r.SubmitUrl != null)
                        sortSubmiturl = CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT);
                    if (sortSubmiturl == null)
                        sortSubmiturl = String.Copy(sortHost);
                    if (sortSubmiturl.EndsWith("/"))
                        sortSubmiturl = sortSubmiturl.Substring(0, sortSubmiturl.Length - 1);

                    if (!sortSubmiturl.Contains("://"))
                        sortSubmiturl = "http://" + sortSubmiturl;
                    if (!sortHost.Contains("://"))
                        sortHost = "http://" + sortHost;

                    string sortBaseSubmiturl = String.Copy(sortSubmiturl);
                    if (sortSubmiturl.LastIndexOf("/") > 7)
                    {
                        Uri sortBaseSubmithostURI = new Uri(sortSubmiturl);
                        sortBaseSubmiturl = String.Format("{0}{1}{2}{3}",
                            sortBaseSubmithostURI.Scheme,
                            Uri.SchemeDelimiter,
                            sortBaseSubmithostURI.Authority,
                            sortBaseSubmithostURI.AbsolutePath.Substring(0, sortBaseSubmithostURI.AbsolutePath.LastIndexOf("/"))
                        );
                    }

                    sortSubmiturl = sortSubmiturl.ToLower();
                    sortHost = sortHost.ToLower();
                    sortBaseSubmiturl = sortBaseSubmiturl.ToLower();

                    foreach (var entryDatabase in items)
                    {
                        string entryUrl = String.Copy(entryDatabase.entry.Strings.ReadSafe(PwDefs.UrlField));
                        if (entryUrl.EndsWith("/"))
                            entryUrl = entryUrl.Substring(0, entryUrl.Length - 1);
                        entryUrl = entryUrl.ToLower();
                        if (!entryUrl.Contains("://"))
                            entryUrl = "http://" + entryUrl;

                        string baseEntryUrl = String.Copy(entryUrl);
                        if (baseEntryUrl.LastIndexOf("/") > 7)
                        {
                            Uri baseEntryUrlURI = new Uri(entryUrl);
                            baseEntryUrl = String.Format("{0}{1}{2}{3}", baseEntryUrlURI.Scheme,
                                Uri.SchemeDelimiter, baseEntryUrlURI.Authority, baseEntryUrlURI.AbsolutePath.Substring(0, baseEntryUrlURI.AbsolutePath.LastIndexOf("/")));
                        }

                        if (sortSubmiturl == entryUrl)
                            entryDatabase.entry.UsageCount = 90;
                        else if (sortSubmiturl.StartsWith(entryUrl) && sortHost != entryUrl && sortBaseSubmiturl != entryUrl)
                            entryDatabase.entry.UsageCount = 80;
                        else if (sortSubmiturl.StartsWith(baseEntryUrl) && sortHost != baseEntryUrl && sortBaseSubmiturl != baseEntryUrl)
                            entryDatabase.entry.UsageCount = 70;
                        else if (sortHost == entryUrl)
                            entryDatabase.entry.UsageCount = 50;
                        else if (sortBaseSubmiturl == entryUrl)
                            entryDatabase.entry.UsageCount = 40;
                        else if (entryUrl.StartsWith(sortSubmiturl))
                            entryDatabase.entry.UsageCount = 30;
                        else if (entryUrl.StartsWith(sortBaseSubmiturl) && sortBaseSubmiturl != sortHost)
                            entryDatabase.entry.UsageCount = 25;
                        else if (sortSubmiturl.StartsWith(entryUrl))
                            entryDatabase.entry.UsageCount = 20;
                        else if (sortSubmiturl.StartsWith(baseEntryUrl))
                            entryDatabase.entry.UsageCount = 15;
                        else if (entryUrl.StartsWith(sortHost))
                            entryDatabase.entry.UsageCount = 10;
                        else if (sortHost.StartsWith(entryUrl))
                            entryDatabase.entry.UsageCount = 5;
                        else
                            entryDatabase.entry.UsageCount = 1;
                    }
                //}

                var itemsList = items.ToList();

                if (configOpt.SpecificMatchingOnly)
                {
                    ulong highestCount = 0;
                    foreach (var entryDatabase in itemsList.ToList())
                    {
                        if (highestCount == 0)
                        {
                            highestCount = entryDatabase.entry.UsageCount;
                        }

                        if (entryDatabase.entry.UsageCount != highestCount)
                        {
                            itemsList.Remove(entryDatabase);
                        }
                    }
                }

                if (configOpt.SortResultByUsername)
                {
                    var items2 = from e in itemsList orderby e.entry.UsageCount descending, GetUserPass(e)[0] ascending select e;
                    itemsList = items2.ToList();
                }
                else
                {
                    var items2 = from e in itemsList orderby e.entry.UsageCount descending, e.entry.Strings.ReadSafe(PwDefs.TitleField) ascending select e;
                    itemsList = items2.ToList();
                }

                foreach (var entryDatabase in itemsList)
                {
                    var e = PrepareElementForResponseEntries(configOpt, entryDatabase);
                    resp.Entries.Add(e);
                }

                if (itemsList.Count > 0)
                {
                    var names = (from e in resp.Entries select e.Name).Distinct<string>();
                    var n = String.Join("\n    ", names.ToArray<string>());

                    if (configOpt.ReceiveCredentialNotification)
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

                    if (entry.StringFields != null)
                    {
                        foreach (var sf in entry.StringFields)
                        {
                            sf.Key = CryptoTransform(sf.Key, false, true, aes, CMode.ENCRYPT);
                            sf.Value = CryptoTransform(sf.Value, false, true, aes, CMode.ENCRYPT);
                        }
                    }
                }

                resp.Count = resp.Entries.Count;
            }
            else
            {
                resp.Success = true;
                resp.Id = r.Id;
                SetResponseVerifier(resp, aes);
            }
        }

        private ResponseEntry PrepareElementForResponseEntries(ConfigOpt configOpt, PwEntryDatabase entryDatabase)
        {
            var name = entryDatabase.entry.Strings.ReadSafe(PwDefs.TitleField);
            var loginpass = GetUserPass(entryDatabase);
            var login = loginpass[0];
            var passwd = loginpass[1];
            var uuid = entryDatabase.entry.Uuid.ToHexString();

            List<ResponseStringField> fields = null;
            if (configOpt.ReturnStringFields)
            {
                fields = new List<ResponseStringField>();
                foreach (var sf in entryDatabase.entry.Strings)
                {
                    if (sf.Key.StartsWith("KPH: "))
                    {
                        var sfValue = entryDatabase.entry.Strings.ReadSafe(sf.Key);
                        fields.Add(new ResponseStringField(sf.Key.Substring(5), sfValue));
                    }
                }

                if (fields.Count > 0)
                {
                    var fields2 = from e2 in fields orderby e2.Key ascending select e2;
                    fields = fields2.ToList<ResponseStringField>();
                }
                else
                {
                    fields = null;
                }
            }

            return new ResponseEntry(name, login, passwd, uuid, fields);
        }

        private void SetLoginHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            string url = CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT);
            var urlHost = GetHost(url);

            PwUuid uuid = null;
            string username, password;

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
                UpdateEntry(uuid, username, password, urlHost, r.Id);
            }
            else
            {
                // create new entry
                CreateEntry(username, password, urlHost, url, r, aes);
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
                    f.Activate();
                    f.Icon = win.Icon;
                    f.Key = r.Key;
                    f.Load += delegate { f.Activate(); };
                    f.ShowDialog(win);

                    if (f.KeyId != null)
                    {
                        var entry = GetConfigEntry(true);

                        bool keyNameExists = true;
                        while (keyNameExists)
                        {
                            DialogResult keyExistsResult = DialogResult.Yes;
                            foreach (var s in entry.Strings)
                            {
                                if (s.Key == ASSOCIATE_KEY_PREFIX + f.KeyId)
                                {
                                    keyExistsResult = MessageBox.Show(
                                        win,
                                        "A shared encryption-key with the name \"" + f.KeyId + "\" already exists.\nDo you want to overwrite it?",
                                        "Overwrite existing key?",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button1
                                    );
                                    break;
                                }
                            }

                            if (keyExistsResult == DialogResult.No)
                            {
                                f.ShowDialog(win);
                            }
                            else
                            {
                                keyNameExists = false;
                            }
                        }

                        if (f.KeyId != null)
                        {
                            entry.Strings.Set(ASSOCIATE_KEY_PREFIX + f.KeyId, new ProtectedString(true, r.Key));
                            entry.Touch(true);
                            resp.Id = f.KeyId;
                            resp.Success = true;
                            SetResponseVerifier(resp, aes);
                            UpdateUI(null);
                        }
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

        private void GeneratePassword(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            byte[] pbEntropy = null;
            ProtectedString psNew;
            PwProfile autoProfile = Program.Config.PasswordGenerator.AutoGeneratedPasswordsProfile;
            PwGenerator.Generate(out psNew, autoProfile, pbEntropy, Program.PwGeneratorPool);

            byte[] pbNew = psNew.ReadUtf8();
            if (pbNew != null)
            {
                ResponseEntry item = new ResponseEntry(Request.GENERATE_PASSWORD, Request.GENERATE_PASSWORD, StrUtil.Utf8.GetString(pbNew), Request.GENERATE_PASSWORD, null);
                resp.Entries.Add(item);
                resp.Success = true;
                resp.Count = 1;
                MemUtil.ZeroByteArray(pbNew);
            }

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

        private bool UpdateEntry(PwUuid uuid, string username, string password, string formHost, string requestId)
        {
            PwEntry entry = null;

            var configOpt = new ConfigOpt(this.host.CustomConfig);
            if (configOpt.SearchInAllOpenedDatabases)
            {
                foreach (PwDocument doc in host.MainWindow.DocumentManager.Documents)
                {
                    if (doc.Database.IsOpen)
                    {
                        entry = doc.Database.RootGroup.FindEntry(uuid, true);
                        if (entry != null)
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                entry = host.Database.RootGroup.FindEntry(uuid, true);
            }

            if (entry == null)
            {
                return false;
            }

            string[] up = GetUserPass(entry);
            var u = up[0];
            var p = up[1];

            if (u != username || p != password)
            {
                bool allowUpdate = configOpt.AlwaysAllowUpdates;

                if (!allowUpdate)
                {
                    host.MainWindow.Activate();

                    DialogResult result;
                    if (host.MainWindow.IsTrayed())
                    {
                        result = MessageBox.Show(
                            String.Format("Do you want to update the information in {0} - {1}?", formHost, u),
                            "Update Entry", MessageBoxButtons.YesNo,
                            MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    }
                    else
                    {
                        result = MessageBox.Show(
                            host.MainWindow,
                            String.Format("Do you want to update the information in {0} - {1}?", formHost, u),
                            "Update Entry", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }


                    if (result == DialogResult.Yes)
                    {
                        allowUpdate = true;
                    }
                }

                if (allowUpdate)
                {
                    PwObjectList<PwEntry> m_vHistory = entry.History.CloneDeep();
                    entry.History = m_vHistory;
                    entry.CreateBackup(null);

                    entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, username));
                    entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, password));
                    entry.Touch(true, false);
                    UpdateUI(entry.ParentGroup);

                    return true;
                }
            }

            return false;
        }

        private bool CreateEntry(string username, string password, string urlHost, string url, Request r, Aes aes)
        {
            string realm = null;
            if (r.Realm != null)
                realm = CryptoTransform(r.Realm, true, false, aes, CMode.DECRYPT);

            var root = host.Database.RootGroup;
            var group = root.FindCreateGroup(KEEPASSHTTP_GROUP_NAME, false);
            if (group == null)
            {
                group = new PwGroup(true, true, KEEPASSHTTP_GROUP_NAME, PwIcon.WorldComputer);
                root.AddGroup(group, true);
                UpdateUI(null);
            }

            string submithost = null;
            if (r.SubmitUrl != null)
                submithost = GetHost(CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT));

            string baseUrl = url;
            // index bigger than https:// <-- this slash
            if (baseUrl.LastIndexOf("/") > 9)
            {
                baseUrl = baseUrl.Substring(0, baseUrl.LastIndexOf("/") + 1);
            }

            PwEntry entry = new PwEntry(true, true);
            entry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, urlHost));
            entry.Strings.Set(PwDefs.UserNameField, new ProtectedString(false, username));
            entry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, password));
            entry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, baseUrl));

            if ((submithost != null && urlHost != submithost) || realm != null)
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

            return true;
        }
    }
}
