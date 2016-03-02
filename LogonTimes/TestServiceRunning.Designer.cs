namespace LogonTimes
{
    partial class TestServiceRunning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestServiceRunning));
            this.btnUpdateLogins = new System.Windows.Forms.Button();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.btnChangeSession = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbEventTypes = new System.Windows.Forms.ComboBox();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpdateLogins
            // 
            this.btnUpdateLogins.Location = new System.Drawing.Point(69, 259);
            this.btnUpdateLogins.Name = "btnUpdateLogins";
            this.btnUpdateLogins.Size = new System.Drawing.Size(144, 23);
            this.btnUpdateLogins.TabIndex = 0;
            this.btnUpdateLogins.Text = "Update logins";
            this.btnUpdateLogins.UseVisualStyleBackColor = true;
            this.btnUpdateLogins.Click += new System.EventHandler(this.btnUpdateLogins_Click);
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(106, 106);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(133, 20);
            this.txtUserName.TabIndex = 1;
            this.txtUserName.Text = "Administrator";
            // 
            // btnChangeSession
            // 
            this.btnChangeSession.Location = new System.Drawing.Point(69, 175);
            this.btnChangeSession.Name = "btnChangeSession";
            this.btnChangeSession.Size = new System.Drawing.Size(144, 23);
            this.btnChangeSession.TabIndex = 2;
            this.btnChangeSession.Text = "Change session";
            this.btnChangeSession.UseVisualStyleBackColor = true;
            this.btnChangeSession.Click += new System.EventHandler(this.btnChangeSession_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "User name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Event type";
            // 
            // cmbEventTypes
            // 
            this.cmbEventTypes.FormattingEnabled = true;
            this.cmbEventTypes.Location = new System.Drawing.Point(106, 137);
            this.cmbEventTypes.Name = "cmbEventTypes";
            this.cmbEventTypes.Size = new System.Drawing.Size(133, 21);
            this.cmbEventTypes.TabIndex = 5;
            // 
            // btnConfigure
            // 
            this.btnConfigure.Location = new System.Drawing.Point(69, 27);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Size = new System.Drawing.Size(144, 23);
            this.btnConfigure.TabIndex = 6;
            this.btnConfigure.Text = "Configure";
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // TestServiceRunning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 327);
            this.Controls.Add(this.btnConfigure);
            this.Controls.Add(this.cmbEventTypes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnChangeSession);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.btnUpdateLogins);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestServiceRunning";
            this.Text = "TestServiceRunning";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdateLogins;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Button btnChangeSession;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbEventTypes;
        private System.Windows.Forms.Button btnConfigure;
    }
}