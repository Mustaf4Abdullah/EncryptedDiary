using System;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace EncryptedDiary
{
    public class ModernizedLoginForm : Form
    {
        private bool darkMode = false;
        private readonly string connectionString;
        
    // UI Components
        private RoundedPanel mainPanel = null!;
        private Panel logoPanel = null!;
        private Label lblWelcome = null!;
        private Label lblTitle = null!;
        private ModernTextBox txtUsername = null!;
        private ModernTextBox txtPassword = null!;
        private ModernButton btnLogin = null!;
        private ModernButton btnRegister = null!;
        private CheckBox chkRememberMe = null!;
        private Label lblTheme = null!;
        private ComboBox cmbTheme = null!;
        private PictureBox picLogo = null!;
        
        // Word counter for better UX
        private Label lblStatus = null!;
        
        public ModernizedLoginForm()
        {
            this.connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;
            InitializeComponents();
            LoadThemeSettings();
        }
        
        private void InitializeComponents()
        {
            // Form properties
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Encrypted Diary";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.DoubleBuffered = true;
            
            try
            {
                // Create rounded main panel
                mainPanel = new RoundedPanel
                {
                    BorderColor = ModernTheme.PrimaryColor,
                    BorderThickness = 2,
                    CornerRadius = 20,
                    DrawShadow = true,
                    Location = new Point(250, 120),
                    Name = "mainPanel",
                    Size = new Size(400, 380),
                    BackColor = Color.FromArgb(248, 249, 250)
                };
                
                // Create logo panel
                logoPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 100,
                    BackColor = Color.Transparent
                };
                
                // Create diary icon
                picLogo = new PictureBox
                {
                    Size = new Size(64, 64),
                    Location = new Point(418, 30),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };
                
                // Try to load icon if it exists
                try
                {
                    string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "diary_icon.png");
                    if (System.IO.File.Exists(iconPath))
                    {
                        picLogo.Image = Image.FromFile(iconPath);
                    }
                }
                catch
                {
                    // Icon couldn't be loaded - will show blank
                }
                
                // Create welcome and title labels
                lblWelcome = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(400, 30),
                    Text = "Hoş geldiniz",
                    ForeColor = ModernTheme.DarkTextColor,
                    BackColor = Color.Transparent
                };
                
                lblTitle = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point),
                    Location = new Point(360, 60),
                    Text = "Encrypted Diary",
                    ForeColor = ModernTheme.PrimaryColor,
                    BackColor = Color.Transparent
                };
                
                // Create modern username textbox
                txtUsername = new ModernTextBox
                {
                    BackColor = Color.White,
                    BorderColor = ModernTheme.AccentColor,
                    BorderSize = 2,
                    Location = new Point(50, 80),
                    Padding = new Padding(7),
                    PlaceholderColor = Color.DarkGray,
                    PlaceholderText = "Kullanıcı Adı",
                    Size = new Size(300, 40),
                    TabIndex = 0,
                    UnderlinedStyle = false,
                    Font = new Font("Segoe UI", 12F)
                };
                
                // Create modern password textbox
                txtPassword = new ModernTextBox
                {
                    BackColor = Color.White,
                    BorderColor = ModernTheme.AccentColor,
                    BorderSize = 2,
                    Location = new Point(50, 150),
                    Padding = new Padding(7),
                    PasswordChar = true,
                    PlaceholderColor = Color.DarkGray,
                    PlaceholderText = "Şifre",
                    Size = new Size(300, 40),
                    TabIndex = 1,
                    Font = new Font("Segoe UI", 12F)
                };
                
                // Create modern login button
                btnLogin = new ModernButton
                {
                    BackColor = ModernTheme.PrimaryColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(50, 220),
                    Size = new Size(140, 45),
                    TabIndex = 2,
                    Text = "Giriş Yap",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                };
                btnLogin.Click += BtnLogin_Click;
                
                // Create modern register button
                btnRegister = new ModernButton
                {
                    BackColor = ModernTheme.AccentColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(210, 220),
                    Size = new Size(140, 45),
                    TabIndex = 3,
                    Text = "Kayıt Ol",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold)
                };
                btnRegister.Click += BtnRegister_Click;
                
                // Create modern remember me checkbox
                chkRememberMe = new CheckBox
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(50, 290),
                    Size = new Size(109, 24),
                    TabIndex = 4,
                    Text = "Beni Hatırla",
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                
                // Theme selection
                lblTheme = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(760, 20),
                    Text = "Tema",
                    ForeColor = ModernTheme.PrimaryColor
                };
                
                cmbTheme = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(760, 45),
                    Size = new Size(120, 28),
                    TabIndex = 6,
                    FlatStyle = FlatStyle.Flat
                };
                cmbTheme.Items.Add("Açık Tema");
                cmbTheme.Items.Add("Koyu Tema");
                cmbTheme.SelectedIndex = 0;
                cmbTheme.SelectedIndexChanged += (sender, e) => 
                {
                    darkMode = cmbTheme.SelectedIndex == 1;
                    ApplyTheme();
                    SaveThemeSettings();
                };
                
                // Status label for notifications
                lblStatus = new Label
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9F),
                    Location = new Point(50, 330),
                    Size = new Size(300, 30),
                    BackColor = Color.Transparent
                };
                
                // Add controls to panels
                logoPanel.Controls.Add(lblTitle);
                logoPanel.Controls.Add(lblWelcome);
                logoPanel.Controls.Add(picLogo);
                
                mainPanel.Controls.Add(txtUsername);
                mainPanel.Controls.Add(txtPassword);
                mainPanel.Controls.Add(btnLogin);
                mainPanel.Controls.Add(btnRegister);
                mainPanel.Controls.Add(chkRememberMe);
                mainPanel.Controls.Add(lblStatus);
                
                // Add controls to form
                this.Controls.Add(mainPanel);
                this.Controls.Add(logoPanel);
                this.Controls.Add(lblTheme);
                this.Controls.Add(cmbTheme);
                
                // Apply theme
                ApplyTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing UI: {ex.Message}", "UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }
        
        private void ApplyTheme()
        {
            // Apply form styling
            if (darkMode)
            {
                this.BackColor = ModernTheme.DarkBackgroundColor;
                logoPanel.BackColor = Color.FromArgb(30, 36, 40);
                mainPanel.BackColor = ModernTheme.DarkControlBackColor;
                mainPanel.BorderColor = ModernTheme.AccentColor;
                lblWelcome.ForeColor = ModernTheme.LightTextColor;
                lblTheme.ForeColor = ModernTheme.LightTextColor;
                chkRememberMe.ForeColor = ModernTheme.LightTextColor;
                lblStatus.ForeColor = ModernTheme.LightTextColor;
                
                // Dark theme text boxes
                txtUsername.BackColor = Color.FromArgb(45, 50, 60);
                txtUsername.ForeColor = Color.White;
                txtPassword.BackColor = Color.FromArgb(45, 50, 60);
                txtPassword.ForeColor = Color.White;
                
                // Dark theme combo box
                cmbTheme.BackColor = Color.FromArgb(45, 50, 60);
                cmbTheme.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = ModernTheme.LightBackgroundColor;
                logoPanel.BackColor = Color.Transparent;
                mainPanel.BackColor = Color.FromArgb(248, 249, 250);
                mainPanel.BorderColor = ModernTheme.PrimaryColor;
                lblWelcome.ForeColor = ModernTheme.DarkTextColor;
                lblTheme.ForeColor = ModernTheme.PrimaryColor;
                chkRememberMe.ForeColor = ModernTheme.DarkTextColor;
                lblStatus.ForeColor = ModernTheme.DarkTextColor;
                
                // Light theme text boxes
                txtUsername.BackColor = Color.White;
                txtUsername.ForeColor = Color.Black;
                txtPassword.BackColor = Color.White;
                txtPassword.ForeColor = Color.Black;
                
                // Light theme combo box
                cmbTheme.BackColor = Color.White;
                cmbTheme.ForeColor = Color.Black;
            }
            
            // The title always has the primary color
            lblTitle.ForeColor = ModernTheme.PrimaryColor;
        }
          private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowStatus("Lütfen kullanıcı adı ve şifre giriniz", false);
                return;
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Password FROM Users WHERE Username = @Username";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", txtUsername.Text);
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            string storedPassword = result?.ToString() ?? string.Empty;
                            string hashedPassword = HashPassword(txtPassword.Text);                            if (storedPassword == hashedPassword)
                            {
                                ModernizedMainForm mainForm = new ModernizedMainForm(txtUsername.Text, txtPassword.Text, darkMode);
                                this.Hide();
                                mainForm.Show();
                            }
                            else
                            {
                                ShowStatus("Yanlış şifre girdiniz", false);
                            }
                        }
                        else
                        {
                            ShowStatus("Kullanıcı bulunamadı", false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Giriş hatası: " + ex.Message, false);
            }
        }        private void BtnRegister_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowStatus("Lütfen kullanıcı adı ve şifre giriniz", false);
                return;
            }

            try
            {
                if (RegisterUser(txtUsername.Text, txtPassword.Text))
                {
                    ShowStatus("Kayıt başarılı! Şimdi giriş yapabilirsiniz", true);
                }
                else
                {
                    ShowStatus("Bu kullanıcı adı zaten kullanılıyor", false);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("Kayıt hatası: " + ex.Message, false);
            }
        }

        private bool RegisterUser(string username, string password)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Username", username);
                    int exists = Convert.ToInt32(checkCommand.ExecuteScalar());
                    if (exists > 0) return false;
                }

                string hashedPassword = HashPassword(password);
                string insertQuery = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Username", username);
                    insertCommand.Parameters.AddWithValue("@Password", hashedPassword);
                    insertCommand.ExecuteNonQuery();
                }

                return true;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }
        
        private void ShowStatus(string message, bool success)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = success ? Color.Green : Color.Red;
            // Create toast notification for better UX
            ToastNotification.Show(
                message, 
                success ? ToastNotification.NotificationType.Success : ToastNotification.NotificationType.Error,
                3000,
                darkMode);
        }
    }
}
