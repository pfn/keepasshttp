using KeePass.App.Configuration;

namespace KeePassHttp
{
	public class ConfigOpt
	{
		readonly AceCustomConfig _config;
		const string ReceiveCredentialNotificationKey = "KeePassHttp_ReceiveCredentialNotification";

		public ConfigOpt(AceCustomConfig config)
		{
			_config = config;
		}

		public bool ReceiveCredentialNotification
		{
			get { return _config.GetBool(ReceiveCredentialNotificationKey, true); }
			set { _config.SetBool(ReceiveCredentialNotificationKey, value); }
		}
	}
}