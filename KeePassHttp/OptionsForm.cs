using KeePassLib;
using KeePassHttp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeePassLib.Collections;

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


        private PwEntry GetConfigEntry(PwDatabase db)
        {
            var kphe = new KeePassHttpExt();
            var root = db.RootGroup;
            var uuid = new PwUuid(kphe.KEEPASSHTTP_UUID);
            var entry = root.FindEntry(uuid, false);
            return entry;
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            credNotifyCheckbox.Checked = _config.ReceiveCredentialNotification;
            credMatchingCheckbox.Checked = _config.SpecificMatchingOnly;
            unlockDatabaseCheckbox.Checked = _config.UnlockDatabaseRequest;
            credAllowAccessCheckbox.Checked = _config.AlwaysAllowAccess;
            credAllowUpdatesCheckbox.Checked = _config.AlwaysAllowUpdates;
            credSearchInAllOpenedDatabases.Checked = _config.SearchInAllOpenedDatabases;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _config.ReceiveCredentialNotification = credNotifyCheckbox.Checked;
            _config.SpecificMatchingOnly = credMatchingCheckbox.Checked;
            _config.UnlockDatabaseRequest = unlockDatabaseCheckbox.Checked;
            _config.AlwaysAllowAccess = credAllowAccessCheckbox.Checked;
            _config.AlwaysAllowUpdates = credAllowUpdatesCheckbox.Checked;
            _config.SearchInAllOpenedDatabases = credSearchInAllOpenedDatabases.Checked;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (KeePass.Program.MainForm.DocumentManager.ActiveDatabase.IsOpen)
            {
                PwDatabase db = KeePass.Program.MainForm.DocumentManager.ActiveDatabase;
                var entry = GetConfigEntry(db);
                if (entry != null)
                {
                    List<string> deleteKeys = new List<string>();

                    foreach (var s in entry.Strings)
                    {
                        if (s.Key.IndexOf(KeePassHttpExt.ASSOCIATE_KEY_PREFIX) == 0)
                        {
                            deleteKeys.Add(s.Key);
                        }
                    }


                    if(deleteKeys.Count > 0)
                    {
                        PwObjectList<PwEntry> m_vHistory = entry.History.CloneDeep();
                        entry.History = m_vHistory;
                        entry.CreateBackup(null);

                        foreach (var key in deleteKeys)
                        {
                            entry.Strings.Remove(key);
                        }

                        entry.Touch(true);
                        KeePass.Program.MainForm.UpdateUI(false, null, true, db.RootGroup, true, null, true);
                        MessageBox.Show(
                            String.Format("Successfully removed {0} connection{1} from KeePassHttp Settings.", deleteKeys.Count.ToString(), deleteKeys.Count == 1 ? "" : "s"),
                            "Removed " + deleteKeys.Count.ToString() + " from database",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "No existing connections found in KeePassHttp Settings.", "No existing connections found",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }
                else
                {
                    MessageBox.Show("The active database does not contain an entry of KeePassHttp Settings.", "KeePassHttp Settings not available!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("The active database is locked!\nPlease unlock the selected database or choose another one which is unlocked.", "Database locked!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void removePermissionsButton_Click(object sender, EventArgs e)
        {
            if (KeePass.Program.MainForm.DocumentManager.ActiveDatabase.IsOpen)
            {
                PwDatabase db = KeePass.Program.MainForm.DocumentManager.ActiveDatabase;

                uint counter = 0;
                var entries = db.RootGroup.GetEntries(true);

                if (entries.Count() > 999)
                {
                    MessageBox.Show(
                        String.Format("{0} entries detected!\nSearching and removing permissions could take some while.\n\nWe will inform you when the process has been finished.", entries.Count().ToString()),
                        String.Format("{0} entries detected", entries.Count().ToString()),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                foreach (var entry in entries) {
                    foreach (var str in entry.Strings)
                    {
                        if (str.Key == KeePassHttpExt.KEEPASSHTTP_NAME)
                        {
                            PwObjectList<PwEntry> m_vHistory = entry.History.CloneDeep();
                            entry.History = m_vHistory;
                            entry.CreateBackup(null);

                            entry.Strings.Remove(str.Key);

                            entry.Touch(true);

                            counter += 1;

                            break;
                        }
                    }
                }

                if (counter > 0)
                {
                    KeePass.Program.MainForm.UpdateUI(false, null, true, db.RootGroup, true, null, true);
                    MessageBox.Show(
                        String.Format("Successfully removed permissions from {0} entr{1}.", counter.ToString(), counter == 1 ? "y" : "ies"),
                        String.Format("Removed permissions from {0} entr{1}", counter.ToString(), counter == 1 ? "y" : "ies"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "The active database does not contain an entry with permissions.",
                        "No entry with permissions found!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            else
            {
                MessageBox.Show("The active database is locked!\nPlease unlock the selected database or choose another one which is unlocked.", "Database locked!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
