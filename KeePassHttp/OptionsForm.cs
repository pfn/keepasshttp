using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeePassHttp
{
	public partial class OptionsForm : Form
	{
		readonly ConfigOpt _config;

		public OptionsForm(ConfigOpt config)
		{
			_config = config;
			InitializeComponent();
		}

		private void OptionsForm_Load(object sender, EventArgs e)
		{
			credNotifyCheckbox.Checked = _config.ReceiveCredentialNotification;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			_config.ReceiveCredentialNotification = credNotifyCheckbox.Checked;
		}
	}
}
