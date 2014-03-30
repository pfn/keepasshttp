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
        const string MatchSchemesKey = "KeePassHttp_MatchSchemes";
        const string ReturnStringFieldsKey = "KeePassHttp_ReturnStringFields";
        const string SortResultByUsernameKey = "KeePassHttp_SortResultByUsername";
        const string ListenerHostHttpKey = "KeePassHttp_ListenerHostHttp";
        const string ListenerPortHttpKey = "KeePassHttp_ListenerPortHttp";
        const string ActivateHttpsListenerKey = "KeePassHttp_ActivateHttpsListener";
        const string ListenerHostHttpsKey = "KeePassHttp_ListenerHostHttps";
        const string ListenerPortHttpsKey = "KeePassHttp_ListenerPortHttps";

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

        public bool SortResultByUsername
        {
            get { return _config.GetBool(SortResultByUsernameKey, true); }
            set { _config.SetBool(SortResultByUsernameKey, value); }
        }

        public string ListenerHostHttp {
            get { return _config.GetString(ListenerHostHttpKey, KeePassHttpExt.DEFAULT_HOST); }
            set { _config.SetString(ListenerHostHttpKey, value); }
        }
        
        public long ListenerPortHttp
        {
            get { return _config.GetLong(ListenerPortHttpKey, KeePassHttpExt.DEFAULT_PORT_HTTP); }
            set { _config.SetLong(ListenerPortHttpKey, value); }
        }

        public bool ActivateHttpsListener
        {
            get { return _config.GetBool(ActivateHttpsListenerKey, false); }
            set { _config.SetBool(ActivateHttpsListenerKey, value); }
        }

        public string ListenerHostHttps
        {
            get { return _config.GetString(ListenerHostHttpsKey, KeePassHttpExt.DEFAULT_HOST); }
            set { _config.SetString(ListenerHostHttpsKey, value); }
        }

        public long ListenerPortHttps
        {
            get { return _config.GetLong(ListenerPortHttpsKey, KeePassHttpExt.DEFAULT_PORT_HTTPS); }
            set { _config.SetLong(ListenerPortHttpsKey, value); }
        }
    }
}