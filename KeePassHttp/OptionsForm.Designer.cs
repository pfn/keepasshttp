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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.SortByUsernameRadioButton = new System.Windows.Forms.RadioButton();
            this.SortByTitleRadioButton = new System.Windows.Forms.RadioButton();
            this.matchSchemesCheckbox = new System.Windows.Forms.CheckBox();
            this.removePermissionsButton = new System.Windows.Forms.Button();
            this.unlockDatabaseCheckbox = new System.Windows.Forms.CheckBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.credMatchingCheckbox = new System.Windows.Forms.CheckBox();
            this.credNotifyCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.returnStringFieldsCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.credSearchInAllOpenedDatabases = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.credAllowUpdatesCheckbox = new System.Windows.Forms.CheckBox();
            this.credAllowAccessCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.instructionsLink = new System.Windows.Forms.LinkLabel();
            this.activateHttpsListenerCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.listenerHostHttp = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.portNumberHttp = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.listenerHostHttps = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.portNumberHttps = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumberHttp)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumberHttps)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(649, 791);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(176, 54);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(461, 791);
            this.okButton.Margin = new System.Windows.Forms.Padding(6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(176, 54);
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
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.ItemSize = new System.Drawing.Size(88, 30);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(6);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(820, 767);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.SortByUsernameRadioButton);
            this.tabPage1.Controls.Add(this.SortByTitleRadioButton);
            this.tabPage1.Controls.Add(this.matchSchemesCheckbox);
            this.tabPage1.Controls.Add(this.removePermissionsButton);
            this.tabPage1.Controls.Add(this.unlockDatabaseCheckbox);
            this.tabPage1.Controls.Add(this.removeButton);
            this.tabPage1.Controls.Add(this.credMatchingCheckbox);
            this.tabPage1.Controls.Add(this.credNotifyCheckbox);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(812, 729);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // SortByUsernameRadioButton
            // 
            this.SortByUsernameRadioButton.AutoSize = true;
            this.SortByUsernameRadioButton.Location = new System.Drawing.Point(8, 232);
            this.SortByUsernameRadioButton.Margin = new System.Windows.Forms.Padding(6);
            this.SortByUsernameRadioButton.Name = "SortByUsernameRadioButton";
            this.SortByUsernameRadioButton.Size = new System.Drawing.Size(343, 29);
            this.SortByUsernameRadioButton.TabIndex = 19;
            this.SortByUsernameRadioButton.TabStop = true;
            this.SortByUsernameRadioButton.Text = "Sort found entries by &username";
            this.SortByUsernameRadioButton.UseVisualStyleBackColor = true;
            // 
            // SortByTitleRadioButton
            // 
            this.SortByTitleRadioButton.AutoSize = true;
            this.SortByTitleRadioButton.Location = new System.Drawing.Point(8, 277);
            this.SortByTitleRadioButton.Margin = new System.Windows.Forms.Padding(6);
            this.SortByTitleRadioButton.Name = "SortByTitleRadioButton";
            this.SortByTitleRadioButton.Size = new System.Drawing.Size(282, 29);
            this.SortByTitleRadioButton.TabIndex = 18;
            this.SortByTitleRadioButton.TabStop = true;
            this.SortByTitleRadioButton.Text = "Sort found entries by &title";
            this.SortByTitleRadioButton.UseVisualStyleBackColor = true;
            // 
            // matchSchemesCheckbox
            // 
            this.matchSchemesCheckbox.AutoSize = true;
            this.matchSchemesCheckbox.Location = new System.Drawing.Point(8, 163);
            this.matchSchemesCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.matchSchemesCheckbox.Name = "matchSchemesCheckbox";
            this.matchSchemesCheckbox.Size = new System.Drawing.Size(732, 54);
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
            this.removePermissionsButton.Location = new System.Drawing.Point(22, 409);
            this.removePermissionsButton.Margin = new System.Windows.Forms.Padding(6);
            this.removePermissionsButton.Name = "removePermissionsButton";
            this.removePermissionsButton.Size = new System.Drawing.Size(756, 54);
            this.removePermissionsButton.TabIndex = 16;
            this.removePermissionsButton.Text = "Remo&ve all stored permissions from entries in active database";
            this.removePermissionsButton.UseVisualStyleBackColor = true;
            this.removePermissionsButton.Click += new System.EventHandler(this.removePermissionsButton_Click);
            // 
            // unlockDatabaseCheckbox
            // 
            this.unlockDatabaseCheckbox.AutoSize = true;
            this.unlockDatabaseCheckbox.Location = new System.Drawing.Point(8, 119);
            this.unlockDatabaseCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.unlockDatabaseCheckbox.Name = "unlockDatabaseCheckbox";
            this.unlockDatabaseCheckbox.Size = new System.Drawing.Size(509, 29);
            this.unlockDatabaseCheckbox.TabIndex = 15;
            this.unlockDatabaseCheckbox.Text = "Re&quest for unlocking the database if it is locked";
            this.unlockDatabaseCheckbox.UseVisualStyleBackColor = true;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(22, 344);
            this.removeButton.Margin = new System.Windows.Forms.Padding(6);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(756, 54);
            this.removeButton.TabIndex = 11;
            this.removeButton.Text = "R&emove all shared encryption-keys from active database";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // credMatchingCheckbox
            // 
            this.credMatchingCheckbox.AutoSize = true;
            this.credMatchingCheckbox.Location = new System.Drawing.Point(8, 50);
            this.credMatchingCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.credMatchingCheckbox.Name = "credMatchingCheckbox";
            this.credMatchingCheckbox.Size = new System.Drawing.Size(474, 54);
            this.credMatchingCheckbox.TabIndex = 9;
            this.credMatchingCheckbox.Text = "&Return only best matching entries for an URL\r\ninstead of all entries for the who" +
    "le domain";
            this.credMatchingCheckbox.UseVisualStyleBackColor = true;
            // 
            // credNotifyCheckbox
            // 
            this.credNotifyCheckbox.AutoSize = true;
            this.credNotifyCheckbox.Location = new System.Drawing.Point(8, 6);
            this.credNotifyCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.credNotifyCheckbox.Name = "credNotifyCheckbox";
            this.credNotifyCheckbox.Size = new System.Drawing.Size(532, 29);
            this.credNotifyCheckbox.TabIndex = 8;
            this.credNotifyCheckbox.Text = "Sh&ow a notification when credentials are requested";
            this.credNotifyCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.returnStringFieldsCheckbox);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.credSearchInAllOpenedDatabases);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.credAllowUpdatesCheckbox);
            this.tabPage2.Controls.Add(this.credAllowAccessCheckbox);
            this.tabPage2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(812, 729);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Advanced";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(100, 427);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(516, 52);
            this.label4.TabIndex = 22;
            this.label4.Text = "Automatic creates or updates are not supported\r\nfor string fields!";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(98, 294);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(530, 120);
            this.label3.TabIndex = 21;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // returnStringFieldsCheckbox
            // 
            this.returnStringFieldsCheckbox.AutoSize = true;
            this.returnStringFieldsCheckbox.Location = new System.Drawing.Point(8, 256);
            this.returnStringFieldsCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.returnStringFieldsCheckbox.Name = "returnStringFieldsCheckbox";
            this.returnStringFieldsCheckbox.Size = new System.Drawing.Size(521, 28);
            this.returnStringFieldsCheckbox.TabIndex = 20;
            this.returnStringFieldsCheckbox.Text = "&Return also advanced string fields which start with \"KPH: \"";
            this.returnStringFieldsCheckbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(98, 202);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(521, 24);
            this.label2.TabIndex = 19;
            this.label2.Text = "Only the selected database has to be connected with a client!";
            // 
            // credSearchInAllOpenedDatabases
            // 
            this.credSearchInAllOpenedDatabases.AutoSize = true;
            this.credSearchInAllOpenedDatabases.Location = new System.Drawing.Point(8, 163);
            this.credSearchInAllOpenedDatabases.Margin = new System.Windows.Forms.Padding(6);
            this.credSearchInAllOpenedDatabases.Name = "credSearchInAllOpenedDatabases";
            this.credSearchInAllOpenedDatabases.Size = new System.Drawing.Size(475, 28);
            this.credSearchInAllOpenedDatabases.TabIndex = 18;
            this.credSearchInAllOpenedDatabases.Text = "Searc&h in all opened databases for matching entries";
            this.credSearchInAllOpenedDatabases.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(2, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(723, 26);
            this.label1.TabIndex = 17;
            this.label1.Text = "Activate the following options only, if you know what you are doing!";
            // 
            // credAllowUpdatesCheckbox
            // 
            this.credAllowUpdatesCheckbox.AutoSize = true;
            this.credAllowUpdatesCheckbox.Location = new System.Drawing.Point(6, 102);
            this.credAllowUpdatesCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.credAllowUpdatesCheckbox.Name = "credAllowUpdatesCheckbox";
            this.credAllowUpdatesCheckbox.Size = new System.Drawing.Size(288, 28);
            this.credAllowUpdatesCheckbox.TabIndex = 16;
            this.credAllowUpdatesCheckbox.Text = "Always allow &updating entries";
            this.credAllowUpdatesCheckbox.UseVisualStyleBackColor = true;
            // 
            // credAllowAccessCheckbox
            // 
            this.credAllowAccessCheckbox.AutoSize = true;
            this.credAllowAccessCheckbox.Location = new System.Drawing.Point(6, 57);
            this.credAllowAccessCheckbox.Margin = new System.Windows.Forms.Padding(6);
            this.credAllowAccessCheckbox.Name = "credAllowAccessCheckbox";
            this.credAllowAccessCheckbox.Size = new System.Drawing.Size(294, 28);
            this.credAllowAccessCheckbox.TabIndex = 15;
            this.credAllowAccessCheckbox.Text = "Always allow &access to entries";
            this.credAllowAccessCheckbox.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.instructionsLink);
            this.tabPage3.Controls.Add(this.activateHttpsListenerCheckbox);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 34);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(812, 729);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Listener Configuration";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // instructionsLink
            // 
            this.instructionsLink.AutoSize = true;
            this.instructionsLink.Location = new System.Drawing.Point(9, 98);
            this.instructionsLink.Name = "instructionsLink";
            this.instructionsLink.Size = new System.Drawing.Size(221, 25);
            this.instructionsLink.TabIndex = 47;
            this.instructionsLink.TabStop = true;
            this.instructionsLink.Text = "Read the instructions!";
            this.instructionsLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.instructionsLink_LinkClicked);
            // 
            // activateHttpsListenerCheckbox
            // 
            this.activateHttpsListenerCheckbox.AutoSize = true;
            this.activateHttpsListenerCheckbox.Location = new System.Drawing.Point(14, 333);
            this.activateHttpsListenerCheckbox.Name = "activateHttpsListenerCheckbox";
            this.activateHttpsListenerCheckbox.Size = new System.Drawing.Size(279, 29);
            this.activateHttpsListenerCheckbox.TabIndex = 46;
            this.activateHttpsListenerCheckbox.Text = "Activate HTTPS Listener";
            this.activateHttpsListenerCheckbox.UseVisualStyleBackColor = true;
            this.activateHttpsListenerCheckbox.CheckedChanged += new System.EventHandler(this.activateHttpsListenerCheckbox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.listenerHostHttp);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.portNumberHttp);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(14, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(774, 155);
            this.groupBox2.TabIndex = 45;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "HTTP Listener";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(440, 47);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(178, 25);
            this.label10.TabIndex = 48;
            this.label10.Text = "Default: localhost";
            // 
            // listenerHostHttp
            // 
            this.listenerHostHttp.Location = new System.Drawing.Point(92, 46);
            this.listenerHostHttp.Name = "listenerHostHttp";
            this.listenerHostHttp.Size = new System.Drawing.Size(324, 31);
            this.listenerHostHttp.TabIndex = 47;
            this.listenerHostHttp.Text = "localhost";
            this.listenerHostHttp.TextChanged += new System.EventHandler(this.listenerHost_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 47);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(62, 25);
            this.label8.TabIndex = 46;
            this.label8.Text = "Host:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(440, 95);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(152, 25);
            this.label7.TabIndex = 45;
            this.label7.Text = "Default: 19455";
            // 
            // portNumberHttp
            // 
            this.portNumberHttp.Location = new System.Drawing.Point(93, 97);
            this.portNumberHttp.Margin = new System.Windows.Forms.Padding(6);
            this.portNumberHttp.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.portNumberHttp.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.portNumberHttp.Name = "portNumberHttp";
            this.portNumberHttp.Size = new System.Drawing.Size(120, 31);
            this.portNumberHttp.TabIndex = 44;
            this.portNumberHttp.Value = new decimal(new int[] {
            19455,
            0,
            0,
            0});
            this.portNumberHttp.ValueChanged += new System.EventHandler(this.portNumber_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 99);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 25);
            this.label6.TabIndex = 43;
            this.label6.Text = "Port:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.listenerHostHttps);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.portNumberHttps);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Location = new System.Drawing.Point(14, 368);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(774, 149);
            this.groupBox1.TabIndex = 44;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "HTTPS Listener";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(435, 42);
            this.label12.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(178, 25);
            this.label12.TabIndex = 48;
            this.label12.Text = "Default: localhost";
            // 
            // listenerHostHttps
            // 
            this.listenerHostHttps.Location = new System.Drawing.Point(87, 41);
            this.listenerHostHttps.Name = "listenerHostHttps";
            this.listenerHostHttps.Size = new System.Drawing.Size(324, 31);
            this.listenerHostHttps.TabIndex = 47;
            this.listenerHostHttps.Text = "localhost";
            this.listenerHostHttps.TextChanged += new System.EventHandler(this.listenerHost_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 42);
            this.label13.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(62, 25);
            this.label13.TabIndex = 46;
            this.label13.Text = "Host:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(435, 90);
            this.label14.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(152, 25);
            this.label14.TabIndex = 45;
            this.label14.Text = "Default: 19456";
            // 
            // portNumberHttps
            // 
            this.portNumberHttps.Location = new System.Drawing.Point(88, 92);
            this.portNumberHttps.Margin = new System.Windows.Forms.Padding(6);
            this.portNumberHttps.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.portNumberHttps.Minimum = new decimal(new int[] {
            1025,
            0,
            0,
            0});
            this.portNumberHttps.Name = "portNumberHttps";
            this.portNumberHttps.Size = new System.Drawing.Size(120, 31);
            this.portNumberHttps.TabIndex = 44;
            this.portNumberHttps.Value = new decimal(new int[] {
            19456,
            0,
            0,
            0});
            this.portNumberHttps.ValueChanged += new System.EventHandler(this.portNumber_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(21, 94);
            this.label15.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 25);
            this.label15.TabIndex = 43;
            this.label15.Text = "Port:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(9, 541);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(583, 50);
            this.label9.TabIndex = 41;
            this.label9.Text = "Don\'t forget to change the listener (host + port) also in\r\nthe plugins like chrom" +
    "eIPass, PassIFox, kypass, etc.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 13);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(762, 75);
            this.label5.TabIndex = 36;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(840, 860);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "OptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeePassHttp Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumberHttp)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.portNumberHttps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
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
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox activateHttpsListenerCheckbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox listenerHostHttp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown portNumberHttp;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox listenerHostHttps;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown portNumberHttps;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.LinkLabel instructionsLink;
    }
}