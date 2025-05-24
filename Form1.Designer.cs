using System;
using System.Drawing;
using System.Windows.Forms;

namespace EncryptedDiary
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPanel = new EncryptedDiary.RoundedPanel();
            this.chkRememberMe = new System.Windows.Forms.CheckBox();
            this.btnRegister = new EncryptedDiary.ModernButton();
            this.btnLogin = new EncryptedDiary.ModernButton();
            this.txtPassword = new EncryptedDiary.ModernTextBox();
            this.txtUsername = new EncryptedDiary.ModernTextBox();
            this.logoPanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.mainPanel.SuspendLayout();
            this.logoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            mainPanel.BorderColor = Color.FromArgb(41, 128, 185);
            mainPanel.BorderThickness = 2;
            mainPanel.CornerRadius = 15;
            mainPanel.Controls.Add(txtUsername);
            mainPanel.Controls.Add(txtPassword);
            mainPanel.Controls.Add(btnLogin);
            mainPanel.Controls.Add(btnRegister);
            mainPanel.Controls.Add(chkRememberMe);
            mainPanel.DrawShadow = true;
            mainPanel.Location = new Point(191, 110);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(400, 350);
            mainPanel.TabIndex = 7;
            //
            // logoPanel
            //
            logoPanel.Controls.Add(lblTitle);
            logoPanel.Controls.Add(lblWelcome);
            logoPanel.Dock = DockStyle.Top;
            logoPanel.Location = new Point(0, 0);
            logoPanel.Name = "logoPanel";
            logoPanel.Size = new Size(800, 100);
            logoPanel.TabIndex = 8;
            //
            // lblWelcome
            //
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblWelcome.Location = new Point(340, 30);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(120, 28);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Hoş geldiniz";
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(310, 58);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(180, 35);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Encrypted Diary";
            //            // txtUsername
            // 
            txtUsername.BackColor = SystemColors.Window;
            txtUsername.BorderColor = Color.FromArgb(52, 152, 219);
            txtUsername.BorderSize = 2;
            txtUsername.Location = new Point(50, 80);
            txtUsername.Name = "txtUsername";
            txtUsername.Padding = new Padding(7);
            txtUsername.PlaceholderColor = Color.DarkGray;
            txtUsername.PlaceholderText = "Kullanıcı Adı";
            txtUsername.Size = new Size(300, 35);
            txtUsername.TabIndex = 0;
            txtUsername.UnderlinedStyle = false;
            //            // txtPassword
            // 
            txtPassword.BackColor = SystemColors.Window;
            txtPassword.BorderColor = Color.FromArgb(52, 152, 219);
            txtPassword.BorderSize = 2;
            txtPassword.Location = new Point(50, 150);
            txtPassword.Name = "txtPassword";
            txtPassword.Padding = new Padding(7);
            txtPassword.PasswordChar = true;
            txtPassword.PlaceholderColor = Color.DarkGray;
            txtPassword.PlaceholderText = "Şifre";
            txtPassword.Size = new Size(300, 35);
            txtPassword.TabIndex = 1;
                        // btnLogin
            // 
            btnLogin.BackColor = ModernTheme.PrimaryColor;
            btnLogin.BorderRadius = 20;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(50, 220);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(140, 45);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Giriş Yap";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += new EventHandler(BtnLogin_Click);
            //            // btnRegister
            // 
            btnRegister.BackColor = ModernTheme.AccentColor;
            btnRegister.BorderRadius = 20;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(210, 220);
            btnRegister.Name = "btnRegister";
            btnRegister.Size = new Size(140, 45);
            btnRegister.TabIndex = 3;
            btnRegister.Text = "Kayıt Ol";
            btnRegister.UseVisualStyleBackColor = false;
            btnRegister.Click += new EventHandler(BtnRegister_Click);
            //            // chkRememberMe
            // 
            chkRememberMe.AutoSize = true;
            chkRememberMe.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            chkRememberMe.Location = new Point(50, 280);
            chkRememberMe.Name = "chkRememberMe";
            chkRememberMe.Size = new Size(109, 24);
            chkRememberMe.TabIndex = 4;
            chkRememberMe.Text = "Beni Hatırla";
            chkRememberMe.UseVisualStyleBackColor = true;
            //            // lblTheme
            // 
            lblTheme.AutoSize = true;
            lblTheme.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblTheme.Location = new Point(660, 20);
            lblTheme.Name = "lblTheme";
            lblTheme.Size = new Size(91, 20);
            lblTheme.TabIndex = 5;
            lblTheme.Text = "Tema seçimi";
            lblTheme.ForeColor = ModernTheme.PrimaryColor;
            // 
            // cmbTheme
            // 
            cmbTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTheme.FormattingEnabled = true;
            cmbTheme.Location = new Point(660, 45);
            cmbTheme.Name = "cmbTheme";
            cmbTheme.Size = new Size(120, 28);
            cmbTheme.TabIndex = 6;
            cmbTheme.FlatStyle = FlatStyle.Flat;
            //            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 500);
            Controls.Add(mainPanel);
            Controls.Add(logoPanel);
            Controls.Add(lblTheme);
            Controls.Add(cmbTheme);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Encrypted Diary - Giriş";
            
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
            
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private EncryptedDiary.RoundedPanel mainPanel;
        private System.Windows.Forms.Panel logoPanel;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblTitle;
        private EncryptedDiary.ModernTextBox txtUsername;
        private EncryptedDiary.ModernTextBox txtPassword;
        private EncryptedDiary.ModernButton btnLogin;
        private EncryptedDiary.ModernButton btnRegister;
        private System.Windows.Forms.CheckBox chkRememberMe;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
    }
}
