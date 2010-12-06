using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using KeePassLib;

namespace KeePassHttp
{
    public partial class AccessControlForm : Form
    {
        public AccessControlForm()
        {
            InitializeComponent();
        }

        private void AllowButton_Click(object sender, EventArgs e)
        {
            Allowed = true;
            Close();
        }

        private void DenyButton_Click(object sender, EventArgs e)
        {
            Denied = true;
            Close();
        }

        public bool Allowed = false;
        public bool Denied = false;

        public bool Remember
        {
            get { return RememberCheck.Checked; }
        }

        public List<PwEntry> Entries
        {
            set
            {
                EntriesBox.SelectionMode = SelectionMode.None;
                foreach (var e in value)
                {
                    var title = e.Strings.Get(PwDefs.TitleField).ReadString();
                    var username = e.Strings.Get(PwDefs.UserNameField).ReadString();
                    EntriesBox.Items.Add(title + " - " + username);
                }
            }
        }
    }
}
