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

namespace KeePassHttp
{

    internal delegate void RequestHandler(Request request, Response response, Aes aes);

    public class KeePassHttpExt : Plugin
    {

        private static string encode64(byte[] b)
        {
            // for some reason, a \n is prepended, chop it off
            return new SoapBase64Binary(b).ToString().Substring(1);
        }

        private static byte[] decode64(string s)
        {
            return SoapBase64Binary.Parse(s).Value;
        }
        /// <summary>
        /// an arbitrarily generated uuid for the keepasshttp root entry
        /// </summary>
        private readonly byte[] KEEPASSHTTP_UUID = {
                0x34, 0x69, 0x7a, 0x40, 0x8a, 0x5b, 0x41, 0xc0,
                0x9f, 0x36, 0x89, 0x7d, 0x62, 0x3e, 0xcb, 0x31
                                                };
        private const string KEEPASSHTTP_NAME = "KeePassHttp Settings";
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
            p.SearchInUrls = false;
            p.SearchInUserNames = false;
            p.SearchInUuids = false;
            return p;
        }

        private void GetAllLoginsHandler(Request r, Response resp, Aes aes)
        {
            if (!VerifyRequest(r, aes))
                return;

            var list = new PwObjectList<PwEntry>();

            var root = host.Database.RootGroup;

            var parms = MakeSearchParameters();

            parms.SearchString = "\\."; // match anything look like a domain name

            root.SearchEntries(parms, list, false);
            foreach (var entry in list) {
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
                    entry.Name  = CryptoTransform(entry.Name,  false, true, enc);
                    entry.Login = CryptoTransform(entry.Login, false, true, enc);
                    entry.Uuid  = CryptoTransform(entry.Uuid,  false, true, enc);
                }
            }
        }
        private void GetLoginsHandler(Request r, Response resp, Aes aes)
        {
        }
        private void SetLoginHandler(Request r, Response resp, Aes aes)
        {
        }
        private void AssociateHandler(Request r, Response resp, Aes aes)
        {
            if (!TestRequestVerifier(r, aes, r.Key))
                return;

            var notify = host.MainWindow.MainNotifyIcon;

            notify.BalloonTipIcon = ToolTipIcon.Info;
            notify.BalloonTipTitle = "KeePassHttp";
            notify.BalloonTipText = "New key association requested";
            notify.ShowBalloonTip(3000);

            // key is good, prompt user to save
            using (var f = new ConfirmAssociationForm())
            {
                var win = host.MainWindow;
                win.Invoke((MethodInvoker)delegate {
                    f.Icon = win.Icon;
                    f.Key = r.Key;
                    f.Load += delegate { f.Activate();  };
                    f.ShowDialog(win);
                    if (f.KeyId != null)
                    {
                        var entry = GetConfigEntry(true);
                        entry.Strings.Set(f.KeyId, new ProtectedString(true, r.Key));
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

        private bool VerifyRequest(Request r, Aes aes) {
            var entry = GetConfigEntry(false);
            if (entry == null)
                return false;
            var s = entry.Strings.Get(r.Id);
            if (s == null)
                return false;

            return TestRequestVerifier(r, aes, s.ReadString());
        }

        private bool TestRequestVerifier(Request r, Aes aes, string key)
        {
            var success = false;
            var crypted = decode64(r.Verifier);

            aes.Key = decode64(key);
            aes.IV = decode64(r.Nonce);

            using (var dec = aes.CreateDecryptor())
            {
                var buf = dec.TransformFinalBlock(crypted, 0, crypted.Length);
                var value = Encoding.UTF8.GetString(buf);
                success = value == r.Nonce;
            }
            return success;
        }

        private void SetResponseVerifier(Response r, Aes aes)
        {
            aes.GenerateIV();
            r.Nonce = encode64(aes.IV);
            using (var enc = aes.CreateEncryptor())
            {
                r.Verifier = CryptoTransform(r.Nonce, false, true, enc);
            }
        }

        private string CryptoTransform(string input, bool base64in, bool base64out, ICryptoTransform codec)
        {
            byte[] bytes;
            if (base64in)
                bytes = decode64(input);
            else
                bytes = Encoding.UTF8.GetBytes(input);

            var buf = codec.TransformFinalBlock(bytes, 0, bytes.Length);

            return base64out ? encode64(buf) : Encoding.UTF8.GetString(buf);
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
                host.MainWindow.UpdateUI(false, null, true, root, true, null, true);
            }
            return entry;
        }

        public override bool Initialize(IPluginHost host)
        {
            var httpSupported = HttpListener.IsSupported;
            this.host = host;
            if (httpSupported)
            {
                handlers.Add(Request.TEST_ASSOCIATE, TestAssociateHandler);
                handlers.Add(Request.ASSOCIATE,      AssociateHandler);
                handlers.Add(Request.GET_LOGINS,     GetLoginsHandler);
                handlers.Add(Request.GET_ALL_LOGINS, GetAllLoginsHandler);
                handlers.Add(Request.SET_LOGIN,      SetLoginHandler);

                listener = new HttpListener();
                listener.Prefixes.Add(HTTP_PREFIX + port + "/");
                listener.Start();

                httpThread = new Thread(new ThreadStart(Run));
                httpThread.Start();
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
            }
        }

        private Response ProcessRequest(Request r, HttpListenerResponse resp)
        {
            var response = new Response(r.RequestType);

            using (var aes = new AesManaged())
            {
                var handler = handlers[r.RequestType];
                if (handler != null)
                {
                    try
                    {
                        handler(r, response, aes);
                    }
                    catch
                    {
                        resp.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    resp.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            }

            return response;
        }
        private void RequestHandler(IAsyncResult r)
        {
            if (stopped) return;
            var l    = (HttpListener)r.AsyncState;
            var ctx  = l.EndGetContext(r);
            var req  = ctx.Request;
            var resp = ctx.Response;

            var db = host.Database;
            if (db.IsOpen)
            {
                var settings = new JsonSerializerSettings();
                settings.DefaultValueHandling = DefaultValueHandling.Ignore;
                settings.NullValueHandling    = NullValueHandling.Ignore;

                var serializer = JsonSerializer.Create(settings);
                Request request = null;

                using (var ins = new JsonTextReader(new StreamReader(req.InputStream))) {
                    try
                    {
                        request = serializer.Deserialize<Request>(ins);
                    }
                    catch (JsonSerializationException) { } // ignore, bad request
                }

                Response response = null;
                if (request != null)
                    response = ProcessRequest(request, resp);

                resp.StatusCode = (int)HttpStatusCode.OK;
                resp.ContentType = "application/json";
                var writer = new StringWriter();
                serializer.Serialize(writer, response);
                var buffer = Encoding.UTF8.GetBytes(writer.ToString());
                resp.OutputStream.Write(buffer, 0, buffer.Length);
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
    }
    public class Request
    {
        public const string GET_LOGINS     = "get-logins";
        public const string GET_ALL_LOGINS = "get-all-logins";
        public const string SET_LOGIN      = "set-login";
        public const string ASSOCIATE      = "associate";
        public const string TEST_ASSOCIATE = "test-associate";

        public string RequestType;

        /// <summary>
        /// Always encrypted, used with set-login
        /// </summary>
        public string Login;
        public string Password;
        
        /// <summary>
        /// Always encrypted, used with get and set-login
        /// </summary>
        public string Url;

        /// <summary>
        /// Send the AES key ID with the 'associate' request
        /// </summary>
        public string Key;

        /// <summary>
        /// Always required, an identifier given by the KeePass user
        /// </summary>
        public string Id;
        /// <summary>
        /// A value used to ensure that the correct key has been chosen,
        /// it is always the value of Nonce encrypted with Key
        /// </summary>
        public string Verifier;
        /// <summary>
        /// Nonce value used in conjunction with all encrypted fields,
        /// randomly generated for each request
        /// </summary>
        public string Nonce;
    }

    public class Response
    {
        public Response(string request)
        {
            RequestType = request;

            if (request == Request.GET_LOGINS || request == Request.GET_ALL_LOGINS)
                Entries = new List<ResponseEntry>();
            else
                Entries = null;
        }

        /// <summary>
        /// Mirrors the request type of KeePassRequest
        /// </summary>
        public string RequestType;

        public bool Success = false;

        /// <summary>
        /// The user selected string as a result of 'associate',
        /// always returned on every request
        /// </summary>
        public string Id;

        /// <summary>
        /// The resulting entries for a get-login request
        /// </summary>
        public List<ResponseEntry> Entries { get; private set; }

        /// <summary>
        /// Nonce value used in conjunction with all encrypted fields,
        /// randomly generated for each request
        /// </summary>
        public string Nonce;

        /// <summary>
        /// Same purpose as Request.Verifier, but a new value
        /// </summary>
        public string Verifier;
    }
    public class ResponseEntry
    {
        public ResponseEntry() { }
        public ResponseEntry(string name, string login, string password, string uuid)
        {
            Login    = login;
            Password = password;
            Uuid     = uuid;
            Name     = name;
        }
        public string Login;
        public string Password;
        public string Uuid;
        public string Name;
    }
}
