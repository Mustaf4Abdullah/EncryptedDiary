using System;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace EncryptedDiary
{    public partial class SearchForm : Form
    {        private readonly string connectionString;
        private readonly int userId;
        private readonly bool darkMode;
        private readonly IDiaryForm parentForm;
        private readonly string password;

        public SearchForm(int userId, bool darkMode, IDiaryForm parentForm, string password)
        {
            this.userId = userId;
            this.darkMode = darkMode;
            this.connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;
            this.parentForm = parentForm;
            this.password = password;
            InitializeComponent();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.lblSearchText = new Label();
            this.txtSearchText = new TextBox();
            this.lblStartDate = new Label();
            this.dtpStartDate = new DateTimePicker();
            this.lblEndDate = new Label();
            this.dtpEndDate = new DateTimePicker();
            this.chkIncludeTags = new CheckBox();
            this.lblTag = new Label();
            this.txtTag = new TextBox();
            this.btnSearch = new Button();
            this.lvResults = new ListView();
            this.colDate = new ColumnHeader();
            this.colPreview = new ColumnHeader();
            this.btnOpenEntry = new Button();
            this.SuspendLayout();
            
            // lblSearchText
            this.lblSearchText.AutoSize = true;
            this.lblSearchText.Location = new Point(12, 15);
            this.lblSearchText.Name = "lblSearchText";
            this.lblSearchText.Size = new Size(83, 20);
            this.lblSearchText.TabIndex = 0;
            this.lblSearchText.Text = "Search text:";
            
            // txtSearchText
            this.txtSearchText.Location = new Point(101, 12);
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new Size(387, 27);
            this.txtSearchText.TabIndex = 1;
            
            // lblStartDate
            this.lblStartDate.AutoSize = true;
            this.lblStartDate.Location = new Point(12, 52);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new Size(76, 20);
            this.lblStartDate.TabIndex = 2;
            this.lblStartDate.Text = "Start date:";
            
            // dtpStartDate
            this.dtpStartDate.Format = DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new Point(101, 47);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new Size(125, 27);
            this.dtpStartDate.TabIndex = 3;
            
            // lblEndDate
            this.lblEndDate.AutoSize = true;
            this.lblEndDate.Location = new Point(251, 52);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new Size(70, 20);
            this.lblEndDate.TabIndex = 4;
            this.lblEndDate.Text = "End date:";
            
            // dtpEndDate
            this.dtpEndDate.Format = DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new Point(327, 47);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new Size(125, 27);
            this.dtpEndDate.TabIndex = 5;
            
            // chkIncludeTags
            this.chkIncludeTags.AutoSize = true;
            this.chkIncludeTags.Location = new Point(12, 87);
            this.chkIncludeTags.Name = "chkIncludeTags";
            this.chkIncludeTags.Size = new Size(122, 24);
            this.chkIncludeTags.TabIndex = 6;
            this.chkIncludeTags.Text = "Include tag(s):";
            this.chkIncludeTags.UseVisualStyleBackColor = true;
            
            // lblTag
            this.lblTag.AutoSize = true;
            this.lblTag.Location = new Point(140, 88);
            this.lblTag.Name = "lblTag";
            this.lblTag.Size = new Size(35, 20);
            this.lblTag.TabIndex = 7;
            this.lblTag.Text = "Tag:";
            
            // txtTag
            this.txtTag.Enabled = false;
            this.txtTag.Location = new Point(181, 85);
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new Size(200, 27);
            this.txtTag.TabIndex = 8;
            
            // btnSearch
            this.btnSearch.Location = new Point(389, 83);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new Size(94, 29);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new EventHandler(BtnSearch_Click);
            
            // lvResults
            this.lvResults.Columns.AddRange(new ColumnHeader[] {
                this.colDate,
                this.colPreview
            });
            this.lvResults.FullRowSelect = true;
            this.lvResults.HideSelection = false;
            this.lvResults.Location = new Point(12, 125);
            this.lvResults.MultiSelect = false;
            this.lvResults.Name = "lvResults";
            this.lvResults.Size = new Size(471, 237);
            this.lvResults.TabIndex = 10;
            this.lvResults.UseCompatibleStateImageBehavior = false;
            this.lvResults.View = View.Details;
            
            // colDate
            this.colDate.Text = "Date";
            this.colDate.Width = 100;
            
            // colPreview
            this.colPreview.Text = "Content Preview";
            this.colPreview.Width = 365;
            
            // btnOpenEntry
            this.btnOpenEntry.Location = new Point(364, 368);
            this.btnOpenEntry.Name = "btnOpenEntry";
            this.btnOpenEntry.Size = new Size(119, 29);
            this.btnOpenEntry.TabIndex = 11;
            this.btnOpenEntry.Text = "Open Entry";
            this.btnOpenEntry.UseVisualStyleBackColor = true;
            this.btnOpenEntry.Click += new EventHandler(BtnOpenEntry_Click);
            
            // SearchForm
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(495, 409);
            this.Controls.Add(this.btnOpenEntry);
            this.Controls.Add(this.lvResults);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.lblTag);
            this.Controls.Add(this.chkIncludeTags);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.txtSearchText);
            this.Controls.Add(this.lblSearchText);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Search Diary Entries";
            this.ResumeLayout(false);
            this.PerformLayout();
            
            // Setup event handlers
            this.chkIncludeTags.CheckedChanged += new EventHandler(ChkIncludeTags_CheckedChanged);
            
            // Initialize date values
            dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            dtpEndDate.Value = DateTime.Now;
        }        private void ChkIncludeTags_CheckedChanged(object? sender, EventArgs e)
        {
            txtTag.Enabled = chkIncludeTags.Checked;
        }        private void BtnSearch_Click(object? sender, EventArgs e)
        {
            try
            {
                lvResults.Items.Clear();
                string searchText = txtSearchText.Text.Trim();
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1); // End of day
                string? tagFilter = chkIncludeTags.Checked ? txtTag.Text.Trim() : null;

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // First get all entries within the date range (without content search in SQL)
                    string query = @"
                        SELECT e.EntryId, e.EntryDate, e.Content 
                        FROM DiaryEntries e
                        WHERE e.UserId = @UserId 
                        AND e.EntryDate BETWEEN @StartDate AND @EndDate";

                    // Add tag filter if specified
                    if (chkIncludeTags.Checked && !string.IsNullOrWhiteSpace(tagFilter))
                    {
                        query += @" 
                            AND EXISTS (
                                SELECT 1 FROM DiaryEntryTags t 
                                WHERE t.EntryId = e.EntryId 
                                AND t.TagName = @TagName
                            )";
                    }

                    query += " ORDER BY e.EntryDate DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@StartDate", startDate);
                        command.Parameters.AddWithValue("@EndDate", endDate);

                        if (chkIncludeTags.Checked && !string.IsNullOrWhiteSpace(tagFilter))
                        {
                            command.Parameters.AddWithValue("@TagName", tagFilter);
                        }

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int entryId = reader.GetInt32(0);
                                DateTime entryDate = reader.GetDateTime(1);
                                string encryptedContent = reader.GetString(2);

                                // Decrypt the content using the password
                                string decryptedContent = DecryptString(encryptedContent, password);

                                // If search text is specified, check if decrypted content contains it
                                bool contentMatches = string.IsNullOrWhiteSpace(searchText) || 
                                    decryptedContent.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

                                if (contentMatches)
                                {
                                    // Create a preview (first 50 chars)
                                    string preview = decryptedContent.Length > 50 
                                        ? decryptedContent.Substring(0, 47) + "..." 
                                        : decryptedContent;

                                    var item = new ListViewItem(entryDate.ToShortDateString());
                                    item.SubItems.Add(preview);
                                    item.Tag = entryId; // Store the entry ID for later use
                                    lvResults.Items.Add(item);
                                }
                            }
                        }
                    }
                }

                if (lvResults.Items.Count == 0)
                {
                    MessageBox.Show("No entries found matching your criteria.", 
                        "Search Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }private void BtnOpenEntry_Click(object? sender, EventArgs e)
        {
            if (lvResults.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an entry to open.", 
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            try
            {
                // Get the entry ID from the Tag property
                var selectedItem = lvResults.SelectedItems[0];
                if (selectedItem?.Tag != null && int.TryParse(selectedItem.Tag.ToString(), out int entryId))
                {
                    // Get the date for this entry from the database using null-forgiving operator since connectionString is initialized in constructor
                    using (var connection = new SqlConnection(connectionString!))
                    {
                        connection.Open();

                        string query = "SELECT EntryDate FROM DiaryEntries WHERE EntryId = @EntryId";
                        using (var command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@EntryId", entryId);
                            var result = command.ExecuteScalar();
                            
                            if (result != null && result != DBNull.Value)
                            {
                                DateTime entryDate = (DateTime)result;
                                
                                // Call LoadSpecificDate on the parent form
                                // The parentForm field is marked as readonly and initialized in constructor
                                if (parentForm != null)
                                {
                                    parentForm.LoadSpecificDate(entryDate);
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Cannot open entry: parent form reference is missing.",
                                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Entry not found in the database.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Cannot retrieve entry ID from the selected item.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening entry: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyTheme()
        {
            if (darkMode)
            {
                this.BackColor = Color.FromArgb(45, 45, 48);
                this.ForeColor = Color.White;

                txtSearchText.BackColor = Color.FromArgb(70, 70, 70);
                txtSearchText.ForeColor = Color.White;
                
                txtTag.BackColor = Color.FromArgb(70, 70, 70);
                txtTag.ForeColor = Color.White;
                
                lvResults.BackColor = Color.FromArgb(70, 70, 70);
                lvResults.ForeColor = Color.White;
                
                btnSearch.BackColor = Color.FromArgb(70, 70, 70);
                btnSearch.ForeColor = Color.White;
                
                btnOpenEntry.BackColor = Color.FromArgb(70, 70, 70);
                btnOpenEntry.ForeColor = Color.White;
                
                // DateTimePicker is difficult to theme in dark mode
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
                
                txtSearchText.BackColor = SystemColors.Window;
                txtSearchText.ForeColor = SystemColors.WindowText;
                
                txtTag.BackColor = SystemColors.Window;
                txtTag.ForeColor = SystemColors.WindowText;
                
                lvResults.BackColor = SystemColors.Window;
                lvResults.ForeColor = SystemColors.WindowText;
                
                btnSearch.BackColor = SystemColors.Control;
                btnSearch.ForeColor = SystemColors.ControlText;
                
                btnOpenEntry.BackColor = SystemColors.Control;
                btnOpenEntry.ForeColor = SystemColors.ControlText;
            }
        }
          private Label lblSearchText = null!;
        private TextBox txtSearchText = null!;
        private Label lblStartDate = null!;
        private DateTimePicker dtpStartDate = null!;
        private Label lblEndDate = null!;
        private DateTimePicker dtpEndDate = null!;
        private CheckBox chkIncludeTags = null!;
        private Label lblTag = null!;
        private TextBox txtTag = null!;
        private Button btnSearch = null!;
        private ListView lvResults = null!;
        private ColumnHeader colDate = null!;
        private ColumnHeader colPreview = null!;
        private Button btnOpenEntry = null!;

        // Encryption methods for decrypting diary content during search
        private string DecryptString(string cipherText, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(cipherText))
                    return "";

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
