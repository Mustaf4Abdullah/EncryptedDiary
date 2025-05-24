using System;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;

namespace EncryptedDiary
{
    public partial class DiaryEntryTagsForm : Form
    {
        private int entryId;
        private readonly string connectionString;
        private bool darkMode;

        public DiaryEntryTagsForm(int entryId, bool darkMode)
        {
            this.entryId = entryId;
            this.darkMode = darkMode;
            this.connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;
            InitializeComponent();
            LoadTags();
            ApplyTheme();
        }

        private void InitializeComponent()
        {
            this.lstTags = new ListBox();
            this.txtNewTag = new TextBox();
            this.btnAddTag = new Button();
            this.btnRemoveTag = new Button();
            this.btnDone = new Button();
            this.SuspendLayout();
            // 
            // lstTags
            // 
            this.lstTags.FormattingEnabled = true;
            this.lstTags.ItemHeight = 20;
            this.lstTags.Location = new Point(12, 12);
            this.lstTags.Name = "lstTags";
            this.lstTags.Size = new Size(260, 204);
            this.lstTags.TabIndex = 0;
            // 
            // txtNewTag
            // 
            this.txtNewTag.Location = new Point(12, 232);
            this.txtNewTag.Name = "txtNewTag";
            this.txtNewTag.Size = new Size(175, 27);
            this.txtNewTag.TabIndex = 1;
            // 
            // btnAddTag
            // 
            this.btnAddTag.Location = new Point(193, 230);
            this.btnAddTag.Name = "btnAddTag";
            this.btnAddTag.Size = new Size(79, 29);
            this.btnAddTag.TabIndex = 2;
            this.btnAddTag.Text = "Add";
            this.btnAddTag.UseVisualStyleBackColor = true;
            this.btnAddTag.Click += new EventHandler(BtnAddTag_Click);
            // 
            // btnRemoveTag
            // 
            this.btnRemoveTag.Location = new Point(12, 275);
            this.btnRemoveTag.Name = "btnRemoveTag";
            this.btnRemoveTag.Size = new Size(129, 29);
            this.btnRemoveTag.TabIndex = 3;
            this.btnRemoveTag.Text = "Remove Selected";
            this.btnRemoveTag.UseVisualStyleBackColor = true;
            this.btnRemoveTag.Click += new EventHandler(BtnRemoveTag_Click);
            // 
            // btnDone
            // 
            this.btnDone.Location = new Point(193, 275);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new Size(79, 29);
            this.btnDone.TabIndex = 4;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new EventHandler(BtnDone_Click);
            // 
            // DiaryEntryTagsForm
            // 
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(284, 320);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.btnRemoveTag);
            this.Controls.Add(this.btnAddTag);
            this.Controls.Add(this.txtNewTag);
            this.Controls.Add(this.lstTags);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiaryEntryTagsForm";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Manage Tags";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void LoadTags()
        {
            try
            {
                lstTags.Items.Clear();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT TagName 
                        FROM DiaryEntryTags 
                        WHERE EntryId = @EntryId 
                        ORDER BY TagName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EntryId", entryId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lstTags.Items.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tags: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddTag_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewTag.Text))
                return;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO DiaryEntryTags (EntryId, TagName)
                        VALUES (@EntryId, @TagName)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EntryId", entryId);
                        command.Parameters.AddWithValue("@TagName", txtNewTag.Text.Trim());
                        command.ExecuteNonQuery();
                    }
                }

                lstTags.Items.Add(txtNewTag.Text.Trim());
                txtNewTag.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding tag: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRemoveTag_Click(object sender, EventArgs e)
        {
            if (lstTags.SelectedIndex == -1)
                return;

            try
            {
                string tagName = lstTags.SelectedItem.ToString();
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        DELETE FROM DiaryEntryTags 
                        WHERE EntryId = @EntryId AND TagName = @TagName";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EntryId", entryId);
                        command.Parameters.AddWithValue("@TagName", tagName);
                        command.ExecuteNonQuery();
                    }
                }

                lstTags.Items.RemoveAt(lstTags.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing tag: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyTheme()
        {
            if (darkMode)
            {
                this.BackColor = Color.FromArgb(45, 45, 48);
                this.ForeColor = Color.White;
                
                lstTags.BackColor = Color.FromArgb(70, 70, 70);
                lstTags.ForeColor = Color.White;
                
                txtNewTag.BackColor = Color.FromArgb(70, 70, 70);
                txtNewTag.ForeColor = Color.White;
                
                btnAddTag.BackColor = Color.FromArgb(70, 70, 70);
                btnAddTag.ForeColor = Color.White;
                
                btnRemoveTag.BackColor = Color.FromArgb(70, 70, 70);
                btnRemoveTag.ForeColor = Color.White;
                
                btnDone.BackColor = Color.FromArgb(70, 70, 70);
                btnDone.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = SystemColors.ControlText;
                
                lstTags.BackColor = SystemColors.Window;
                lstTags.ForeColor = SystemColors.WindowText;
                
                txtNewTag.BackColor = SystemColors.Window;
                txtNewTag.ForeColor = SystemColors.WindowText;
                
                btnAddTag.BackColor = SystemColors.Control;
                btnAddTag.ForeColor = SystemColors.ControlText;
                
                btnRemoveTag.BackColor = SystemColors.Control;
                btnRemoveTag.ForeColor = SystemColors.ControlText;
                
                btnDone.BackColor = SystemColors.Control;
                btnDone.ForeColor = SystemColors.ControlText;
            }
        }

        private ListBox lstTags;
        private TextBox txtNewTag;
        private Button btnAddTag;
        private Button btnRemoveTag;
        private Button btnDone;
    }
}
