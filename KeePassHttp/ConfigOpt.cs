using KeePass.App.Configuration;

namespace KeePassHttp
{
    public class ConfigOpt
    {
        readonly AceCustomConfig _config;
        const string ReceiveCredentialNotificationKey = "KeePassHttp_ReceiveCredentialNotification";
        const string SpecificMatchingOnlyKey = "KeePassHttp_SpecificMatchingOnly";
        const string UnlockDatabaseRequestKey = "KeePassHttp_UnlockDatabaseRequest";
        const string AlwaysAllowAccessKey = "KeePassHttp_AlwaysAllowAccess";
        const string AlwaysAllowUpdatesKey = "KeePassHttp_AlwaysAllowUpdates";
        const string SearchInAllOpenedDatabasesKey = "KeePassHttp_SearchInAllOpenedDatabases";
        const string HideExpiredKey = "KeePassHttp_HideExpired";
        const string MatchSchemesKey = "KeePassHttp_MatchSchemes";
        const string ReturnStringFieldsKey = "KeePassHttp_ReturnStringFields";
        const string ReturnStringFieldsWithKphOnlyKey = "KeePassHttp_ReturnStringFieldsWithKphOnly";
        const string SortResultByUsernameKey = "KeePassHttp_SortResultByUsername";
        const string ListenerPortKey = "KeePassHttp_ListenerPort";
        const string ListenerHostKey = "KeePassHttp_ListenerHost";

        public ConfigOpt(AceCustomConfig config)
        {
            _config = config;
        }

        public bool ReceiveCredentialNotification
        {
            get { return _config.GetBool(ReceiveCredentialNotificationKey, true); }
            set { _config.SetBool(ReceiveCredentialNotificationKey, value); }
        }

        public bool UnlockDatabaseRequest
        {
            get { return _config.GetBool(UnlockDatabaseRequestKey, false); }
            set { _config.SetBool(UnlockDatabaseRequestKey, value); }
        }

        public bool SpecificMatchingOnly
        {
            get { return _config.GetBool(SpecificMatchingOnlyKey, false); }
            set { _config.SetBool(SpecificMatchingOnlyKey, value); }
        }

        public bool AlwaysAllowAccess
        {
            get { return _config.GetBool(AlwaysAllowAccessKey, false); }
            set { _config.SetBool(AlwaysAllowAccessKey, value); }
        }

        public bool AlwaysAllowUpdates
        {
            get { return _config.GetBool(AlwaysAllowUpdatesKey, false); }
            set { _config.SetBool(AlwaysAllowUpdatesKey, value); }
        }

        public bool SearchInAllOpenedDatabases
        {
            get { return _config.GetBool(SearchInAllOpenedDatabasesKey, false); }
            set { _config.SetBool(SearchInAllOpenedDatabasesKey, value); }
        }

        public bool HideExpired
        {
            get { return _config.GetBool(HideExpiredKey, false); }
            set { _config.SetBool(HideExpiredKey, value); }
        }
        public bool MatchSchemes
        {
            get { return _config.GetBool(MatchSchemesKey, false); }
            set { _config.SetBool(MatchSchemesKey, value); }
        }

        public bool ReturnStringFields
        {
            get { return _config.GetBool(ReturnStringFieldsKey, false); }
            set { _config.SetBool(ReturnStringFieldsKey, value); }
        }

        public bool ReturnStringFieldsWithKphOnly
        {
            get { return _config.GetBool(ReturnStringFieldsWithKphOnlyKey, true); }
            set { _config.SetBool(ReturnStringFieldsWithKphOnlyKey, value); }
        }

        public bool SortResultByUsername
        {
            get { return _config.GetBool(SortResultByUsernameKey, true); }
            set { _config.SetBool(SortResultByUsernameKey, value); }
        }

        public long ListenerPort
        {
            get { return _config.GetLong(ListenerPortKey, KeePassHttpExt.DEFAULT_PORT); }
            set { _config.SetLong(ListenerPortKey, value); }
        }

        public string ListenerHost
        {
            get { return _config.GetString(ListenerHostKey, KeePassHttpExt.DEFAULT_HOST); }
            set { _config.SetString(ListenerHostKey, value); }
        }
    }
}