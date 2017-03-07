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
using KeePassLib.Cryptography;

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

            Func<PwEntry, bool> hideExpired = delegate(PwEntry e)
            {
                DateTime dtNow = DateTime.UtcNow;

                if(e.Expires && (e.ExpiryTime <= dtNow))
                {
                    return false;
                }

                return true;
            };

            if (configOpt.HideExpired)
            {
                result = from e in result where hideExpired(e.entry) select e;
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

                string compareToUrl = null;
                if (r.SubmitUrl != null)
                {
                    compareToUrl = CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT);
                }
                if(String.IsNullOrEmpty(compareToUrl))
                    compareToUrl = CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT);

                compareToUrl = compareToUrl.ToLower();

                foreach (var entryDatabase in items)
                {
                    string entryUrl = String.Copy(entryDatabase.entry.Strings.ReadSafe(PwDefs.UrlField));
                    if (String.IsNullOrEmpty(entryUrl))
                        entryUrl = entryDatabase.entry.Strings.ReadSafe(PwDefs.TitleField);

                    entryUrl = entryUrl.ToLower();

                    entryDatabase.entry.UsageCount = (ulong)LevenshteinDistance(compareToUrl, entryUrl);

                }

                var itemsList = items.ToList();

                if (configOpt.SpecificMatchingOnly)
                {
                    itemsList = (from e in itemsList 
                                 orderby e.entry.UsageCount ascending 
                                 select e).ToList();

                    ulong lowestDistance = itemsList.Count > 0 ?
                        itemsList[0].entry.UsageCount :
                        0;

                    itemsList = (from e in itemsList
                                 where e.entry.UsageCount == lowestDistance
                                 orderby e.entry.UsageCount
                                 select e).ToList();
                    
                }

                if (configOpt.SortResultByUsername)
                {
                    var items2 = from e in itemsList orderby e.entry.UsageCount ascending, GetUserPass(e)[0] ascending select e;
                    itemsList = items2.ToList();
                }
                else
                {
                    var items2 = from e in itemsList orderby e.entry.UsageCount ascending, e.entry.Strings.ReadSafe(PwDefs.TitleField) ascending select e;
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
        //http://en.wikibooks.org/wiki/Algorithm_Implementation/Strings/Levenshtein_distance#C.23
        private int LevenshteinDistance(string source, string target)
        {
            if (String.IsNullOrEmpty(source))
            {
                if (String.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }
            if (String.IsNullOrEmpty(target)) return source.Length;

            if (source.Length > target.Length)
            {
                var temp = target;
                target = source;
                source = temp;
            }

            var m = target.Length;
            var n = source.Length;
            var distance = new int[2, m + 1];
            // Initialize the distance 'matrix'
            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;
            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target[j - 1] == source[i - 1] ? 0 : 1);
                    distance[currentRow, j] = Math.Min(Math.Min(
                                            distance[previousRow, j] + 1,
                                            distance[currentRow, j - 1] + 1),
                                            distance[previousRow, j - 1] + cost);
                }
            }
            return distance[currentRow, m];
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
                    var sfValue = entryDatabase.entry.Strings.ReadSafe(sf.Key);
                    if (configOpt.ReturnStringFieldsWithKphOnly)
                    {
                        if (sf.Key.StartsWith("KPH: "))
                        {
                            fields.Add(new ResponseStringField(sf.Key.Substring(5), sfValue));
                        }
                    }
                    else
                    {
                        fields.Add(new ResponseStringField(sf.Key, sfValue));
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
                uint uBits = QualityEstimation.EstimatePasswordBits(pbNew);
                ResponseEntry item = new ResponseEntry(Request.GENERATE_PASSWORD, uBits.ToString(), StrUtil.Utf8.GetString(pbNew), Request.GENERATE_PASSWORD, null);
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
