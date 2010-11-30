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
        private const int DEFAULT_PORT = 9455;
        /// <summary>
        /// TODO make configurable
        /// </summary>
        private int port = DEFAULT_PORT;
        private const string HTTP_PREFIX = "http://localhost:";
        private Thread httpThread;
        private volatile bool stopped = false;
        Dictionary<string,RequestHandler> handlers = new Dictionary<string,RequestHandler>();

        private void GetLoginHandler(Request r, Response resp, Aes aes)
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
            var entry = GetConfigEntry(false);
            if (entry == null)
                return;
            var s = entry.Strings.Get(r.Id);
            if (s == null)
                return;

            if (!TestRequestVerifier(r, aes, s.ReadString()))
                return;

            resp.Success = true;
            resp.Id = r.Id;
            SetResponseVerifier(resp, aes);
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
            aes.IV = decode64(r.Nonce);
            using (var enc = aes.CreateEncryptor())
            {
                var bytes = Encoding.UTF8.GetBytes(r.Nonce);
                var buf = enc.TransformFinalBlock(bytes, 0, bytes.Length);
                r.Verifier = encode64(buf);
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
                handlers.Add(Request.GET_LOGIN,      GetLoginHandler);
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
                    var r = listener.BeginGetContext(new AsyncCallback(Listener), listener);
                    r.AsyncWaitHandle.WaitOne();
                    r.AsyncWaitHandle.Close();
                }
                catch (ThreadInterruptedException) { }
            }
        }

        private Response ProcessRequest(Request r, HttpListenerResponse resp)
        {
            var response = new Response(r.RequestType);
            var crypted = decode64(r.Verifier);
            var iv      = decode64(r.Nonce);

            using (var aes = new AesManaged())
            {
                var handler = handlers[r.RequestType];
                if (handler != null)
                {
                    aes.GenerateIV();
                    response.Nonce = encode64(aes.IV);
                    try
                    {
                        handler(r, response, aes);
                    }
                    catch
                    {
                        resp.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    if (!response.Success)
                        response.Nonce = null;
                }
            }

            return response;
        }
        private void Listener(IAsyncResult r)
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
                var list = new PwObjectList<PwEntry>();
                Response response = null;
                if (request != null)
                {
                    response = ProcessRequest(request, resp);

                    if (request.Url != null)
                    {
                        try
                        {
                            var url = new Uri(request.Url);
                        }
                        catch (UriFormatException) { } // ignore, bad url
                    }

                    var root = db.RootGroup;
                    var parms = new SearchParameters();

                    parms.SearchInTitles = true;
                    parms.RegularExpression = true;
                    parms.SearchInGroupNames = false;
                    parms.SearchInNotes = false;
                    parms.SearchInOther = false;
                    parms.SearchInPasswords = false;
                    parms.SearchInTags = false;
                    parms.SearchInUrls = false;
                    parms.SearchInUserNames = false;
                    parms.SearchInUuids = false;
                    parms.SearchString = "google\\.com$";

                    root.SearchEntries(parms, list, true);
                }
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
        public const string GET_LOGIN      = "get-login";
        public const string SET_LOGIN      = "set-login";
        public const string ASSOCIATE      = "associate";
        public const string TEST_ASSOCIATE = "test-associate";

        public Request(string request, string id, string nonce)
        {
            RequestType = request;
            Id          = id;
            Nonce       = nonce;
            Verifier    = null;
            Login       = null;
            Password    = null;
            Url         = null;
            Key         = null;
        }
        /// <summary>
        ///   Requests: 'get-login', 'set-login', 'associate', 'test-associate'
        /// </summary>
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

            if (request == KeePassHttp.Request.GET_LOGIN)
                Entries = new List<EntryResponse>();
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
        public List<EntryResponse> Entries { get; private set; }

        /// <summary>
        /// Nonce value used in conjunction with all encrypted fields,
        /// randomly generated for each request
        /// </summary>
        public string Nonce;

        /// <summary>
        /// Same as Request.Verifier
        /// </summary>
        public string Verifier;
    }
    public class EntryResponse
    {
        public EntryResponse() { }
        public EntryResponse(string login, string password)
        {
            Login    = login;
            Password = password;
        }
        public string Login;
        public string Password;
    }
}
