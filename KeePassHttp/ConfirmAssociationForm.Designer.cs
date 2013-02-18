namespace KeePassHttp
{
    partial class ConfirmAssociationForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            this.KeyLabel = new System.Windows.Forms.Label();
            this.KeyName = new System.Windows.Forms.TextBox();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(80, 13);
            label1.TabIndex = 0;
            label1.Text = "Encryption key:";
            // 
            // label2
            // 
            label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            label2.Location = new System.Drawing.Point(12, 78);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(440, 48);
            label2.TabIndex = 1;
            label2.Text = "You have received an association request for the above key. If you would like to " +
    "allow it access to your KeePass database give it a unique name to identify and a" +
    "ccept it.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 37);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(57, 13);
            label3.TabIndex = 3;
            label3.Text = "Key name:";
            // 
            // KeyLabel
            // 
            this.KeyLabel.AutoSize = true;
            this.KeyLabel.Location = new System.Drawing.Point(99, 9);
            this.KeyLabel.Name = "KeyLabel";
            this.KeyLabel.Size = new System.Drawing.Size(87, 13);
            this.KeyLabel.TabIndex = 2;
            this.KeyLabel.Text = "Placeholder Text";
            // 
            // KeyName
            // 
            this.KeyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.KeyName.Location = new System.Drawing.Point(102, 34);
            this.KeyName.Name = "KeyName";
            this.KeyName.Size = new System.Drawing.Size(350, 20);
            this.KeyName.TabIndex = 4;
            // 
            // Save
            // 
            this.Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Save.Location = new System.Drawing.Point(296, 129);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 5;
            this.Save.Text = "&Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.Location = new System.Drawing.Point(377, 129);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 6;
            this.Cancel.Text = "&Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // ConfirmAssociationForm
            // 
            this.AcceptButton = this.Save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 164);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.KeyName);
            this.Controls.Add(label3);
            this.Controls.Add(this.KeyLabel);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfirmAssociationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "KeePassHttp: Confirm New Key Association";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label KeyLabel;
        private System.Windows.Forms.TextBox KeyName;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Cancel;

    }
}