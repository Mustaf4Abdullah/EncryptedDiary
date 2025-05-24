namespace EncryptedDiary
{
    partial class MainForm
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
            menuStrip1 = new MenuStrip();
            mnuFile = new ToolStripMenuItem();
            mnuNew = new ToolStripMenuItem();
            mnuSave = new ToolStripMenuItem();
            mnuLogout = new ToolStripMenuItem();
            mnuExit = new ToolStripMenuItem();
            mnuTools = new ToolStripMenuItem();
            mnuChangePassword = new ToolStripMenuItem();
            mnuChangeTheme = new ToolStripMenuItem();
            rtbDiary = new RichTextBox();
            dtpDate = new DateTimePicker();
            btnSave = new Button();
            statusStrip1 = new StatusStrip();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { mnuFile, mnuTools });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuNew, mnuSave, mnuLogout, mnuExit });
            mnuFile.Name = "mnuFile";
            mnuFile.Size = new Size(46, 24);
            mnuFile.Text = "File";
            //            // 
            // mnuNew
            // 
            mnuNew.Name = "mnuNew";
            mnuNew.Size = new Size(123, 26);
            mnuNew.Text = "New";
            mnuNew.Click += new EventHandler(MnuNew_Click);
            // 
            // mnuSave
            // 
            mnuSave.Name = "mnuSave";
            mnuSave.Size = new Size(123, 26);
            mnuSave.Text = "Save";
            mnuSave.Click += new EventHandler(BtnSave_Click);
            //
            // mnuLogout
            //
            mnuLogout.Name = "mnuLogout";
            mnuLogout.Size = new Size(123, 26);
            mnuLogout.Text = "Logout";
            mnuLogout.Click += new EventHandler(MnuLogout_Click);
            // 
            // mnuExit
            // 
            mnuExit.Name = "mnuExit";
            mnuExit.Size = new Size(123, 26);
            mnuExit.Text = "Exit";
            mnuExit.Click += new EventHandler(MnuExit_Click);
            // 
            // mnuTools
            // 
            mnuTools.DropDownItems.AddRange(new ToolStripItem[] { mnuChangePassword, mnuChangeTheme });
            mnuTools.Name = "mnuTools";
            mnuTools.Size = new Size(58, 24);
            mnuTools.Text = "Tools";
            //            // mnuChangePassword
            // 
            mnuChangePassword.Name = "mnuChangePassword";
            mnuChangePassword.Size = new Size(207, 26);
            mnuChangePassword.Text = "Change Password";
            mnuChangePassword.Click += new EventHandler(MnuChangePassword_Click);
            //            // mnuChangeTheme
            // 
            mnuChangeTheme.Name = "mnuChangeTheme";
            mnuChangeTheme.Size = new Size(207, 26);
            mnuChangeTheme.Text = "Change Theme";
            mnuChangeTheme.Click += new EventHandler(MnuChangeTheme_Click);
            // 
            // rtbDiary
            // 
            rtbDiary.Location = new Point(12, 31);
            rtbDiary.Name = "rtbDiary";
            rtbDiary.Size = new Size(309, 394);
            rtbDiary.TabIndex = 1;
            rtbDiary.Text = "";
            //            // dtpDate
            // 
            dtpDate.Location = new Point(423, 12);
            dtpDate.Name = "dtpDate";
            dtpDate.Size = new Size(250, 27);
            dtpDate.TabIndex = 2;
            dtpDate.ValueChanged += new EventHandler(DtpDate_ValueChanged);
            // // btnSave
            // 
            btnSave.Location = new Point(627, 372);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(94, 29);
            btnSave.TabIndex = 3;
            btnSave.Text = "Kaydet";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += new EventHandler(BtnSave_Click);
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Location = new Point(0, 426);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 24);
            statusStrip1.TabIndex = 4;
            statusStrip1.Text = "Durum çubuğu";
            //            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip1);
            Controls.Add(btnSave);
            Controls.Add(dtpDate);
            Controls.Add(rtbDiary);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "Ana Ekran";
            
            // Set the custom icon
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "encryptedDiary_logo.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch
            {
                // If icon loading fails, continue without custom icon
            }
            
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

                private MenuStrip menuStrip1;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuSave;
        private ToolStripMenuItem mnuExit;
        private ToolStripMenuItem mnuNew;
        private ToolStripMenuItem mnuTools;
        private ToolStripMenuItem mnuChangePassword;
        private ToolStripMenuItem mnuChangeTheme;
        private RichTextBox rtbDiary;
        private DateTimePicker dtpDate;
        private Button btnSave;
        private StatusStrip statusStrip1;
        private ToolStripMenuItem mnuLogout;
    }
}