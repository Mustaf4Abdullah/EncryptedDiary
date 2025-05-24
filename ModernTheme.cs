using System;
using System.Drawing;
using System.Windows.Forms;

namespace EncryptedDiary
{
    /// <summary>
    /// A utility class to apply modern styling to forms and controls
    /// </summary>
    public static class ModernTheme
    {
        // Modern color palette
        public static Color PrimaryColor = Color.FromArgb(41, 128, 185); // Blue
        public static Color AccentColor = Color.FromArgb(52, 152, 219); // Light blue
        public static Color DarkBackgroundColor = Color.FromArgb(34, 40, 49); // Dark blue-gray
        public static Color LightBackgroundColor = Color.FromArgb(240, 245, 249); // Light gray-blue
        public static Color DarkTextColor = Color.FromArgb(44, 62, 80); // Dark text for light mode
        public static Color LightTextColor = Color.FromArgb(238, 238, 238); // Light text for dark mode
        public static Color DarkControlBackColor = Color.FromArgb(57, 62, 70); // Dark control background
        
        /// <summary>
        /// Applies modern styling to a form based on dark/light mode selection
        /// </summary>
        public static void ApplyTheme(Form form, bool darkMode)
        {
            if (darkMode)
            {
                form.BackColor = DarkBackgroundColor;
                form.ForeColor = LightTextColor;
            }
            else
            {
                form.BackColor = LightBackgroundColor;
                form.ForeColor = DarkTextColor;
            }
            
            // Apply styling to all controls
            foreach (Control control in form.Controls)
            {
                ApplyControlTheme(control, darkMode);
            }
            
            // Change form font to a more modern one
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }
        
        /// <summary>
        /// Applies modern styling to a control based on control type
        /// </summary>
        private static void ApplyControlTheme(Control control, bool darkMode)
        {
            if (control is TextBox textBox)
            {
                SetupTextBox(textBox, darkMode);
            }
            else if (control is Button button)
            {
                SetupButton(button);
            }
            else if (control is Label label)
            {
                SetupLabel(label, darkMode);
            }
            else if (control is CheckBox checkBox)
            {
                SetupCheckBox(checkBox, darkMode);
            }
            else if (control is ComboBox comboBox)
            {
                SetupComboBox(comboBox, darkMode);
            }
            else if (control is RichTextBox richTextBox)
            {
                SetupRichTextBox(richTextBox, darkMode);
            }
            else if (control is DateTimePicker dateTimePicker)
            {
                SetupDateTimePicker(dateTimePicker, darkMode);
            }
            else if (control is MenuStrip menuStrip)
            {
                SetupMenuStrip(menuStrip, darkMode);
            }
            else if (control is StatusStrip statusStrip)
            {
                SetupStatusStrip(statusStrip, darkMode);
            }
            else if (control is Panel panel)
            {
                SetupPanel(panel, darkMode);
                
                // Apply theme to child controls
                foreach (Control childControl in panel.Controls)
                {
                    ApplyControlTheme(childControl, darkMode);
                }
            }
            else if (control is GroupBox groupBox)
            {
                SetupGroupBox(groupBox, darkMode);
                
                // Apply theme to child controls
                foreach (Control childControl in groupBox.Controls)
                {
                    ApplyControlTheme(childControl, darkMode);
                }
            }
            else if (control is ListBox listBox)
            {
                SetupListBox(listBox, darkMode);
            }
        }
        
        private static void SetupTextBox(TextBox textBox, bool darkMode)
        {
            if (darkMode)
            {
                textBox.BackColor = DarkControlBackColor;
                textBox.ForeColor = LightTextColor;
            }
            else
            {
                textBox.BackColor = Color.White;
                textBox.ForeColor = DarkTextColor;
            }
            
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
        
        private static void SetupButton(Button button)
        {
            button.BackColor = PrimaryColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            
            // Add hover effect
            button.MouseEnter += (s, e) => button.BackColor = AccentColor;
            button.MouseLeave += (s, e) => button.BackColor = PrimaryColor;
        }
        
        private static void SetupLabel(Label label, bool darkMode)
        {
            if (darkMode)
            {
                label.ForeColor = LightTextColor;
            }
            else
            {
                label.ForeColor = DarkTextColor;
            }
            
            label.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
        
        private static void SetupCheckBox(CheckBox checkBox, bool darkMode)
        {
            if (darkMode)
            {
                checkBox.ForeColor = LightTextColor;
            }
            else
            {
                checkBox.ForeColor = DarkTextColor;
            }
            
            checkBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
        
        private static void SetupComboBox(ComboBox comboBox, bool darkMode)
        {
            if (darkMode)
            {
                comboBox.BackColor = DarkControlBackColor;
                comboBox.ForeColor = LightTextColor;
            }
            else
            {
                comboBox.BackColor = Color.White;
                comboBox.ForeColor = DarkTextColor;
            }
            
            comboBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            comboBox.FlatStyle = FlatStyle.Flat;
        }
        
        private static void SetupRichTextBox(RichTextBox richTextBox, bool darkMode)
        {
            if (darkMode)
            {
                richTextBox.BackColor = DarkControlBackColor;
                richTextBox.ForeColor = LightTextColor;
            }
            else
            {
                richTextBox.BackColor = Color.White;
                richTextBox.ForeColor = DarkTextColor;
            }
            
            richTextBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            richTextBox.BorderStyle = BorderStyle.FixedSingle;
        }
        
        private static void SetupDateTimePicker(DateTimePicker dateTimePicker, bool darkMode)
        {
            if (darkMode)
            {
                dateTimePicker.CalendarForeColor = LightTextColor;
                dateTimePicker.CalendarMonthBackground = DarkControlBackColor;
            }
            
            dateTimePicker.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
        
        private static void SetupMenuStrip(MenuStrip menuStrip, bool darkMode)
        {
            if (darkMode)
            {
                menuStrip.BackColor = DarkControlBackColor;
                menuStrip.ForeColor = LightTextColor;
            }
            else
            {
                menuStrip.BackColor = Color.White;
                menuStrip.ForeColor = DarkTextColor;
            }
            
            menuStrip.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            menuStrip.Renderer = new ModernToolStripRenderer(darkMode);
        }
        
        private static void SetupStatusStrip(StatusStrip statusStrip, bool darkMode)
        {
            if (darkMode)
            {
                statusStrip.BackColor = DarkControlBackColor;
                statusStrip.ForeColor = LightTextColor;
            }
            else
            {
                statusStrip.BackColor = Color.White;
                statusStrip.ForeColor = DarkTextColor;
            }
            
            statusStrip.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            statusStrip.SizingGrip = false;
        }
        
        private static void SetupPanel(Panel panel, bool darkMode)
        {
            if (darkMode)
            {
                panel.BackColor = DarkBackgroundColor;
                panel.ForeColor = LightTextColor;
            }
            else
            {
                panel.BackColor = LightBackgroundColor;
                panel.ForeColor = DarkTextColor;
            }
            
            panel.BorderStyle = BorderStyle.FixedSingle;
        }
        
        private static void SetupGroupBox(GroupBox groupBox, bool darkMode)
        {
            if (darkMode)
            {
                groupBox.BackColor = DarkBackgroundColor;
                groupBox.ForeColor = LightTextColor;
            }
            else
            {
                groupBox.BackColor = LightBackgroundColor;
                groupBox.ForeColor = DarkTextColor;
            }
            
            groupBox.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        }
        
        private static void SetupListBox(ListBox listBox, bool darkMode)
        {
            if (darkMode)
            {
                listBox.BackColor = DarkControlBackColor;
                listBox.ForeColor = LightTextColor;
            }
            else
            {
                listBox.BackColor = Color.White;
                listBox.ForeColor = DarkTextColor;
            }
            
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }
    }
    
    /// <summary>
    /// Custom renderer for MenuStrip to match the modern theme
    /// </summary>
    public class ModernToolStripRenderer : ToolStripProfessionalRenderer
    {
        private bool darkMode;
        
        public ModernToolStripRenderer(bool darkMode)
        {
            this.darkMode = darkMode;
        }
        
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
            
            Color backColor = e.Item.Selected 
                ? ModernTheme.AccentColor 
                : (darkMode ? ModernTheme.DarkControlBackColor : Color.White);
                
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, rect);
            }
        }
    }
}
