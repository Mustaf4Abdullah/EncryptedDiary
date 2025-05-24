using System;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EncryptedDiary
{    /// <summary>
    /// Modernized version of the MainForm with enhanced UI/UX features
    /// </summary>
    public class ModernizedMainForm : Form, IDiaryForm
    {
        private string username;
        private string password;
        private bool darkMode;
        private int userId;
        private int currentEntryId = -1;
        private readonly string connectionString;
        
        // UI Components
        private RoundedPanel mainPanel = null!;
        private RoundedPanel sidePanel = null!;
        private ModernTextBox searchBox = null!;
        private RichTextBox rtbDiary = null!;
        private DateTimePicker dtpDate = null!;
        private ModernButton btnSave = null!;
        private ModernButton btnNew = null!;
        private ModernButton btnTags = null!;
        private ModernButton btnStatistics = null!;
        private ModernButton btnExport = null!;
        private ModernButton btnLogout = null!;
        private Label lblWordCount = null!;
        private Label lblTitle = null!;
        private Label lblUsername = null!;
        private PictureBox picLogo = null!;
        private Panel topPanel = null!;
        private ComboBox cmbTheme = null!;
        
        // Animation manager for smooth transitions
        private AnimationManager animationManager = new AnimationManager();
        
        public ModernizedMainForm(string username, string password, bool darkMode)
        {
            this.username = username;
            this.password = password;
            this.darkMode = darkMode;
            this.connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;
            
            InitializeComponents();
            InitializeDatabaseConnection();
            ApplyTheme();
            LoadDiary();
        }
        
        private void InitializeComponents()
        {
            // Form properties
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Encrypted Diary";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;            this.MinimizeBox = true;
            this.DoubleBuffered = true;
              // Set the custom icon
            try
            {
                string iconPath = System.IO.Path.Combine(Application.StartupPath, "encryptedDiary_logo.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
                else
                {
                    this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                }
            }
            catch
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            
            try
            {
                // Create top panel for title and user info
                topPanel = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 60,
                    BackColor = Color.Transparent
                };
                
                // Create diary icon
                picLogo = new PictureBox
                {
                    Size = new Size(48, 48),
                    Location = new Point(20, 6),
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
                
                // Create title label
                lblTitle = new Label
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point),
                    Location = new Point(80, 10),
                    Size = new Size(300, 40),
                    Text = "Encrypted Diary",
                    ForeColor = ModernTheme.PrimaryColor,
                    BackColor = Color.Transparent
                };
                
                // Create username label
                lblUsername = new Label
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(800, 20),
                    Size = new Size(160, 25),
                    Text = $"User: {username}",
                    ForeColor = ModernTheme.DarkTextColor,
                    BackColor = Color.Transparent
                };
                
                // Theme selection
                cmbTheme = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(970, 20),
                    Size = new Size(110, 28),
                    TabIndex = 6,
                    FlatStyle = FlatStyle.Flat
                };
                cmbTheme.Items.Add("Light Theme");
                cmbTheme.Items.Add("Dark Theme");
                cmbTheme.SelectedIndex = darkMode ? 1 : 0;
                cmbTheme.SelectedIndexChanged += (sender, e) => 
                {
                    darkMode = cmbTheme.SelectedIndex == 1;
                    ApplyTheme();
                    SaveThemeSettings();
                };
                
                // Create side panel for diary navigation
                sidePanel = new RoundedPanel
                {
                    BorderColor = ModernTheme.PrimaryColor,
                    BorderThickness = 2,
                    CornerRadius = 20,
                    DrawShadow = true,
                    Location = new Point(20, 80),
                    Name = "sidePanel",
                    Size = new Size(250, 580),
                    BackColor = Color.FromArgb(248, 249, 250)
                };
                
                // Create main panel for diary content
                mainPanel = new RoundedPanel
                {
                    BorderColor = ModernTheme.PrimaryColor,
                    BorderThickness = 2,
                    CornerRadius = 20,
                    DrawShadow = true,
                    Location = new Point(290, 80),
                    Name = "mainPanel",
                    Size = new Size(790, 580),
                    BackColor = Color.FromArgb(248, 249, 250)
                };
                
                // Date picker
                dtpDate = new DateTimePicker
                {
                    Location = new Point(20, 20),
                    Size = new Size(210, 27),
                    Font = new Font("Segoe UI", 10F),
                    Format = DateTimePickerFormat.Long,
                    TabIndex = 0
                };
                dtpDate.ValueChanged += DtpDate_ValueChanged;
                
                // Search box
                searchBox = new ModernTextBox
                {
                    BackColor = Color.White,
                    BorderColor = ModernTheme.AccentColor,
                    BorderSize = 2,
                    Location = new Point(20, 70),
                    Padding = new Padding(7),
                    PlaceholderColor = Color.DarkGray,
                    PlaceholderText = "Search diary entries...",
                    Size = new Size(210, 40),
                    TabIndex = 1,
                    Font = new Font("Segoe UI", 10F)
                };
                searchBox.KeyDown += SearchBox_KeyDown;
                
                // New entry button
                btnNew = new ModernButton
                {
                    BackColor = ModernTheme.AccentColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(20, 130),
                    Size = new Size(210, 40),
                    TabIndex = 2,
                    Text = "New Entry",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                };
                btnNew.Click += BtnNew_Click;
                
                // Tags button
                btnTags = new ModernButton
                {
                    BackColor = ModernTheme.PrimaryColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(20, 180),
                    Size = new Size(210, 40),
                    TabIndex = 3,
                    Text = "Manage Tags",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                };
                btnTags.Click += BtnTags_Click;
                
                // Statistics button
                btnStatistics = new ModernButton
                {
                    BackColor = ModernTheme.PrimaryColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(20, 230),
                    Size = new Size(210, 40),
                    TabIndex = 4,
                    Text = "Statistics",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                };
                btnStatistics.Click += BtnStatistics_Click;
                
                // Export button
                btnExport = new ModernButton
                {
                    BackColor = ModernTheme.PrimaryColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(20, 280),
                    Size = new Size(210, 40),
                    TabIndex = 5,
                    Text = "Export",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                };
                btnExport.Click += BtnExport_Click;
                
                // Logout button
                btnLogout = new ModernButton
                {
                    BackColor = Color.FromArgb(192, 57, 43),  // Red color for logout
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(20, 330),
                    Size = new Size(210, 40),
                    TabIndex = 6,
                    Text = "Logout",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                };
                btnLogout.Click += BtnLogout_Click;
                
                // Rich text box for diary content
                rtbDiary = new RichTextBox
                {
                    BorderStyle = BorderStyle.None,
                    Location = new Point(20, 20),
                    Size = new Size(750, 480),
                    TabIndex = 6,
                    Font = new Font("Segoe UI", 11F),
                    ScrollBars = RichTextBoxScrollBars.Vertical,
                    AcceptsTab = true,
                    AutoWordSelection = true,
                    EnableAutoDragDrop = true
                };
                rtbDiary.TextChanged += RtbDiary_TextChanged;
                
                // Save button
                btnSave = new ModernButton
                {
                    BackColor = ModernTheme.PrimaryColor,
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(630, 520),
                    Size = new Size(140, 40),
                    TabIndex = 7,
                    Text = "Save",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold)
                };
                btnSave.Click += BtnSave_Click;
                  // Logout button
                btnLogout = new ModernButton
                {
                    BackColor = Color.FromArgb(192, 57, 43),  // Red color for logout
                    BorderRadius = 20,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    Location = new Point(20, 330),
                    Size = new Size(210, 40),
                    TabIndex = 8,
                    Text = "Logout",
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold)
                };
                btnLogout.Click += BtnLogout_Click;
                  // Word count label
                lblWordCount = new Label
                {
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10F),
                    Location = new Point(20, 520),
                    Size = new Size(210, 40),
                    Text = "Words: 0",
                    ForeColor = ModernTheme.DarkTextColor,
                    BackColor = Color.Transparent
                };
                
                // Add controls to panels
                topPanel.Controls.Add(picLogo);
                topPanel.Controls.Add(lblTitle);
                topPanel.Controls.Add(lblUsername);
                topPanel.Controls.Add(cmbTheme);
                
                sidePanel.Controls.Add(dtpDate);
                sidePanel.Controls.Add(searchBox);
                sidePanel.Controls.Add(btnNew);
                sidePanel.Controls.Add(btnTags);
                sidePanel.Controls.Add(btnStatistics);                sidePanel.Controls.Add(btnExport);
                sidePanel.Controls.Add(btnLogout);
                
                mainPanel.Controls.Add(rtbDiary);
                mainPanel.Controls.Add(btnSave);
                mainPanel.Controls.Add(lblWordCount);
                
                // Add controls to form
                this.Controls.Add(topPanel);
                this.Controls.Add(sidePanel);
                this.Controls.Add(mainPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing UI: {ex.Message}", "UI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
          private void SearchBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;                string searchTerm = searchBox.Text.Trim();
                if (!string.IsNullOrEmpty(searchTerm))                {
                    // Call the search form with proper parent reference
                    var searchForm = new SearchForm(userId, darkMode, this, password);
                    searchForm.ShowDialog();
                }
            }
        }
        
        private void RtbDiary_TextChanged(object? sender, EventArgs e)
        {
            UpdateWordCount();
        }
        
        private void UpdateWordCount()
        {
            string text = rtbDiary.Text.Trim();
            int wordCount = 0;
            
            if (!string.IsNullOrEmpty(text))
            {
                // Split by whitespace and count non-empty words
                string[] words = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                wordCount = words.Length;
            }
            
            lblWordCount.Text = $"Words: {wordCount}";
        }
        
        private void InitializeDatabaseConnection()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Get user ID
                    string query = "SELECT UserId FROM Users WHERE Username = @Username";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        userId = (int)command.ExecuteScalar();
                    }

                    // Create necessary tables if they don't exist
                    EnsureDatabaseSetup(connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void EnsureDatabaseSetup(SqlConnection connection)
        {
            try
            {
                // Create DiaryEntryTags table if it doesn't exist
                string createTagsTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DiaryEntryTags')
                    CREATE TABLE DiaryEntryTags (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        EntryId INT NOT NULL,
                        TagName NVARCHAR(50) NOT NULL,
                        CONSTRAINT FK_DiaryEntryTags_DiaryEntries FOREIGN KEY (EntryId) 
                        REFERENCES DiaryEntries(EntryId) ON DELETE CASCADE
                    )";
                
                using (var command = new SqlCommand(createTagsTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
                
                // Create a word count column in DiaryEntries if it doesn't exist
                try
                {
                    string addWordCountColumnQuery = @"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE name = 'WordCount' AND object_id = OBJECT_ID('DiaryEntries')
                        )
                        ALTER TABLE DiaryEntries ADD WordCount INT NULL";
                    
                    using (var command = new SqlCommand(addWordCountColumnQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    // Ignore errors - the column might already exist or the table might not exist yet
                }
                
                // Create a sentiment column in DiaryEntries if it doesn't exist
                try
                {
                    string addSentimentColumnQuery = @"
                        IF NOT EXISTS (
                            SELECT * FROM sys.columns 
                            WHERE name = 'Sentiment' AND object_id = OBJECT_ID('DiaryEntries')
                        )
                        ALTER TABLE DiaryEntries ADD Sentiment NVARCHAR(20) NULL";
                    
                    using (var command = new SqlCommand(addSentimentColumnQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch 
                {
                    // Ignore errors
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't stop execution
                Console.WriteLine($"Error setting up database: {ex.Message}");
            }
        }
        
        private void LoadDiary()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT EntryId, Content, WordCount, Sentiment
                        FROM DiaryEntries 
                        WHERE UserId = @UserId 
                        AND CONVERT(DATE, EntryDate) = @EntryDate";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@EntryDate", dtpDate.Value.Date);

                        using (var reader = command.ExecuteReader())
                        {                            if (reader.Read())
                            {
                                currentEntryId = reader.GetInt32(0);
                                string encryptedContent = reader.GetString(1);
                                
                                // Decrypt the content before displaying
                                string content = string.IsNullOrEmpty(encryptedContent) ? "" : DecryptString(encryptedContent, password);
                                rtbDiary.Text = content;
                                
                                // Update word count
                                if (!reader.IsDBNull(2))
                                {
                                    int wordCount = reader.GetInt32(2);
                                    string sentiment = !reader.IsDBNull(3) ? reader.GetString(3) : "Neutral";
                                    lblWordCount.Text = $"Words: {wordCount} | Mood: {sentiment}";
                                }
                            }
                            else
                            {
                                currentEntryId = -1;
                                rtbDiary.Clear();
                                lblWordCount.Text = "Words: 0";
                            }
                        }
                    }
                }
                
                // Show confirmation with animation
                ShowSaveConfirmation(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading diary: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
          private async Task SaveDiaryAsync()
        {
            try
            {
                // Calculate word count
                string content = rtbDiary.Text.Trim();
                int wordCount = 0;
                if (!string.IsNullOrEmpty(content))
                {
                    string[] words = content.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    wordCount = words.Length;
                }
                
                // Detect sentiment using ML model
                string sentiment = await DetectSentimentAsync(content);
                
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query;
                    if (currentEntryId > 0)
                    {
                        // Update existing entry
                        query = @"
                            UPDATE DiaryEntries 
                            SET Content = @Content, WordCount = @WordCount, Sentiment = @Sentiment
                            WHERE EntryId = @EntryId";
                    }
                    else
                    {
                        // Insert new entry
                        query = @"
                            INSERT INTO DiaryEntries (UserId, EntryDate, Content, WordCount, Sentiment)
                            VALUES (@UserId, @EntryDate, @Content, @WordCount, @Sentiment);
                            SELECT SCOPE_IDENTITY();";
                    }

                    using (var command = new SqlCommand(query, connection))
                    {
                        if (currentEntryId > 0)
                        {
                            command.Parameters.AddWithValue("@EntryId", currentEntryId);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@UserId", userId);
                            command.Parameters.AddWithValue("@EntryDate", dtpDate.Value);                        }
                        
                        // Encrypt the content before saving to database
                        string encryptedContent = string.IsNullOrEmpty(content) ? "" : EncryptString(content, password);
                        
                        command.Parameters.AddWithValue("@Content", encryptedContent);
                        command.Parameters.AddWithValue("@WordCount", wordCount);
                        command.Parameters.AddWithValue("@Sentiment", sentiment);

                        if (currentEntryId <= 0)
                        {
                            // Get the new entry ID
                            var result = command.ExecuteScalar();
                            currentEntryId = Convert.ToInt32(result);
                        }
                        else
                        {
                            command.ExecuteNonQuery();
                        }
                        
                        // Update word count
                        lblWordCount.Text = $"Words: {wordCount} | Mood: {sentiment}";
                        
                        // Show saved confirmation with animation
                        ShowSaveConfirmation(true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving diary: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ShowSaveConfirmation(bool show)
        {
            if (show)
            {
                // Show a toast notification for a better UX
                ToastNotification.Show(
                    "Entry saved successfully", 
                    ToastNotification.NotificationType.Success,
                    2000,
                    darkMode);
            }
        }
        
        private async Task<string> DetectSentimentAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Neutral";

            try
            {
                using (var client = new HttpClient())
                {
                    var content = new StringContent(
                        JsonConvert.SerializeObject(new { text = text }),
                        Encoding.UTF8, "application/json"
                    );

                    HttpResponseMessage response = await client.PostAsync("http://localhost:5000/predict", content);                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        return result?["emotion"] ?? "Neutral";
                    }
                    else
                    {
                        // Fallback to keyword-based detection if API fails
                        return DetectSentimentFallback(text);
                    }
                }
            }
            catch
            {
                // Fallback to keyword-based detection if HTTP request fails
                return DetectSentimentFallback(text);
            }
        }

        private string DetectSentimentFallback(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "Neutral";
            
            // Simple keyword-based sentiment analysis (can be improved with ML)
            string textLower = text.ToLower();
            
            string[] positiveWords = { "happy", "good", "great", "excellent", "love", "joy", "wonderful", 
                "amazing", "fantastic", "beautiful", "excited", "glad", "mutlu", "güzel", "harika", "süper", "iyi" };
            
            string[] negativeWords = { "sad", "bad", "awful", "terrible", "hate", "angry", "upset", 
                "disappointing", "horrible", "worried", "fear", "kötü", "üzgün", "kızgın", "korku", "endişeli" };
            
            int positiveScore = 0;
            int negativeScore = 0;
            
            foreach (string word in positiveWords)
            {
                if (textLower.Contains(word))
                {
                    positiveScore++;
                }
            }
            
            foreach (string word in negativeWords)
            {
                if (textLower.Contains(word))
                {
                    negativeScore++;
                }
            }
            
            if (positiveScore > negativeScore * 2)
                return "Very Positive";
            else if (positiveScore > negativeScore)
                return "Positive";
            else if (negativeScore > positiveScore * 2)
                return "Very Negative";
            else if (negativeScore > positiveScore)
                return "Negative";
            else
                return "Neutral";
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
                        command.Parameters.AddWithValue("@Username", username);
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
                topPanel.BackColor = Color.FromArgb(30, 36, 40);
                sidePanel.BackColor = ModernTheme.DarkControlBackColor;
                sidePanel.BorderColor = ModernTheme.AccentColor;
                mainPanel.BackColor = ModernTheme.DarkControlBackColor;
                mainPanel.BorderColor = ModernTheme.AccentColor;
                lblTitle.ForeColor = ModernTheme.LightTextColor;
                lblUsername.ForeColor = ModernTheme.LightTextColor;
                lblWordCount.ForeColor = ModernTheme.LightTextColor;
                
                // Dark theme controls
                rtbDiary.BackColor = Color.FromArgb(45, 50, 60);
                rtbDiary.ForeColor = Color.White;
                searchBox.BackColor = Color.FromArgb(45, 50, 60);
                searchBox.ForeColor = Color.White;
                cmbTheme.BackColor = Color.FromArgb(45, 50, 60);
                cmbTheme.ForeColor = Color.White;
                dtpDate.CalendarForeColor = Color.White;
                dtpDate.CalendarMonthBackground = Color.FromArgb(45, 50, 60);
            }
            else
            {
                this.BackColor = ModernTheme.LightBackgroundColor;
                topPanel.BackColor = Color.Transparent;
                sidePanel.BackColor = Color.FromArgb(248, 249, 250);
                sidePanel.BorderColor = ModernTheme.PrimaryColor;
                mainPanel.BackColor = Color.FromArgb(248, 249, 250);
                mainPanel.BorderColor = ModernTheme.PrimaryColor;
                lblTitle.ForeColor = ModernTheme.PrimaryColor;
                lblUsername.ForeColor = ModernTheme.DarkTextColor;
                lblWordCount.ForeColor = ModernTheme.DarkTextColor;
                
                // Light theme controls
                rtbDiary.BackColor = Color.White;
                rtbDiary.ForeColor = Color.Black;
                searchBox.BackColor = Color.White;
                searchBox.ForeColor = Color.Black;
                cmbTheme.BackColor = Color.White;
                cmbTheme.ForeColor = Color.Black;
            }
        }
          // Event handlers
        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            await SaveDiaryAsync();
        }
        
        private void BtnNew_Click(object? sender, EventArgs e)
        {
            rtbDiary.Clear();
            currentEntryId = -1;
            lblWordCount.Text = "Words: 0";
        }
        
        private void BtnTags_Click(object? sender, EventArgs e)
        {
            if (currentEntryId <= 0)
            {
                MessageBox.Show("Please save your diary entry before managing tags.",
                    "Save Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            var tagsForm = new DiaryEntryTagsForm(currentEntryId, darkMode);
            tagsForm.ShowDialog();
        }
        
        private void BtnStatistics_Click(object? sender, EventArgs e)
        {
            ShowStatistics();
        }
        
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            // Show export context menu
            ContextMenuStrip exportMenu = new ContextMenuStrip();
            
            ToolStripMenuItem exportTextItem = new ToolStripMenuItem("Export to Text File");
            exportTextItem.Click += (s, evt) => ExportToTextFile();
            
            ToolStripMenuItem exportHtmlItem = new ToolStripMenuItem("Export to HTML");
            exportHtmlItem.Click += (s, evt) => ExportToHtml();
            
            ToolStripMenuItem exportPdfItem = new ToolStripMenuItem("Export to PDF");
            exportPdfItem.Click += (s, evt) => ExportToPdf();
            
            exportMenu.Items.Add(exportTextItem);
            exportMenu.Items.Add(exportHtmlItem);
            exportMenu.Items.Add(exportPdfItem);
            
            // Apply theme to menu
            if (darkMode)
            {
                exportMenu.BackColor = ModernTheme.DarkControlBackColor;
                exportMenu.ForeColor = Color.White;
                
                foreach (ToolStripItem item in exportMenu.Items)
                {
                    item.BackColor = ModernTheme.DarkControlBackColor;
                    item.ForeColor = Color.White;
                }
            }
            
            // Show the context menu below the export button
            exportMenu.Show(btnExport, new Point(0, btnExport.Height));
        }
        
        private void DtpDate_ValueChanged(object? sender, EventArgs e)
        {
            LoadDiary();
        }
        
        // Utility methods
        private void ShowStatistics()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT 
                            COUNT(*) AS TotalEntries,
                            SUM(WordCount) AS TotalWords,
                            AVG(WordCount) AS AverageWords,
                            MAX(WordCount) AS MaxWords,
                            MIN(CASE WHEN WordCount > 0 THEN WordCount ELSE NULL END) AS MinWords
                        FROM DiaryEntries
                        WHERE UserId = @UserId";
                    
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        using (var reader = command.ExecuteReader())
                        {                            if (reader.Read())
                            {
                                int totalEntries = reader.GetInt32(0);
                                
                                // Handle null values for statistics properly
                                int totalWords = 0;
                                if (!reader.IsDBNull(1)) 
                                {
                                    // SUM can return a larger type, so handle conversion carefully
                                    object sumResult = reader.GetValue(1);
                                    totalWords = Convert.ToInt32(sumResult);
                                }
                                  double avgWords = 0;
                                if (!reader.IsDBNull(2)) 
                                {
                                    object avgResult = reader.GetValue(2);
                                    avgWords = Convert.ToDouble(avgResult);
                                }
                                
                                int maxWords = 0;
                                if (!reader.IsDBNull(3)) 
                                {
                                    object maxResult = reader.GetValue(3);
                                    maxWords = Convert.ToInt32(maxResult);
                                }
                                
                                int minWords = 0;
                                if (!reader.IsDBNull(4)) 
                                {
                                    object minResult = reader.GetValue(4);
                                    minWords = Convert.ToInt32(minResult);
                                }
                                
                                string stats = $"Diary Statistics:\n\n" +
                                              $"Total Entries: {totalEntries}\n" +
                                              $"Total Words Written: {totalWords}\n" +
                                              $"Average Words per Entry: {avgWords:F1}\n" +
                                              $"Longest Entry: {maxWords} words\n" +
                                              $"Shortest Entry: {minWords} words\n";
                                
                                MessageBox.Show(stats, "Diary Statistics", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting statistics: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ExportToTextFile()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = $"Diary_{dtpDate.Value:yyyy-MM-dd}.txt"
                };
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string content = $"Date: {dtpDate.Value:yyyy-MM-dd}\n\n{rtbDiary.Text}";
                    File.WriteAllText(saveFileDialog.FileName, content);
                    
                    ToastNotification.Show(
                        "Diary entry exported to text file", 
                        ToastNotification.NotificationType.Success,
                        3000,
                        darkMode);
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Show(
                    $"Error exporting: {ex.Message}", 
                    ToastNotification.NotificationType.Error,
                    3000,
                    darkMode);
            }
        }
        
        private void ExportToHtml()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "HTML files (*.html)|*.html|All files (*.*)|*.*",
                    FileName = $"Diary_{dtpDate.Value:yyyy-MM-dd}.html"
                };
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
    <title>Diary Entry - {dtpDate.Value:yyyy-MM-dd}</title>
    <style>
        body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; margin: 40px; }}
        .header {{ color: #2980b9; border-bottom: 1px solid #eee; padding-bottom: 10px; }}
        .content {{ margin-top: 20px; }}
        .date {{ color: #7f8c8d; font-size: 0.9em; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Diary Entry</h1>
        <div class='date'>{dtpDate.Value:MMMM d, yyyy}</div>
    </div>
    <div class='content'>
        {rtbDiary.Text.Replace(Environment.NewLine, "<br/>")}
    </div>
</body>
</html>";
                    
                    File.WriteAllText(saveFileDialog.FileName, htmlContent);
                    
                    ToastNotification.Show(
                        "Diary entry exported to HTML", 
                        ToastNotification.NotificationType.Success,
                        3000,
                        darkMode);
                }
            }
            catch (Exception ex)
            {
                ToastNotification.Show(
                    $"Error exporting: {ex.Message}", 
                    ToastNotification.NotificationType.Error,
                    3000,
                    darkMode);
            }
        }
        
        private void ExportToPdf()
        {
            // This is a placeholder - actual PDF export would require a PDF library
            MessageBox.Show("PDF export requires a PDF library. Please install a PDF export library and update this method.",
                "PDF Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Example implementation would use iTextSharp, PdfSharp, or another PDF library
        }
        
        // Allow public access for SearchForm to call this
        public void LoadSpecificDate(DateTime date)
        {
            dtpDate.Value = date;
            LoadDiary();
        }        private async void BtnLogout_Click(object? sender, EventArgs e)
        {
            // Ask for confirmation before logout
            if (MessageBox.Show("Are you sure you want to logout? Any unsaved changes will be lost.", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Save the current diary entry if needed
                await SaveDiaryAsync();

                // Create and show the login form
                var loginForm = new ModernizedLoginForm();
                loginForm.Show();
                
                // Close this form (hide first to avoid flickering)
                this.Hide();
                this.Close();
            }
        }        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Check if we're closing due to logout (in which case we don't need to save)
            if (e.CloseReason == CloseReason.UserClosing && ModifierKeys != Keys.Control)
            {
                // Ask for confirmation
                var result = MessageBox.Show("Are you sure you want to exit?",
                    "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.No)
                {
                    e.Cancel = true;  // Cancel the close event
                }
            }
        }

        // Encryption methods for diary content protection
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
