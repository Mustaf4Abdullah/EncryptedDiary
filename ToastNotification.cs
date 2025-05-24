using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EncryptedDiary
{
    public class ToastNotification : Form
    {        private AnimationManager animationManager = new AnimationManager();
        private string message;
        private System.Windows.Forms.Timer displayTimer = new System.Windows.Forms.Timer();
        private Color backgroundColor;
        private Color textColor;
        private int cornerRadius = 10;
        private bool isDarkMode;
        private Label messageLabel = null!;
        
        public enum NotificationType
        {
            Success,
            Error,
            Info,
            Warning
        }
        
        // Constructor for single instance use
        private ToastNotification(string message, NotificationType type, int duration = 3000, bool isDarkMode = false)
        {
            this.message = message;
            this.isDarkMode = isDarkMode;
            
            // Set appearance based on notification type
            SetAppearanceByType(type);
            
            InitializeComponent();
            
            // Set timer for auto-close
            displayTimer.Interval = duration;
            displayTimer.Tick += (s, e) => 
            {
                displayTimer.Stop();
                CloseWithAnimation();
            };
        }
        
        private void InitializeComponent()
        {
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            
            // Initialize label
            messageLabel = new Label();
            messageLabel.AutoSize = false;
            messageLabel.TextAlign = ContentAlignment.MiddleCenter;
            messageLabel.Dock = DockStyle.Fill;
            messageLabel.ForeColor = textColor;
            messageLabel.Font = new Font("Segoe UI", 10f, FontStyle.Regular);
            messageLabel.Text = message;
            
            // Add label to form
            this.Controls.Add(messageLabel);
            
            // Calculate the form size based on text
            using (Graphics g = CreateGraphics())
            {
                SizeF textSize = g.MeasureString(message, messageLabel.Font);
                int width = Math.Max((int)textSize.Width + 40, 200);
                int height = Math.Max((int)textSize.Height + 20, 50);
                this.Size = new Size(width, height);
            }
            
            // Enable double buffering for smooth animations
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true);
                
            // Position the form at the bottom-right corner of the screen
            Rectangle workingArea = Screen.GetWorkingArea(this);
            this.Location = new Point(
                workingArea.Right - this.Width - 20,
                workingArea.Bottom - this.Height - 20);
                
            // Set initial opacity to 0 for fade-in animation
            this.Opacity = 0;
            
            // Paint event for custom drawing
            this.Paint += ToastNotification_Paint;
        }
        
        private void ToastNotification_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Create rounded rectangle path
            using (GraphicsPath path = new GraphicsPath())
            {
                Rectangle bounds = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
                int radius = cornerRadius;
                
                // Add arcs for rounded corners
                path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90); // Top-left
                path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90); // Top-right
                path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90); // Bottom-right
                path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90); // Bottom-left
                path.CloseAllFigures();
                
                // Fill background
                using (SolidBrush brush = new SolidBrush(backgroundColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Draw border
                using (Pen pen = new Pen(Color.FromArgb(50, isDarkMode ? Color.White : Color.Black), 1))
                {
                    g.DrawPath(pen, path);
                }
                
                // Set form's region to rounded rectangle
                this.Region = new Region(path);
            }
        }
        
        private void SetAppearanceByType(NotificationType type)
        {
            if (isDarkMode)
            {
                // Dark mode colors
                switch (type)
                {
                    case NotificationType.Success:
                        backgroundColor = Color.FromArgb(39, 174, 96); // Green
                        break;
                    case NotificationType.Error:
                        backgroundColor = Color.FromArgb(192, 57, 43); // Red
                        break;
                    case NotificationType.Info:
                        backgroundColor = Color.FromArgb(41, 128, 185); // Blue
                        break;
                    case NotificationType.Warning:
                        backgroundColor = Color.FromArgb(211, 84, 0); // Orange
                        break;
                }
                textColor = Color.White;
            }
            else
            {
                // Light mode colors
                switch (type)
                {
                    case NotificationType.Success:
                        backgroundColor = Color.FromArgb(46, 204, 113); // Light Green
                        break;
                    case NotificationType.Error:
                        backgroundColor = Color.FromArgb(231, 76, 60); // Light Red
                        break;
                    case NotificationType.Info:
                        backgroundColor = Color.FromArgb(52, 152, 219); // Light Blue
                        break;
                    case NotificationType.Warning:
                        backgroundColor = Color.FromArgb(230, 126, 34); // Light Orange
                        break;
                }
                textColor = Color.White;
            }
        }
        
        // Show notification with fade-in animation
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            animationManager.StartAnimation(0, 1, 500, value => 
            {
                this.Opacity = value;
            }, "fade");
            displayTimer.Start();
        }
        
        // Close with fade-out animation
        private void CloseWithAnimation()
        {
            animationManager.StartAnimation(1, 0, 500, value => 
            {
                this.Opacity = value;
                if (value <= 0)
                {
                    this.Close();
                }
            }, "fade");
        }
        
        // Show toast notification
        public static void Show(string message, NotificationType type = NotificationType.Info, 
            int duration = 3000, bool isDarkMode = false)
        {            // Run on UI thread
            if (Application.OpenForms.Count > 0 && Application.OpenForms[0]?.InvokeRequired == true)
            {
                Application.OpenForms[0]?.BeginInvoke(new Action(() => {
                    ToastNotification toast = new ToastNotification(message, type, duration, isDarkMode);
                    toast.Show();
                }));
            }
            else
            {
                ToastNotification toast = new ToastNotification(message, type, duration, isDarkMode);
                toast.Show();
            }
        }
        
        // Close the notification when clicked
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            CloseWithAnimation();
        }
    }
}
