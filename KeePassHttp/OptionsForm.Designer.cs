namespace KeePassHttp
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.SortByUsernameRadioButton = new System.Windows.Forms.RadioButton();
            this.SortByTitleRadioButton = new System.Windows.Forms.RadioButton();
            this.hideExpiredCheckbox = new System.Windows.Forms.CheckBox();
            this.matchSchemesCheckbox = new System.Windows.Forms.CheckBox();
            this.removePermissionsButton = new System.Windows.Forms.Button();
            this.unlockDatabaseCheckbox = new System.Windows.Forms.CheckBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.credMatchingCheckbox = new System.Windows.Forms.CheckBox();
            this.credNotifyCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.returnStringFieldsWithKphOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.hostName = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.portNumber = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.returnStringFieldsCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.credSearchInAllOpenedDatabases = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.credAllowUpdatesCheckbox = new System.Windows.Forms.CheckBox();
            this.credAllowAccessCheckbox = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(313, 508);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 28);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(219, 508);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(88, 28);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(1, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(410, 498);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.SortByUsernameRadioButton);
            this.tabPage1.Controls.Add(this.SortByTitleRadioButton);
            this.tabPage1.Controls.Add(this.hideExpiredCheckbox);
            this.tabPage1.Controls.Add(this.matchSchemesCheckbox);
            this.tabPage1.Controls.Add(this.removePermissionsButton);
            this.tabPage1.Controls.Add(this.unlockDatabaseCheckbox);
            this.tabPage1.Controls.Add(this.removeButton);
            this.tabPage1.Controls.Add(this.credMatchingCheckbox);
            this.tabPage1.Controls.Add(this.credNotifyCheckbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(402, 497);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // SortByUsernameRadioButton
            // 
            this.SortByUsernameRadioButton.AutoSize = true;
            this.SortByUsernameRadioButton.Location = new System.Drawing.Point(7, 147);
            this.SortByUsernameRadioButton.Name = "SortByUsernameRadioButton";
            this.SortByUsernameRadioButton.Size = new System.Drawing.Size(171, 17);
            this.SortByUsernameRadioButton.TabIndex = 19;
            this.SortByUsernameRadioButton.TabStop = true;
            this.SortByUsernameRadioButton.Text = "Sort found entries by &username";
            this.SortByUsernameRadioButton.UseVisualStyleBackColor = true;
            // 
            // SortByTitleRadioButton
            // 
            this.SortByTitleRadioButton.AutoSize = true;
            this.SortByTitleRadioButton.Location = new System.Drawing.Point(7, 170);
            this.SortByTitleRadioButton.Name = "SortByTitleRadioButton";
            this.SortByTitleRadioButton.Size = new System.Drawing.Size(141, 17);
            this.SortByTitleRadioButton.TabIndex = 18;
            this.SortByTitleRadioButton.TabStop = true;
            this.SortByTitleRadioButton.Text = "Sort found entries by &title";
            this.SortByTitleRadioButton.UseVisualStyleBackColor = true;
            //
            // hideExpiredCheckbox
            //
            this.hideExpiredCheckbox.AutoSize = true;
            this.hideExpiredCheckbox.Location = new System.Drawing.Point(7, 88);
            this.hideExpiredCheckbox.Name = "hideExpiredCheckbox";
            this.hideExpiredCheckbox.Size = new System.Drawing.Size(256, 17);
            this.hideExpiredCheckbox.TabIndex = 17;
            this.hideExpiredCheckbox.Text = "Don't return e&xpired entries";
            this.hideExpiredCheckbox.UseVisualStyleBackColor = true;
            //
            // matchSchemesCheckbox
            //
            this.matchSchemesCheckbox.AutoSize = true;
            this.matchSchemesCheckbox.Location = new System.Drawing.Point(7, 111);
            this.matchSchemesCheckbox.Name = "matchSchemesCheckbox";
            this.matchSchemesCheckbox.Size = new System.Drawing.Size(375, 30);
            this.matchSchemesCheckbox.TabIndex = 17;
            this.matchSchemesCheckbox.Text = "&Match URL schemes\r\nonly entries with the same scheme (http://, https://, ftp://," +
    " ...) are returned";
            this.matchSchemesCheckbox.UseVisualStyleBackColor = true;
            // 
            // removePermissionsButton
            // 
            this.removePermissionsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removePermissionsButton.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.removePermissionsButton.Location = new System.Drawing.Point(14, 239);
            this.removePermissionsButton.Name = "removePermissionsButton";
            this.removePermissionsButton.Size = new System.Drawing.Size(372, 28);
            this.removePermissionsButton.TabIndex = 16;
            this.removePermissionsButton.Text = "Remo&ve all stored permissions from entries in active database";
            this.removePermissionsButton.UseVisualStyleBackColor = true;
            this.removePermissionsButton.Click += new System.EventHandler(this.removePermissionsButton_Click);
            // 
            // unlockDatabaseCheckbox
            // 
            this.unlockDatabaseCheckbox.AutoSize = true;
            this.unlockDatabaseCheckbox.Location = new System.Drawing.Point(7, 65);
            this.unlockDatabaseCheckbox.Name = "unlockDatabaseCheckbox";
            this.unlockDatabaseCheckbox.Size = new System.Drawing.Size(256, 17);
            this.unlockDatabaseCheckbox.TabIndex = 15;
            this.unlockDatabaseCheckbox.Text = "Re&quest for unlocking the database if it is locked";
            this.unlockDatabaseCheckbox.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(14, 205);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(372, 28);
            this.removeButton.TabIndex = 11;
            this.removeButton.Text = "R&emove all shared encryption-keys from active database";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // credMatchingCheckbox
            // 
            this.credMatchingCheckbox.AutoSize = true;
            this.credMatchingCheckbox.Location = new System.Drawing.Point(7, 29);
            this.credMatchingCheckbox.Name = "credMatchingCheckbox";
            this.credMatchingCheckbox.Size = new System.Drawing.Size(238, 30);
            this.credMatchingCheckbox.TabIndex = 9;
            this.credMatchingCheckbox.Text = "&Return only best matching entries for an URL\r\ninstead of all entries for the who" +
    "le domain";
            this.credMatchingCheckbox.UseVisualStyleBackColor = true;
            // 
            // credNotifyCheckbox
            // 
            this.credNotifyCheckbox.AutoSize = true;
            this.credNotifyCheckbox.Location = new System.Drawing.Point(7, 6);
            this.credNotifyCheckbox.Name = "credNotifyCheckbox";
            this.credNotifyCheckbox.Size = new System.Drawing.Size(267, 17);
            this.credNotifyCheckbox.TabIndex = 8;
            this.credNotifyCheckbox.Text = "Sh&ow a notification when credentials are requested";
            this.credNotifyCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.returnStringFieldsWithKphOnlyCheckBox);
            this.tabPage2.Controls.Add(this.hostName);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.label9);
            this.tabPage2.Controls.Add(this.label7);
            this.tabPage2.Controls.Add(this.portNumber);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.returnStringFieldsCheckbox);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.credSearchInAllOpenedDatabases);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.credAllowUpdatesCheckbox);
            this.tabPage2.Controls.Add(this.credAllowAccessCheckbox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(402, 472);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // returnStringFieldsWithKphOnlyCheckBox
            // 
            this.returnStringFieldsWithKphOnlyCheckBox.AutoSize = true;
            this.returnStringFieldsWithKphOnlyCheckBox.Location = new System.Drawing.Point(55, 215);
            this.returnStringFieldsWithKphOnlyCheckBox.Name = "returnStringFieldsWithKphOnlyCheckBox";
            this.returnStringFieldsWithKphOnlyCheckBox.Size = new System.Drawing.Size(300, 30);
            this.returnStringFieldsWithKphOnlyCheckBox.TabIndex = 31;
            this.returnStringFieldsWithKphOnlyCheckBox.Text = "Only return advanced string fields which start with \"KPH: \"\r\n(Mind the space afte" +
    "r KPH:)";
            this.returnStringFieldsWithKphOnlyCheckBox.UseVisualStyleBackColor = true;
            // 
            // hostName
            // 
            this.hostName.Location = new System.Drawing.Point(48, 318);
            this.hostName.Name = "hostName";
            this.hostName.Size = new System.Drawing.Size(103, 20);
            this.hostName.TabIndex = 25;
            this.hostName.Text = "localhost";
            this.hostName.TextChanged += new System.EventHandler(this.hostName_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 283);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(375, 26);
            this.label10.TabIndex = 23;
            this.label10.Text = "Only change the host to bind to if you want to give access to other computers.\r\nU" +
    "se \'*\' to bind it to all your IP addresses (potentially dangerous!)\r\n";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(157, 318);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(244, 65);
            this.label8.TabIndex = 26;
            this.label8.Text = "Default: localhost\r\nYou might need to run KeePass as administrator \r\nwhen you cha" +
    "nge this.\r\nAlso don\'t forget to open the firewall if you want to \r\nbe able to us" +
    "e it from a different computer.";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 321);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 24;
            this.label9.Text = "Host:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(157, 415);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(241, 39);
            this.label7.TabIndex = 30;
            this.label7.Text = "Default: 19455\r\nDon\'t forget to change the port number also in\r\nthe plugins like " +
    "chromeIPass, PassIFox, kypass,...";
            // 
            // portNumber
            // 
            this.portNumber.Location = new System.Drawing.Point(48, 415);
            this.portNumber.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.portNumber.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.portNumber.Name = "portNumber";
            this.portNumber.Size = new System.Drawing.Size(60, 20);
            this.portNumber.TabIndex = 29;
            this.portNumber.Value = new decimal(new int[] {
            19455,
            0,
            0,
            0});
            this.portNumber.ValueChanged += new System.EventHandler(this.portNumber_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 393);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(312, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Change the default port number if you have connection problems";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 417);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(52, 248);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(277, 26);
            this.label4.TabIndex = 22;
            this.label4.Text = "Automatic creates or updates are not supported\r\nfor string fields!";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 156);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(289, 52);
            this.label3.TabIndex = 21;
            this.label3.Text = "If there are more fields needed than username + password,\r\nnormal \"String Fields\"" +
    " are used, which can be defined in the\r\n\"Advanced\" tab of an entry.\r\nString fiel" +
    "ds are returned in alphabetical order.";
            // 
            // returnStringFieldsCheckbox
            // 
            this.returnStringFieldsCheckbox.AutoSize = true;
            this.returnStringFieldsCheckbox.Location = new System.Drawing.Point(7, 136);
            this.returnStringFieldsCheckbox.Name = "returnStringFieldsCheckbox";
            this.returnStringFieldsCheckbox.Size = new System.Drawing.Size(186, 17);
            this.returnStringFieldsCheckbox.TabIndex = 20;
            this.returnStringFieldsCheckbox.Text = "&Return also advanced string fields";
            this.returnStringFieldsCheckbox.UseVisualStyleBackColor = true;
            this.returnStringFieldsCheckbox.CheckedChanged += new System.EventHandler(this.returnStringFieldsCheckbox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(299, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Only the selected database has to be connected with a client!";
            // 
            // credSearchInAllOpenedDatabases
            // 
            this.credSearchInAllOpenedDatabases.AutoSize = true;
            this.credSearchInAllOpenedDatabases.Location = new System.Drawing.Point(7, 88);
            this.credSearchInAllOpenedDatabases.Name = "credSearchInAllOpenedDatabases";
            this.credSearchInAllOpenedDatabases.Size = new System.Drawing.Size(270, 17);
            this.credSearchInAllOpenedDatabases.TabIndex = 18;
            this.credSearchInAllOpenedDatabases.Text = "Searc&h in all opened databases for matching entries";
            this.credSearchInAllOpenedDatabases.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(391, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Activate the following options only, if you know what you are doing!";
            // 
            // credAllowUpdatesCheckbox
            // 
            this.credAllowUpdatesCheckbox.AutoSize = true;
            this.credAllowUpdatesCheckbox.Location = new System.Drawing.Point(6, 56);
            this.credAllowUpdatesCheckbox.Name = "credAllowUpdatesCheckbox";
            this.credAllowUpdatesCheckbox.Size = new System.Drawing.Size(164, 17);
            this.credAllowUpdatesCheckbox.TabIndex = 16;
            this.credAllowUpdatesCheckbox.Text = "Always allow &updating entries";
            this.credAllowUpdatesCheckbox.UseVisualStyleBackColor = true;
            // 
            // credAllowAccessCheckbox
            // 
            this.credAllowAccessCheckbox.AutoSize = true;
            this.credAllowAccessCheckbox.Location = new System.Drawing.Point(6, 33);
            this.credAllowAccessCheckbox.Name = "credAllowAccessCheckbox";
            this.credAllowAccessCheckbox.Size = new System.Drawing.Size(169, 17);
            this.credAllowAccessCheckbox.TabIndex = 15;
            this.credAllowAccessCheckbox.Text = "Always allow &access to entries";
            this.credAllowAccessCheckbox.UseVisualStyleBackColor = true;
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(411, 545);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeePassHttp Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.CheckBox hideExpiredCheckbox;
        private System.Windows.Forms.CheckBox matchSchemesCheckbox;
        private System.Windows.Forms.Button removePermissionsButton;
        private System.Windows.Forms.CheckBox unlockDatabaseCheckbox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.CheckBox credMatchingCheckbox;
        private System.Windows.Forms.CheckBox credNotifyCheckbox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox credSearchInAllOpenedDatabases;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox credAllowUpdatesCheckbox;
        private System.Windows.Forms.CheckBox credAllowAccessCheckbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox returnStringFieldsCheckbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton SortByUsernameRadioButton;
        private System.Windows.Forms.RadioButton SortByTitleRadioButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown portNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox hostName;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox returnStringFieldsWithKphOnlyCheckBox;
    }
}