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
    public partial class ConfirmAssociationForm : Form
    {
        public ConfirmAssociationForm()
        {
            InitializeComponent();
            Saved = false;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            var value = KeyName.Text;
            if (value != null && value.Trim() != "")
            {
                Saved = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string KeyId
        {
            get
            {
                return Saved ? KeyName.Text : null;
            }
        }

        public bool Saved { get; private set; }
        public string Key
        {
            get
            { 
                return KeyLabel.Text;
            }
            set
            {
                KeyLabel.Text = value;
            }
        }
    }
}
