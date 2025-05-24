using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EncryptedDiary
{
    [DefaultEvent("TextChanged")]
    public class ModernTextBox : UserControl
    {
        // Fields
        private Color _borderColor = Color.MediumSlateBlue;
        private Color _focusedBorderColor = Color.HotPink;
        private int _borderSize = 2;
        private bool _underlinedStyle = false;
        private Color _placeholderColor = Color.DarkGray;
        private string _placeholderText = "";
        private bool _isFocused = false;
        private bool _passwordChar = false;
        
        // Components
        private TextBox textBox;
        private PictureBox iconBox;
        
        // Constructor
        public ModernTextBox()
        {
            InitializeComponent();
        }
        
        // Properties
        [Category("Modern Appearance")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        public Color FocusedBorderColor
        {
            get { return _focusedBorderColor; }
            set
            {
                _focusedBorderColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        public int BorderSize
        {
            get { return _borderSize; }
            set
            {
                _borderSize = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        public bool UnderlinedStyle
        {
            get { return _underlinedStyle; }
            set
            {
                _underlinedStyle = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        public bool PasswordChar
        {
            get { return _passwordChar; }
            set
            {
                _passwordChar = value;
                if (textBox != null)
                    textBox.UseSystemPasswordChar = value;
            }
        }

        [Category("Modern Appearance")]
        public bool Multiline
        {
            get { return textBox.Multiline; }
            set { textBox.Multiline = value; }
        }

        [Category("Modern Appearance")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                textBox.BackColor = value;
            }
        }

        [Category("Modern Appearance")]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set
            {
                base.ForeColor = value;
                textBox.ForeColor = value;
            }
        }

        [Category("Modern Appearance")]
        public override Font Font
        {
            get { return base.Font; }
            set
            {
                base.Font = value;
                textBox.Font = value;
                if (DesignMode)
                    UpdateControlHeight();
            }
        }

        [Category("Modern Appearance")]
        public string Text
        {
            get { return textBox.Text; }
            set { textBox.Text = value; }
        }

        [Category("Modern Appearance")]
        public Color PlaceholderColor
        {
            get { return _placeholderColor; }
            set
            {
                _placeholderColor = value;
                if (isPlaceholder)
                    textBox.ForeColor = value;
            }
        }

        [Category("Modern Appearance")]
        public string PlaceholderText
        {
            get { return _placeholderText; }
            set
            {
                _placeholderText = value;
                textBox.Text = "";
                SetPlaceholder();
            }
        }
        
        [Category("Modern Appearance")]
        public Image Icon
        {
            get { return iconBox.Image; }
            set 
            { 
                iconBox.Image = value;
                iconBox.Visible = (value != null);
                AdjustTextBoxPadding();
            }
        }
        
        // Overridden methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            // Draw border
            using (Pen penBorder = new Pen(_isFocused ? _focusedBorderColor : _borderColor, _borderSize))
            {
                penBorder.Alignment = PenAlignment.Inset;

                if (_underlinedStyle) // Line style
                {
                    g.DrawLine(penBorder, 0, Height - 1, Width, Height - 1);
                }
                else // Normal style
                {
                    g.DrawRectangle(penBorder, 0, 0, Width - 0.5F, Height - 0.5F);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode)
                UpdateControlHeight();
            
            AdjustTextBoxSize();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControlHeight();
        }

        // Private methods
        private void UpdateControlHeight()
        {
            if (!textBox.Multiline)
            {
                int txtHeight = TextRenderer.MeasureText("Text", Font).Height + 2;
                iconBox.Size = new Size(txtHeight, txtHeight);
                iconBox.Location = new Point(2, (Height - iconBox.Height) / 2);
                
                textBox.Multiline = true;
                textBox.MinimumSize = new Size(0, txtHeight);
                textBox.Multiline = false;

                Height = textBox.Height + Padding.Top + Padding.Bottom;
            }
        }
        
        private void AdjustTextBoxSize()
        {
            if (iconBox.Visible)
            {
                int iconWidth = iconBox.Width + 4;
                textBox.Location = new Point(iconWidth, textBox.Location.Y);
                textBox.Width = Width - iconWidth - 4;
            }
            else
            {
                textBox.Location = new Point(4, textBox.Location.Y);
                textBox.Width = Width - 8;
            }
        }
        
        private void AdjustTextBoxPadding()
        {
            if (iconBox.Visible)
            {
                int iconWidth = iconBox.Width + 4;
                textBox.Location = new Point(iconWidth, textBox.Location.Y);
                textBox.Width = Width - iconWidth - 4;
            }
            else
            {
                textBox.Location = new Point(4, textBox.Location.Y);
                textBox.Width = Width - 8;
            }
        }

        private bool isPlaceholder = true;
        
        private void SetPlaceholder()
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) && _placeholderText != "")
            {
                isPlaceholder = true;
                textBox.Text = _placeholderText;
                textBox.ForeColor = _placeholderColor;
                if (_passwordChar)
                    textBox.UseSystemPasswordChar = false;
            }
        }

        private void RemovePlaceholder()
        {
            if (isPlaceholder && _placeholderText != "")
            {
                isPlaceholder = false;
                textBox.Text = "";
                textBox.ForeColor = ForeColor;
                if (_passwordChar)
                    textBox.UseSystemPasswordChar = true;
            }
        }

        // Component initialization
        private void InitializeComponent()
        {
            textBox = new TextBox();
            iconBox = new PictureBox();
            ((ISupportInitialize)iconBox).BeginInit();
            SuspendLayout();
            
            // iconBox
            iconBox.Location = new Point(2, 2);
            iconBox.Name = "iconBox";
            iconBox.Size = new Size(24, 24);
            iconBox.SizeMode = PictureBoxSizeMode.CenterImage;
            iconBox.Visible = false;
            
            // textBox
            textBox.BorderStyle = BorderStyle.None;
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            textBox.Location = new Point(4, 4);
            textBox.Name = "textBox";
            textBox.Size = new Size(216, 20);
            textBox.TabIndex = 0;
            
            // Set events
            textBox.Click += (s, e) => {
                OnClick(e);
            };
            textBox.MouseEnter += (s, e) => {
                OnMouseEnter(e);
            };
            textBox.MouseLeave += (s, e) => {
                OnMouseLeave(e);
            };
            textBox.KeyDown += (s, e) => {
                OnKeyDown(e);
            };
            
            textBox.Enter += (s, e) => {
                _isFocused = true;
                Invalidate();
                RemovePlaceholder();
            };
            
            textBox.Leave += (s, e) => {
                _isFocused = false;
                Invalidate();
                SetPlaceholder();
            };
            
            textBox.TextChanged += (s, e) => {
                OnTextChanged(e);
            };
            
            // Controls
            Controls.Add(textBox);
            Controls.Add(iconBox);
            ((ISupportInitialize)iconBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        
        // Public methods to expose TextBox functionality
        public void Clear()
        {
            textBox.Clear();
        }
        
        public void Focus()
        {
            textBox.Focus();
        }
        
        public void Select(int start, int length)
        {
            textBox.Select(start, length);
        }
        
        public void SelectAll()
        {
            textBox.SelectAll();
        }
    }
}
