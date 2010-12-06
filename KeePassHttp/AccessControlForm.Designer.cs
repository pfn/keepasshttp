namespace KeePassHttp
{
    partial class AccessControlForm
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
            System.Windows.Forms.Button AllowButton;
            System.Windows.Forms.Button DenyButton;
            this.EntriesBox = new System.Windows.Forms.ListBox();
            this.RememberCheck = new System.Windows.Forms.CheckBox();
            this.ConfirmTextLabel = new System.Windows.Forms.Label();
            AllowButton = new System.Windows.Forms.Button();
            DenyButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AllowButton
            // 
            AllowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            AllowButton.Location = new System.Drawing.Point(176, 207);
            AllowButton.Name = "AllowButton";
            AllowButton.Size = new System.Drawing.Size(75, 23);
            AllowButton.TabIndex = 1;
            AllowButton.Text = "&Allow";
            AllowButton.UseVisualStyleBackColor = true;
            AllowButton.Click += new System.EventHandler(this.AllowButton_Click);
            // 
            // DenyButton
            // 
            DenyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            DenyButton.Location = new System.Drawing.Point(257, 207);
            DenyButton.Name = "DenyButton";
            DenyButton.Size = new System.Drawing.Size(75, 23);
            DenyButton.TabIndex = 2;
            DenyButton.Text = "&Deny";
            DenyButton.UseVisualStyleBackColor = true;
            DenyButton.Click += new System.EventHandler(this.DenyButton_Click);
            // 
            // EntriesBox
            // 
            this.EntriesBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.EntriesBox.FormattingEnabled = true;
            this.EntriesBox.Location = new System.Drawing.Point(12, 12);
            this.EntriesBox.Name = "EntriesBox";
            this.EntriesBox.Size = new System.Drawing.Size(320, 108);
            this.EntriesBox.TabIndex = 0;
            // 
            // RememberCheck
            // 
            this.RememberCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RememberCheck.AutoSize = true;
            this.RememberCheck.Location = new System.Drawing.Point(12, 211);
            this.RememberCheck.Name = "RememberCheck";
            this.RememberCheck.Size = new System.Drawing.Size(138, 17);
            this.RememberCheck.TabIndex = 3;
            this.RememberCheck.Text = "Remember this decision";
            this.RememberCheck.UseVisualStyleBackColor = true;
            // 
            // ConfirmTextLabel
            // 
            this.ConfirmTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ConfirmTextLabel.Location = new System.Drawing.Point(9, 136);
            this.ConfirmTextLabel.Name = "ConfirmTextLabel";
            this.ConfirmTextLabel.Size = new System.Drawing.Size(323, 65);
            this.ConfirmTextLabel.TabIndex = 4;
            this.ConfirmTextLabel.Text = "www.somewhere.com has requested access to passwords for the above item(s). Please" +
                " select whether you want to allow access.";
            // 
            // AccessControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 242);
            this.Controls.Add(this.ConfirmTextLabel);
            this.Controls.Add(this.RememberCheck);
            this.Controls.Add(DenyButton);
            this.Controls.Add(AllowButton);
            this.Controls.Add(this.EntriesBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccessControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeePassHttp: Confirm Access";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox EntriesBox;
        private System.Windows.Forms.CheckBox RememberCheck;
        private System.Windows.Forms.Label ConfirmTextLabel;
    }
}