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

		private bool isBalloonTipsEnabled()
		{
			int enabledBalloonTipsMachine = (int)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
					"EnableBalloonTips",
					1);
			int enabledBalloonTipsUser = (int)Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced",
				 "EnableBalloonTips",
				 1);
			return (enabledBalloonTipsMachine == 1 && enabledBalloonTipsUser == 1);
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

			PwObjectList<PwGroup> listDatabases = new PwObjectList<PwGroup>();

			var configOpt = new ConfigOpt(this.host.CustomConfig);
			if (configOpt.SearchInAllOpenedDatabases)
			{
				foreach (PwDocument doc in host.MainWindow.DocumentManager.Documents)
				{
					if (doc.Database.IsOpen)
					{
						listDatabases.Add(doc.Database.RootGroup);
					}
				}
			}
			else
			{
				listDatabases.Add(host.Database.RootGroup);
			}

			uint listCount = 0;
			foreach (PwGroup group in listDatabases)
			{
				searchHost = origSearchHost;
				//get all possible entries for given host-name
				while (list.UCount == listCount && (origSearchHost == searchHost || searchHost.IndexOf(".") != -1))
				{
					parms.SearchString = String.Format("^{0}$|/{0}/?", searchHost);
					group.SearchEntries(parms, list);
					searchHost = searchHost.Substring(searchHost.IndexOf(".") + 1);
					//searchHost contains no dot --> prevent possible infinite loop
					if (searchHost == origSearchHost)
						break;
				}
				listCount = list.UCount;
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

				var configOpt = new ConfigOpt(this.host.CustomConfig);
				var config = GetConfigEntry(true);
				var autoAllowS = config.Strings.ReadSafe("Auto Allow");
				var autoAllow = autoAllowS != null && autoAllowS.Trim() != "";
				autoAllow = autoAllow || configOpt.AlwaysAllowAccess;
				var needPrompting = from e in items where filter(e) select e;

				if (needPrompting.ToList().Count > 0 && !autoAllow)
				{
					var clicked = true;

					if (isBalloonTipsEnabled())
					{
						clicked = false;
						var wait = new ManualResetEvent(false);
						var delegated = false;
						EventHandler onclick = delegate { delegated = true; clicked = true; wait.Set(); };
						EventHandler onclose = delegate { delegated = true; wait.Set(); };

						ShowNotification(String.Format(
								"{0}: {1} is requesting access, click to allow or deny",
								r.Id, submithost != null ? submithost : host), onclick, onclose);
						wait.WaitOne(GetNotificationTime() + 5000); // give a little time to fade
						if (!delegated)
							resp.Error = "Notification bubble did not appear";
					}

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

				if (r.SortSelection == "true" || configOpt.SpecificMatchingOnly)
				{
					string sortHost = CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT);
					if (sortHost.EndsWith("/"))
						sortHost = sortHost.Substring(0, sortHost.Length - 1);

					string sortSubmiturl = CryptoTransform(r.SubmitUrl, true, false, aes, CMode.DECRYPT);
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
						sortBaseSubmiturl = String.Format("{0}{1}{2}{3}", sortBaseSubmithostURI.Scheme,
							Uri.SchemeDelimiter, sortBaseSubmithostURI.Authority, sortBaseSubmithostURI.AbsolutePath.Substring(0, sortBaseSubmithostURI.AbsolutePath.LastIndexOf("/")));
					}

					sortSubmiturl = sortSubmiturl.ToLower();
					sortHost = sortHost.ToLower();
					sortBaseSubmiturl = sortBaseSubmiturl.ToLower();

					foreach (var e in items)
					{
						string entryUrl = String.Copy(e.Strings.ReadSafe(PwDefs.UrlField));
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
							e.UsageCount = 90;
						else if (sortSubmiturl.StartsWith(entryUrl) && sortHost != entryUrl && sortBaseSubmiturl != entryUrl)
							e.UsageCount = 80;
						else if (sortSubmiturl.StartsWith(baseEntryUrl) && sortHost != baseEntryUrl && sortBaseSubmiturl != baseEntryUrl)
							e.UsageCount = 70;
						else if (sortHost == entryUrl)
							e.UsageCount = 50;
						else if (sortBaseSubmiturl == entryUrl)
							e.UsageCount = 40;
						else if (entryUrl.StartsWith(sortSubmiturl))
							e.UsageCount = 30;
						else if (entryUrl.StartsWith(sortBaseSubmiturl) && sortBaseSubmiturl != sortHost)
							e.UsageCount = 25;
						else if (sortSubmiturl.StartsWith(entryUrl))
							e.UsageCount = 20;
						else if (sortSubmiturl.StartsWith(baseEntryUrl))
							e.UsageCount = 15;
						else if (entryUrl.StartsWith(sortHost))
							e.UsageCount = 10;
						else if (sortHost.StartsWith(entryUrl))
							e.UsageCount = 5;
						else
							e.UsageCount = 1;
					}

					var items2 = from e in items orderby e.UsageCount descending select e;
					items = items2;
				}

				if (configOpt.SpecificMatchingOnly)
				{
					ulong highestCount = 0;
					foreach (var entry in items)
					{
						if (highestCount == 0)
						{
							highestCount = entry.UsageCount;
						}

						if (entry.UsageCount == highestCount)
						{
							var name = entry.Strings.ReadSafe(PwDefs.TitleField);
							var loginpass = GetUserPass(entry);
							var login = loginpass[0];
							var passwd = loginpass[1];
							var uuid = entry.Uuid.ToHexString();
							var e = new ResponseEntry(name, login, passwd, uuid);
							resp.Entries.Add(e);
						}
					}
				}
				else
				{
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
				}

				if (items.ToList().Count > 0)
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

        private void SetLoginHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;
            string submithost = null;
            PwUuid uuid = null;
            string username, password, url;
            string realm = null;
			url = CryptoTransform(r.Url, true, false, aes, CMode.DECRYPT);
            var formhost = GetHost(url);
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
				var configOpt = new ConfigOpt(this.host.CustomConfig);

                if (u != username || p != password)
				{
					bool allowUpdate = configOpt.AlwaysAllowUpdates;

					if (!allowUpdate)
					{
						if (isBalloonTipsEnabled())
						{
							ShowNotification(String.Format(
								"{0}:  You have an entry change prompt waiting, click to activate", r.Id),
								(s, e) => host.MainWindow.Activate());
						}

						DialogResult result;
						if (host.MainWindow.IsTrayed())
						{
							result = MessageBox.Show(
								String.Format("Do you want to update the information in {0} - {1}?", formhost, u),
								"Update Entry", MessageBoxButtons.YesNo,
								MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
						}
						else
						{
							result = MessageBox.Show(
								host.MainWindow,
								String.Format("Do you want to update the information in {0} - {1}?", formhost, u),
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
                    }
                }
            }
            else
            {
				// creating new entry
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
				entry.Strings.Set(PwDefs.UrlField, new ProtectedString(true, url));
                
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
