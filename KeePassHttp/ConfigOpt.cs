using KeePass.App.Configuration;

namespace KeePassHttp
{
    public class ConfigOpt
    {
        readonly AceCustomConfig _config;
        const string ReceiveCredentialNotificationKey = "KeePassHttp_ReceiveCredentialNotification";
        const string SpecificMatchingOnlyKey = "KeePassHttp_SpecificMatchingOnly";
        const string AlwaysAllowAccessKey = "KeePassHttp_AlwaysAllowAccess";
        const string AlwaysAllowUpdatesKey = "KeePassHttp_AlwaysAllowUpdates";
        const string SearchInAllOpenedDatabasesKey = "KeePassHttp_SearchInAllOpenedDatabases";

        public ConfigOpt(AceCustomConfig config)
        {
            _config = config;
        }

        public bool ReceiveCredentialNotification
        {
            get { return _config.GetBool(ReceiveCredentialNotificationKey, true); }
            set { _config.SetBool(ReceiveCredentialNotificationKey, value); }
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
    }
}