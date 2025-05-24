using System;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EncryptedDiary
{
    public partial class MainForm : Form, IDiaryForm
    {
        private string username;
        private string password;
        private bool darkMode;
        private int userId;
        private int currentEntryId = -1;
        private readonly string connectionString;

        public MainForm(string username, string password, bool darkMode)
        {
            this.username = username;
            this.password = password;
            this.darkMode = darkMode;
            this.connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;

            InitializeComponent();
            SetupStatusBar();
            InitializeDatabaseConnection();
            SetupAdditionalMenuItems();
            LoadDiary();
            ApplyTheme();
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

        private void SetupStatusBar()
        {
            // Create status label for word count
            var wordCountLabel = new ToolStripStatusLabel
            {
                Name = "lblWordCount",
                BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right,
                BorderStyle = Border3DStyle.Etched
            };
            
            // Create status label for save status
            var saveStatusLabel = new ToolStripStatusLabel
            {
                Name = "lblSaveStatus",
                Spring = true,
                TextAlign = ContentAlignment.MiddleRight
            };
            
            // Create status label for username
            var usernameLabel = new ToolStripStatusLabel
            {
                Name = "lblUsername",
                Text = $"User: {username}",
                BorderSides = ToolStripStatusLabelBorderSides.Left,
                BorderStyle = Border3DStyle.Etched
            };
            
            statusStrip1.Items.AddRange(new ToolStripItem[] { wordCountLabel, saveStatusLabel, usernameLabel });
            
            // Add text changed event to update word count
            rtbDiary.TextChanged += (s, e) => UpdateWordCount();
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
            
            if (statusStrip1.Items["lblWordCount"] is ToolStripStatusLabel wordCountLabel)
            {
                wordCountLabel.Text = $"Words: {wordCount}";
            }
        }        private void SetupAdditionalMenuItems()
        {
            // Add Export submenu
            var exportMenuItem = new ToolStripMenuItem("Export");
            exportMenuItem.DropDownItems.Add("Export to Text File", null, ExportToTextFile_Click);
            exportMenuItem.DropDownItems.Add("Export to HTML", null, ExportToHtml_Click);
            exportMenuItem.DropDownItems.Add("Export to PDF", null, ExportToPdf_Click);
            mnuFile.DropDownItems.Insert(2, exportMenuItem);
            
            // Add Print menu item
            var printMenuItem = new ToolStripMenuItem("Print", null, Print_Click);
            mnuFile.DropDownItems.Insert(3, printMenuItem);
              // Add a separator before Logout
            mnuFile.DropDownItems.Insert(4, new ToolStripSeparator());
            
            // Add Logout menu item
            mnuLogout = new ToolStripMenuItem("Logout", null, MnuLogout_Click);
            mnuFile.DropDownItems.Insert(5, mnuLogout);
            
            // Add Statistics menu item
            var statsMenuItem = new ToolStripMenuItem("Statistics", null, Statistics_Click);
            mnuTools.DropDownItems.Add(statsMenuItem);
            
            // Add Search menu item
            var searchMenuItem = new ToolStripMenuItem("Search", null, Search_Click);
            mnuTools.DropDownItems.Insert(0, searchMenuItem);
            
            // Add Tags menu item
            var tagsMenuItem = new ToolStripMenuItem("Manage Tags", null, ManageTags_Click);
            mnuTools.DropDownItems.Insert(1, tagsMenuItem);
            
            // Add a separator before Change Password
            mnuTools.DropDownItems.Insert(2, new ToolStripSeparator());
            
            // Add a Help menu
            var helpMenu = new ToolStripMenuItem("Help");
            var aboutMenuItem = new ToolStripMenuItem("About", null, About_Click);
            var helpContentsMenuItem = new ToolStripMenuItem("Help Contents", null, HelpContents_Click);
            
            helpMenu.DropDownItems.Add(helpContentsMenuItem);
            helpMenu.DropDownItems.Add(aboutMenuItem);
            
            menuStrip1.Items.Add(helpMenu);
        }
          private void ApplyTheme()
        {
            // Use our ModernTheme utility for consistent styling
            ModernTheme.ApplyTheme(this, darkMode);
            
            // Apply theme to rich text box
            if (darkMode)
            {
                // Set custom background and text colors for better readability in dark mode
                rtbDiary.BackColor = ModernTheme.DarkControlBackColor;
                rtbDiary.ForeColor = ModernTheme.LightTextColor;
                
                // Update status bar items
                foreach (ToolStripItem item in statusStrip1.Items)
                {
                    item.ForeColor = ModernTheme.LightTextColor;
                }
                
                // Update menu items
                ApplyThemeToMenuItems(menuStrip1.Items);
            }
            else
            {
                // Light mode styling
                rtbDiary.BackColor = Color.White;
                rtbDiary.ForeColor = ModernTheme.DarkTextColor;
                rtbDiary.ForeColor = SystemColors.WindowText;
                btnSave.BackColor = SystemColors.Control;
                btnSave.ForeColor = SystemColors.ControlText;
                menuStrip1.BackColor = SystemColors.MenuBar;
                menuStrip1.ForeColor = SystemColors.MenuText;
                statusStrip1.BackColor = SystemColors.Control;
                statusStrip1.ForeColor = SystemColors.ControlText;
                
                // Reset status bar items
                foreach (ToolStripItem item in statusStrip1.Items)
                {
                    item.ForeColor = SystemColors.ControlText;
                }
                
                // Reset menu items
                ApplyThemeToMenuItems(menuStrip1.Items);
            }
        }

        private void ApplyThemeToMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = darkMode ? Color.White : SystemColors.MenuText;
                
                if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
                {
                    foreach (ToolStripItem dropDownItem in menuItem.DropDownItems)
                    {
                        dropDownItem.ForeColor = darkMode ? Color.White : SystemColors.MenuText;
                        
                        // Recursively apply to nested menus
                        if (dropDownItem is ToolStripMenuItem subMenuItem && subMenuItem.HasDropDownItems)
                        {
                            ApplyThemeToMenuItems(subMenuItem.DropDownItems);
                        }
                    }
                }
            }
        }

        // Allow public access for SearchForm to call this
        public void LoadSpecificDate(DateTime date)
        {
            dtpDate.Value = date;
            LoadDiary();
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
                                
                                // Update status bar with sentiment if available
                                if (!reader.IsDBNull(3) && statusStrip1.Items["lblWordCount"] is ToolStripStatusLabel wordCountLabel)
                                {
                                    string sentiment = reader.GetString(3);
                                    wordCountLabel.Text = $"Words: {(!reader.IsDBNull(2) ? reader.GetInt32(2) : 0)} | Mood: {sentiment}";
                                }
                            }
                            else
                            {
                                currentEntryId = -1;
                                rtbDiary.Clear();
                                
                                if (statusStrip1.Items["lblWordCount"] is ToolStripStatusLabel wordCountLabel)
                                {
                                    wordCountLabel.Text = "Words: 0";
                                }
                            }
                        }
                    }
                    
                    // Update status bar with save status
                    if (statusStrip1.Items["lblSaveStatus"] is ToolStripStatusLabel saveLabel)
                    {
                        saveLabel.Text = "Ready";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading diary: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        private async Task SaveDiaryAsync()
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
                        
                        // Update status bar
                        if (statusStrip1.Items["lblSaveStatus"] is ToolStripStatusLabel saveLabel)
                        {
                            saveLabel.Text = $"Saved: {DateTime.Now.ToString("HH:mm:ss")}";
                        }
                        
                        if (statusStrip1.Items["lblWordCount"] is ToolStripStatusLabel wordCountLabel)
                        {
                            wordCountLabel.Text = $"Words: {wordCount} | Mood: {sentiment}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving diary: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private async Task<string> DetectSentimentAsync(string text)
{
    if (string.IsNullOrWhiteSpace(text))
        return "Neutral";

    using (var client = new HttpClient())
    {
        var content = new StringContent(
            JsonConvert.SerializeObject(new { text = text }),
            Encoding.UTF8, "application/json"
        );

        HttpResponseMessage response = await client.PostAsync("http://localhost:5000/predict", content);        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return result?["emotion"] ?? "Neutral";
        }
        else
        {
            return "Error";
        }
    }
}        private async void BtnSave_Click(object sender, EventArgs e)
        {
            await SaveDiaryAsync();
        }

        private void DtpDate_ValueChanged(object sender, EventArgs e)
        {
            rtbDiary.Clear();
            LoadDiary();
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            rtbDiary.Clear();
            // Reset entry ID so a new entry will be created on save
            currentEntryId = -1;
        }

        private void MnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }        private void ManageTags_Click(object? sender, EventArgs e)
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
          private void Search_Click(object? sender, EventArgs e)
        {
            var searchForm = new SearchForm(userId, darkMode, this, password);
            searchForm.ShowDialog();
        }
          private void Statistics_Click(object? sender, EventArgs e)
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
                        {
                            if (reader.Read())
                            {
                                int totalEntries = reader.GetInt32(0);
                                  // Handle null values for statistics
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
                                
                                // Format the statistics message
                                string stats = $"Diary Statistics:\n\n" +
                                              $"Total Entries: {totalEntries}\n" +
                                              $"Total Words Written: {totalWords}\n" +
                                              $"Average Words per Entry: {avgWords:F1}\n" +
                                              $"Longest Entry: {maxWords} words\n" +
                                              $"Shortest Entry: {(minWords > 0 ? minWords + " words" : "N/A")}\n";
                                
                                MessageBox.Show(stats, "Diary Statistics", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No diary entries found.", "Diary Statistics", 
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
          private void ExportToTextFile_Click(object? sender, EventArgs e)
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
                    MessageBox.Show($"Diary entry exported to {saveFileDialog.FileName}",
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to text file: {ex.Message}",
                    "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
          private void ExportToHtml_Click(object? sender, EventArgs e)
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
                    // Convert text to HTML
                    string content = rtbDiary.Text;
                    content = content.Replace(Environment.NewLine, "<br/>");
                    
                    string html = $@"<!DOCTYPE html>
<html>
<head>
    <title>Diary Entry - {dtpDate.Value:yyyy-MM-dd}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        h1 {{ color: #333; }}
        .entry {{ line-height: 1.6; margin-top: 20px; }}
    </style>
</head>
<body>
    <h1>Diary Entry - {dtpDate.Value:yyyy-MM-dd}</h1>
    <div class='entry'>
        {content}
    </div>
    <hr/>
    <footer>Generated by EncryptedDiary on {DateTime.Now}</footer>
</body>
</html>";
                    
                    File.WriteAllText(saveFileDialog.FileName, html);
                    
                    MessageBox.Show($"Diary entry exported to {saveFileDialog.FileName}",
                        "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Ask if the user wants to view the HTML file
                    if (MessageBox.Show("Do you want to open the exported HTML file?",
                            "Open File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = saveFileDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to HTML: {ex.Message}",
                    "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
          private void ExportToPdf_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("To export to PDF, please install a PDF printer like Microsoft Print to PDF, " +
                "then use the Print function and select the PDF printer.",
                "PDF Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void Print_Click(object? sender, EventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // This is a simplified print implementation
                // A more advanced version would use PrintDocument
                MessageBox.Show("Printing functionality will be implemented in a future version.",
                    "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
          private void About_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("EncryptedDiary - A secure personal journal\n\n" +
                "Version 1.1\n" +
                "© " + DateTime.Now.Year + " Your Name\n\n" +
                "Features:\n" +
                "- AES encryption for data security\n" +
                "- Dark/light theme support\n" +
                "- Tag organization\n" +
                "- Search functionality\n" +
                "- Export options\n\n" +
                "Thank you for using EncryptedDiary!",
                "About EncryptedDiary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void HelpContents_Click(object? sender, EventArgs e)
        {
            string helpText = "EncryptedDiary Help\n\n" +
                "Getting Started:\n" +
                "- Create entries by typing in the main text area\n" +
                "- Use the date selector to navigate between dates\n" +
                "- Click 'Save' to store your entry\n\n" +
                
                "Features:\n" +
                "- Search: Find entries containing specific text or by date range\n" +
                "- Tags: Organize entries with keywords\n" +
                "- Export: Save entries as text or HTML files\n" +
                "- Statistics: View writing stats\n" +
                "- Theme: Switch between light and dark modes\n\n" +
                
                "Tips:\n" +
                "- Entries are automatically encrypted for privacy\n" +
                "- Word count is shown in the status bar\n" +
                "- The mood of your entry is automatically detected\n";
            
            MessageBox.Show(helpText, "EncryptedDiary Help", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MnuChangePassword_Click(object sender, EventArgs e)
        {
            var oldPassForm = new Form();
            oldPassForm.Text = "Şifre Değiştir";
            oldPassForm.Size = new Size(300, 200);
            oldPassForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            oldPassForm.StartPosition = FormStartPosition.CenterScreen;

            var lblOld = new Label { Text = "Eski Şifre:", Location = new Point(20, 20), Size = new Size(100, 20) };
            var txtOld = new TextBox { Location = new Point(120, 20), Size = new Size(150, 20), PasswordChar = '*' };

            var lblNew = new Label { Text = "Yeni Şifre:", Location = new Point(20, 50), Size = new Size(100, 20) };
            var txtNew = new TextBox { Location = new Point(120, 50), Size = new Size(150, 20), PasswordChar = '*' };

            var lblConfirm = new Label { Text = "Tekrar Yeni Şifre:", Location = new Point(20, 80), Size = new Size(100, 20) };
            var txtConfirm = new TextBox { Location = new Point(120, 80), Size = new Size(150, 20), PasswordChar = '*' };

            var btnOk = new Button { Text = "Tamam", Location = new Point(120, 120), Size = new Size(70, 30) };
            var btnCancel = new Button { Text = "İptal", Location = new Point(200, 120), Size = new Size(70, 30) };

            btnOk.Click += (s, ev) =>
            {
                if (txtNew.Text != txtConfirm.Text)
                {
                    MessageBox.Show("Yeni şifreler eşleşmiyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtOld.Text == password)
                {
                    try
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            // Update password in database
                            string updateQuery = @"
                                UPDATE Users 
                                SET Password = @NewPassword 
                                WHERE UserId = @UserId";

                            using (var command = new SqlCommand(updateQuery, connection))
                            {
                                command.Parameters.AddWithValue("@UserId", userId);
                                command.Parameters.AddWithValue("@NewPassword",
                                    EncryptString(txtNew.Text, txtNew.Text + username));

                                command.ExecuteNonQuery();
                            }
                        }

                        password = txtNew.Text;
                        oldPassForm.Close();
                        MessageBox.Show("Şifre başarıyla değiştirildi.", "Başarılı",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Şifre değiştirilirken hata: {ex.Message}", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Eski şifre yanlış.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnCancel.Click += (s, ev) => oldPassForm.Close();

            oldPassForm.Controls.AddRange(new Control[] { lblOld, txtOld, lblNew, txtNew, lblConfirm, txtConfirm, btnOk, btnCancel });

            if (darkMode)
            {
                oldPassForm.BackColor = Color.FromArgb(45, 45, 48);
                oldPassForm.ForeColor = Color.White;
                foreach (Control c in oldPassForm.Controls)
                {
                    if (c is TextBox)
                    {
                        c.BackColor = Color.FromArgb(70, 70, 70);
                        c.ForeColor = Color.White;
                    }
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(70, 70, 70);
                        c.ForeColor = Color.White;
                    }
                }
            }

            oldPassForm.ShowDialog();
        }

        private void MnuChangeTheme_Click(object sender, EventArgs e)
        {
            darkMode = !darkMode;
            ApplyTheme();

            // Save theme preference to database
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        IF EXISTS (SELECT 1 FROM UserSettings WHERE UserId = @UserId)
                            UPDATE UserSettings 
                            SET DarkMode = @DarkMode 
                            WHERE UserId = @UserId
                        ELSE
                            INSERT INTO UserSettings (UserId, DarkMode)
                            VALUES (@UserId, @DarkMode)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@DarkMode", darkMode);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        // Encryption methods remain the same
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
        }        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                await SaveDiaryAsync();
                Application.Exit();
            }
        }        private async void MnuLogout_Click(object? sender, EventArgs e)
        {
            // Ask for confirmation before logout
            if (MessageBox.Show("Are you sure you want to logout? Any unsaved changes will be lost.", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Save the current diary entry if needed
                await SaveDiaryAsync();

                // Create and show the login form
                var loginForm = new LoginForm();
                loginForm.Show();
                
                // Close this form (hide first to avoid flickering)
                this.Hide();
                this.Close();
            }
        }
    }
}