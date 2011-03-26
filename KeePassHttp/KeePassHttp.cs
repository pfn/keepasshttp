using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;

using KeePass.Plugins;

using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;

using Newtonsoft.Json;
using KeePass.Util.Spr;

namespace KeePassHttp
{
    internal delegate void RequestHandler(Request request, Response response, Aes aes);

    public enum CMode { ENCRYPT, DECRYPT }
    public sealed partial class KeePassHttpExt : Plugin
    {

        /// <summary>
        /// an arbitrarily generated uuid for the keepasshttp root entry
        /// </summary>
        private readonly byte[] KEEPASSHTTP_UUID = {
                0x34, 0x69, 0x7a, 0x40, 0x8a, 0x5b, 0x41, 0xc0,
                0x9f, 0x36, 0x89, 0x7d, 0x62, 0x3e, 0xcb, 0x31
                                                };

        private const int DEFAULT_NOTIFICATION_TIME = 5000;
        private const string KEEPASSHTTP_NAME = "KeePassHttp Settings";
        private const string KEEPASSHTTP_GROUP_NAME = "KeePassHttp Passwords";
        private const string ASSOCIATE_KEY_PREFIX = "AES Key: ";
        private IPluginHost host;
        private HttpListener listener;
        private const int DEFAULT_PORT = 19455;
        /// <summary>
        /// TODO make configurable
        /// </summary>
        private int port = DEFAULT_PORT;
        private const string HTTP_PREFIX = "http://localhost:";
        private Thread httpThread;
        private volatile bool stopped = false;
        Dictionary<string,RequestHandler> handlers = new Dictionary<string,RequestHandler>();

        private SearchParameters MakeSearchParameters()
        {
            var p = new SearchParameters();
            p.SearchInTitles = true;
            p.RegularExpression = true;
            p.SearchInGroupNames = false;
            p.SearchInNotes = false;
            p.SearchInOther = false;
            p.SearchInPasswords = false;
            p.SearchInTags = false;
            p.SearchInUrls = true;
            p.SearchInUserNames = false;
            p.SearchInUuids = false;
            return p;
        }

        private string CryptoTransform(string input, bool base64in, bool base64out, Aes cipher, CMode mode)
        {
            byte[] bytes;
            if (base64in)
                bytes = decode64(input);
            else
                bytes = Encoding.UTF8.GetBytes(input);


            using (var c = mode == CMode.ENCRYPT ? cipher.CreateEncryptor() : cipher.CreateDecryptor()) {
            var buf = c.TransformFinalBlock(bytes, 0, bytes.Length);
            return base64out ? encode64(buf) : Encoding.UTF8.GetString(buf);
            }
        }
        private PwEntry GetConfigEntry(bool create)
        {
            var root = host.Database.RootGroup;
            var uuid = new PwUuid(KEEPASSHTTP_UUID);
            var entry = root.FindEntry(uuid, false);
            if (entry == null && create)
            {
                entry = new PwEntry(false, true);
                entry.Uuid = uuid;
                entry.Strings.Set(PwDefs.TitleField, new ProtectedString(false, KEEPASSHTTP_NAME));
                root.AddEntry(entry, true);
                UpdateUI(null);
            }
            return entry;
        }
        private int GetNotificationTime()
        {
            var time = DEFAULT_NOTIFICATION_TIME;
            var entry = GetConfigEntry(false);
            if (entry != null)
            {
                var s = entry.Strings.ReadSafe("Prompt Timeout");
                if (s != null && s.Trim() != "")
                {
                    try
                    {
                        time = Int32.Parse(s) * 1000;
                    }
                    catch { }
                }
            }

            return time;
        }
        private void ShowNotification(string text)
        {
            ShowNotification(text, null, null);
        }
        private void ShowNotification(string text, EventHandler onclick)
        {
            ShowNotification(text, onclick, null);
        }
        private void ShowNotification(string text, EventHandler onclick, EventHandler onclose)
        {
            MethodInvoker m = delegate
            {
                var notify = host.MainWindow.MainNotifyIcon;

                EventHandler clicked = null;
                EventHandler closed = null;

                clicked = delegate
                {
                    notify.BalloonTipClicked -= clicked;
                    notify.BalloonTipClosed -= closed;
                    if (onclick != null)
                        onclick(notify, null);
                };
                closed = delegate
                {
                    notify.BalloonTipClicked -= clicked;
                    notify.BalloonTipClosed -= closed;
                    if (onclose != null)
                        onclose(notify, null);
                };

                //notify.BalloonTipIcon = ToolTipIcon.Info;
                notify.BalloonTipTitle = "KeePassHttp";
                notify.BalloonTipText = text;
                notify.ShowBalloonTip(GetNotificationTime());
                // need to add listeners after showing, or closed is sent right away
                notify.BalloonTipClosed += closed;
                notify.BalloonTipClicked += clicked;
            };
            if (host.MainWindow.InvokeRequired)
                host.MainWindow.Invoke(m);
            else
                m.Invoke();
        }

        public override bool Initialize(IPluginHost host)
        {
            var httpSupported = HttpListener.IsSupported;
            this.host = host;
            if (httpSupported)
            {
                try {
                    handlers.Add(Request.TEST_ASSOCIATE, TestAssociateHandler);
                    handlers.Add(Request.ASSOCIATE, AssociateHandler);
                    handlers.Add(Request.GET_LOGINS, GetLoginsHandler);
                    handlers.Add(Request.GET_LOGINS_COUNT, GetLoginsCountHandler);
                    handlers.Add(Request.GET_ALL_LOGINS, GetAllLoginsHandler);
                    handlers.Add(Request.SET_LOGIN, SetLoginHandler);

                    listener = new HttpListener();
                    listener.Prefixes.Add(HTTP_PREFIX + port + "/");
                    listener.Start();

                    httpThread = new Thread(new ThreadStart(Run));
                    httpThread.Start();
                } catch (HttpListenerException e) {
                    MessageBox.Show(host.MainWindow, "Unable to start HttpListener: " + e);
                }
            }
            else
            {
                MessageBox.Show(host.MainWindow, "The .NET HttpListener is not supported on your OS");
            }
            return httpSupported;
        }

        private void Run()
        {
            while (!stopped)
            {
                try
                {
                    var r = listener.BeginGetContext(new AsyncCallback(RequestHandler), listener);
                    r.AsyncWaitHandle.WaitOne();
                    r.AsyncWaitHandle.Close();
                }
                catch (ThreadInterruptedException) { }
                catch (HttpListenerException e) {
                    MessageBox.Show(host.MainWindow, "Unable to process request: " + e);
                }
            }
        }

        private JsonSerializer NewJsonSerializer()
        {
            var settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;

            return JsonSerializer.Create(settings);
        }
        private Response ProcessRequest(Request r, HttpListenerResponse resp)
        {
            var response = new Response(r.RequestType);

            using (var aes = new AesManaged())
            {
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var handler = handlers[r.RequestType];
                if (handler != null)
                {
                    try
                    {
                        handler(r, response, aes);
                    }
                    catch (Exception e)
                    {
                        ShowNotification("***BUG*** " + e, (s,evt) => MessageBox.Show(host.MainWindow, e + ""));
                        response.Error = e + "";
                        resp.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    response.Error = "Unknown command: " + r.RequestType;
                    resp.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }

            return response;
        }
        private void RequestHandler(IAsyncResult r)
        {
            if (stopped) return;
            var l    = (HttpListener)r.AsyncState;
            HttpListenerContext ctx = null;
            try {
                ctx  = l.EndGetContext(r);
            } catch (HttpListenerException e) {
                MessageBox.Show(host.MainWindow, "Unable to get HttpListenerContext: " + e);
                return;
            }
            var req  = ctx.Request;
            var resp = ctx.Response;

            var db = host.Database;
            if (db.IsOpen)
            {
                var serializer = NewJsonSerializer();
                Request request = null;

                resp.StatusCode = (int)HttpStatusCode.OK;
                using (var ins = new JsonTextReader(new StreamReader(req.InputStream))) {
                    try
                    {
                        request = serializer.Deserialize<Request>(ins);
                    }
                    catch (JsonSerializationException e) {
                        var buffer = Encoding.UTF8.GetBytes(e + "");
                        resp.StatusCode = (int)HttpStatusCode.BadRequest;
                        resp.ContentLength64 = buffer.Length;
                        resp.OutputStream.Write(buffer, 0, buffer.Length);
                    } // ignore, bad request
                }


                Response response = null;
                if (request != null)
                    response = ProcessRequest(request, resp);

                resp.ContentType = "application/json";
                var writer = new StringWriter();
                if (response != null)
                {
                    serializer.Serialize(writer, response);
                    var buffer = Encoding.UTF8.GetBytes(writer.ToString());
                    resp.ContentLength64 = buffer.Length;
                    resp.OutputStream.Write(buffer, 0, buffer.Length);
                }
            }
            else
            {
                resp.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            }

            var outs = resp.OutputStream;
            outs.Close();
            resp.Close();
        }

        public override void Terminate()
        {
            stopped = true;
            listener.Stop();
            listener.Close();
            httpThread.Interrupt();
        }

        private void UpdateUI(PwGroup group)
        {
            var win = host.MainWindow;
            if (group == null) group = host.Database.RootGroup;
            var f = (MethodInvoker) delegate {
                win.UpdateUI(false, null, true, group, true, null, true);
            };
            if (win.InvokeRequired)
                win.Invoke(f);
            else
                f.Invoke();
        }

        internal string[] GetUserPass(PwEntry entry)
        {
            // follow references
            string user = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.UserNameField), false, entry, host.Database, false, false);
            string pass = SprEngine.Compile(entry.Strings.ReadSafe(PwDefs.PasswordField), false, entry, host.Database, false, false);
            var f = (MethodInvoker)delegate
            {
                // apparently, SprEngine.Compile might modify the database
                host.MainWindow.UpdateUI(false, null, false, null, false, null, false);
            };
            if (host.MainWindow.InvokeRequired)
                host.MainWindow.Invoke(f);
            else
                f.Invoke();

            return new string[] { user, pass };
        }
    }
}
