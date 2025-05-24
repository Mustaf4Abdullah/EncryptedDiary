#pragma warning disable CS0618 // Suppress "Type or member is obsolete" warnings
using System;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace EncryptedDiary
{
    public partial class LoginForm : Form
    {
        private bool darkMode = false;
        private readonly string connectionString;


        public LoginForm()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;
            InitializeComponent();
            
            // Initialize theme combo box items
            cmbTheme.Items.Add("Açık Tema");
            cmbTheme.Items.Add("Koyu Tema");
            cmbTheme.SelectedIndex = 0; // Default to light theme
            
            LoadThemeSettings();
            
            // Initialize modern UI components
            InitializeModernUI();
        }
          private void InitializeModernUI()
        {
            try
            {
                // Create rounded main panel
                RoundedPanel mainPanel = new RoundedPanel
                {
                    BorderColor = ModernTheme.PrimaryColor,
                    BorderThickness = 2,
                    CornerRadius = 15,
                    DrawShadow = true,
                    Location = new Point(191, 110),
                    Name = "mainPanel",
                    Size = new Size(400, 350)
                };
                
                // Create logo panel
                Panel logoPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Location = new Point(0, 0),
                    Name = "logoPanel",
                    Size = new Size(800, 100)
                };
                
                // Create welcome and title labels
                Label lblWelcome = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(340, 30),
                    Name = "lblWelcome",
                    Size = new Size(120, 28),
                    Text = "Hoş geldiniz"
                };
                
                Label lblTitle = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 15F, FontStyle.Bold, GraphicsUnit.Point),
                    Location = new Point(310, 58),
                    Name = "lblTitle",
                    Size = new Size(180, 35),
                    Text = "Encrypted Diary"
                };
                
                // Create modern username textbox
                ModernTextBox modernUsername = new ModernTextBox
                {
                    BackColor = SystemColors.Window,
                    BorderColor = ModernTheme.AccentColor,
                    BorderSize = 2,
                    Location = new Point(50, 80),
                    Name = "modernUsername",
                    Padding = new Padding(7),
                    PlaceholderColor = Color.DarkGray,
                    PlaceholderText = "Kullanıcı Adı",
                    Size = new Size(300, 35),
                    TabIndex = 0,
                    UnderlinedStyle = false
                };                modernUsername.TextChanged += (sender, e) => txtUsername.Text = modernUsername.Text;
                
                // Create modern password textbox
                ModernTextBox modernPassword = new ModernTextBox
                {
                    BackColor = SystemColors.Window,
                    BorderColor = ModernTheme.AccentColor,
                    BorderSize = 2,
                    Location = new Point(50, 150),
                    Name = "modernPassword",
                    Padding = new Padding(7),
                    PasswordChar = true,
                    PlaceholderColor = Color.DarkGray,
                    PlaceholderText = "Şifre",
                    Size = new Size(300, 35),
                    TabIndex = 1
                };
                modernPassword.TextChanged += (sender, e) => txtPassword.Text = modernPassword.Text;
                
                // Create modern login button
                ModernButton modernLoginBtn = new ModernButton
                {
                    BackColor = ModernTheme.PrimaryColor,
                    BorderRadius = 15,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(50, 220),
                    Name = "modernLoginBtn",
                    Size = new Size(140, 45),
                    TabIndex = 2,
                    Text = "Giriş Yap",
                    UseVisualStyleBackColor = false
                };                modernLoginBtn.Click += (sender, e) => BtnLogin_Click(modernLoginBtn, e);
                
                // Create modern register button
                ModernButton modernRegisterBtn = new ModernButton
                {
                    BackColor = ModernTheme.AccentColor,
                    BorderRadius = 15,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(210, 220),
                    Name = "modernRegisterBtn",
                    Size = new Size(140, 45),
                    TabIndex = 3,
                    Text = "Kayıt Ol",
                    UseVisualStyleBackColor = false
                };
                modernRegisterBtn.Click += (sender, e) => BtnRegister_Click(modernRegisterBtn, e);
                
                // Create modern remember me checkbox
                CheckBox modernRememberMe = new CheckBox
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(50, 280),
                    Name = "modernRememberMe",
                    Size = new Size(109, 24),
                    TabIndex = 4,
                    Text = "Beni Hatırla"
                };
                modernRememberMe.CheckedChanged += (sender, e) => chkRememberMe.Checked = modernRememberMe.Checked;

                // Style theme selection area
                lblTheme.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                lblTheme.ForeColor = ModernTheme.PrimaryColor;
                cmbTheme.SelectedIndexChanged += (sender, e) =>
                {
                    darkMode = cmbTheme.SelectedIndex == 1;
                    ApplyTheme();
                    SaveThemeSettings();
                };
                
                // Add controls to panels
                logoPanel.Controls.Add(lblTitle);
                logoPanel.Controls.Add(lblWelcome);
                
                mainPanel.Controls.Add(modernUsername);
                mainPanel.Controls.Add(modernPassword);
                mainPanel.Controls.Add(modernLoginBtn);
                mainPanel.Controls.Add(modernRegisterBtn);
                mainPanel.Controls.Add(modernRememberMe);
                
                // Clear and add controls to form
                Controls.Clear();
                Controls.Add(mainPanel);
                Controls.Add(logoPanel);
                Controls.Add(lblTheme);
                Controls.Add(cmbTheme);
                
                // Re-add original controls but hide them (maintaining their functionality)
                Controls.Add(txtUsername);
                Controls.Add(txtPassword);
                Controls.Add(btnLogin);
                Controls.Add(btnRegister);
                Controls.Add(chkRememberMe);
                
                txtUsername.Visible = false;
                txtPassword.Visible = false;
                btnLogin.Visible = false;
                btnRegister.Visible = false;
                chkRememberMe.Visible = false;
                
                // Apply theme
                ApplyTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Modern UI initialization failed: " + ex.Message);
            }
        }

        private void LoadThemeSettings()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT DarkMode FROM UserSettings WHERE Username = @Username";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", "");
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            darkMode = (bool)result;
                            cmbTheme.SelectedIndex = darkMode ? 1 : 0;
                            ApplyTheme();
                        }
                    }
                }
            }
            catch
            {
                // If table doesn't exist or other error, use default theme
            }
        }

        private void SaveThemeSettings()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        IF EXISTS (SELECT 1 FROM UserSettings WHERE Username = @Username)
                            UPDATE UserSettings SET DarkMode = @DarkMode WHERE Username = @Username
                        ELSE
                            INSERT INTO UserSettings (Username, DarkMode) VALUES (@Username, @DarkMode)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", "");
                        command.Parameters.AddWithValue("@DarkMode", darkMode);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }        private void ApplyTheme()
        {
            // Use our ModernTheme utility class to apply consistent styling
            ModernTheme.ApplyTheme(this, darkMode);
            
            // Add any form-specific styling here
            if (this.Controls.Find("lblTitle", true).Length > 0)
            {
                Label lblTitle = (Label)this.Controls.Find("lblTitle", true)[0];
                lblTitle.ForeColor = ModernTheme.PrimaryColor;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Kullanıcı adı ve şifre giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ValidateUser(txtUsername.Text, txtPassword.Text))
            {
                darkMode = cmbTheme.SelectedIndex == 1;
                SaveThemeSettings();

                var mainForm = new MainForm(txtUsername.Text, txtPassword.Text, darkMode);
                mainForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Geçersiz kullanıcı adı veya şifre.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Kullanıcı adı ve şifre giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (RegisterUser(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show("Kayıt başarılı. Giriş yapabilirsiniz.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Bu kullanıcı adı zaten alınmış.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateUser(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Password FROM Users WHERE Username = @Username";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        var storedPassword = command.ExecuteScalar()?.ToString();

                        if (storedPassword != null)
                        {
                            return DecryptString(storedPassword, password + username) == password;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Giriş sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private bool RegisterUser(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if user exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Username", username);
                        int exists = (int)checkCommand.ExecuteScalar();
                        if (exists > 0) return false;
                    }

                    // Insert new user
                    string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Username", username);
                        insertCommand.Parameters.AddWithValue("@Password", EncryptString(password, password + username));
                        insertCommand.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string EncryptString(string text, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetKey(key);
                aesAlg.IV = new byte[16];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        private string DecryptString(string cipherText, string key)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = GetKey(key);
                    aesAlg.IV = new byte[16];

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private byte[] GetKey(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }
    }
}