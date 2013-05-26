using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using KeePass.Plugins;
using System.Reflection;
using System.Diagnostics;

namespace KeePassHttp
{
    public sealed partial class KeePassHttpExt : Plugin
    {
        private static string encode64(byte[] b)
        {
            return System.Convert.ToBase64String(b);
        }

        private static byte[] decode64(string s)
        {
            return System.Convert.FromBase64String(s);
        }
        private bool VerifyRequest(Request r, Aes aes)
        {
            var entry = GetConfigEntry(false);
            if (entry == null)
                return false;
            var s = entry.Strings.Get(ASSOCIATE_KEY_PREFIX + r.Id);
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
                try {
                    var buf = dec.TransformFinalBlock(crypted, 0, crypted.Length);
                    var value = Encoding.UTF8.GetString(buf);
                    success = value == r.Nonce;
                } catch (CryptographicException) { } // implicit failure
            }
            return success;
        }

        private void SetResponseVerifier(Response r, Aes aes)
        {
            aes.GenerateIV();
            r.Nonce = encode64(aes.IV);
            r.Verifier = CryptoTransform(r.Nonce, false, true, aes, CMode.ENCRYPT);
        }
    }
    public class Request
    {
        public const string GET_LOGINS = "get-logins";
        public const string GET_LOGINS_COUNT = "get-logins-count";
        public const string GET_ALL_LOGINS = "get-all-logins";
        public const string SET_LOGIN = "set-login";
        public const string ASSOCIATE = "associate";
        public const string TEST_ASSOCIATE = "test-associate";
        public const string GENERATE_PASSWORD = "generate-password";

        public string RequestType;

        /// <summary>
        /// Sort selection by best URL matching for given hosts
        /// </summary>
        public string SortSelection;

        /// <summary>
        /// Trigger unlock of database even if feature is disabled in KPH (because of user interaction to fill-in)
        /// </summary>
        public string TriggerUnlock;

        /// <summary>
        /// Always encrypted, used with set-login, uuid is set
        /// if modifying an existing login
        /// </summary>
        public string Login;
        public string Password;
        public string Uuid;

        /// <summary>
        /// Always encrypted, used with get and set-login
        /// </summary>
        public string Url;

        /// <summary>
        /// Always encrypted, used with get-login
        /// </summary>
        public string SubmitUrl;

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

        /// <summary>
        /// Realm value used for filtering results.  Always encrypted.
        /// </summary>
        public string Realm;
    }

    public class Response
    {
        public Response(string request, string hash)
        {
            RequestType = request;

            if (request == Request.GET_LOGINS || request == Request.GET_ALL_LOGINS || request == Request.GENERATE_PASSWORD)
                Entries = new List<ResponseEntry>();
            else
                Entries = null;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.Version = fvi.ProductVersion;

            this.Hash = hash;
        }

        /// <summary>
        /// Mirrors the request type of KeePassRequest
        /// </summary>
        public string RequestType;

        public string Error = null;

        public bool Success = false;

        /// <summary>
        /// The user selected string as a result of 'associate',
        /// always returned on every request
        /// </summary>
        public string Id;

        /// <summary>
        /// response to get-logins-count, number of entries for requested Url
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// response the current version of KeePassHttp
        /// </summary>
        public string Version = "";

        /// <summary>
        /// response an unique hash of the database composed of RootGroup UUid and RecycleBin UUid
        /// </summary>
        public string Hash = "";

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
        public ResponseEntry(string name, string login, string password, string uuid, List<ResponseStringField> stringFields)
        {
            Login = login;
            Password = password;
            Uuid = uuid;
            Name = name;
            StringFields = stringFields;
        }
        public string Login;
        public string Password;
        public string Uuid;
        public string Name;
        public List<ResponseStringField> StringFields = null;
    }
    public class ResponseStringField
    {
        public ResponseStringField() {}
        public ResponseStringField(string key, string value)
        {
            Key = key;
            Value = value;
        }
        public string Key;
        public string Value;
    }
    public class KeePassHttpEntryConfig
    {
        public HashSet<string> Allow = new HashSet<string>();
        public HashSet<string> Deny = new HashSet<string>();
        public string Realm = null;
    }
}
