using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EncryptedDiary
{
    public class ModernButton : Button
    {
        // Fields
        private int _borderSize = 0;
        private int _borderRadius = 10;
        private Color _borderColor = Color.PaleVioletRed;
        private Color _hoverColor = Color.FromArgb(52, 152, 219); // Light blue
        private Color _pressedColor = Color.FromArgb(41, 128, 185); // Darker blue
        private bool _isHovering = false;
        private bool _isPressed = false;

        // Properties
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
        public int BorderRadius
        {
            get { return _borderRadius; }
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

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
        public Color HoverColor
        {
            get { return _hoverColor; }
            set
            {
                _hoverColor = value;
                Invalidate();
            }
        }

        [Category("Modern Appearance")]
        public Color PressedColor
        {
            get { return _pressedColor; }
            set
            {
                _pressedColor = value;
                Invalidate();
            }
        }
        
        // Constructor
        public ModernButton()
        {
            Size = new Size(150, 40);
            FlatAppearance.BorderSize = 0;
            FlatStyle = FlatStyle.Flat;
            BackColor = ModernTheme.PrimaryColor;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Cursor = Cursors.Hand;
            
            // Add event handlers for hover and press effects
            MouseEnter += (s, e) => {
                _isHovering = true;
                Invalidate();
            };
            
            MouseLeave += (s, e) => {
                _isHovering = false;
                _isPressed = false;
                Invalidate();
            };
            
            MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Left)
                {
                    _isPressed = true;
                    Invalidate();
                }
            };
            
            MouseUp += (s, e) => {
                _isPressed = false;
                Invalidate();
            };
        }

        // Methods
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            
            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -BorderSize, -BorderSize);
            
            int smoothSize = 2;
            if (BorderSize > 0)
                smoothSize = BorderSize;

            if (BorderRadius > 2) // Rounded button
            {
                using (GraphicsPath pathSurface = GetFigurePath(rectSurface, BorderRadius))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder, BorderRadius - BorderSize))
                using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(BorderColor, BorderSize))
                {
                    // Draw surface
                    pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    
                    // Button surface
                    Region = new Region(pathSurface);
                    
                    // Button background
                    Color buttonColor = BackColor;
                    if (_isPressed)
                        buttonColor = _pressedColor;
                    else if (_isHovering)
                        buttonColor = _hoverColor;
                        
                    using (SolidBrush brushBackground = new SolidBrush(buttonColor))
                    {
                        pevent.Graphics.FillPath(brushBackground, pathSurface);
                    }
                    
                    // Button border
                    if (BorderSize > 0)
                    {
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
                    }
                    
                    // Draw text
                    TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, ForeColor, 
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
            else // Normal button
            {
                // Button background
                Color buttonColor = BackColor;
                if (_isPressed)
                    buttonColor = _pressedColor;
                else if (_isHovering)
                    buttonColor = _hoverColor;
                    
                using (SolidBrush brushBackground = new SolidBrush(buttonColor))
                {
                    pevent.Graphics.FillRectangle(brushBackground, rectSurface);
                }
                
                // Button border
                if (BorderSize > 0)
                {
                    using (Pen penBorder = new Pen(BorderColor, BorderSize))
                    {
                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, Width - 1, Height - 1);
                    }
                }
                
                // Draw text
                TextRenderer.DrawText(pevent.Graphics, Text, Font, ClientRectangle, ForeColor, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            // Top-left corner
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            
            // Top-right corner
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            
            // Bottom-right corner
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            
            // Bottom-left corner
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            
            path.CloseFigure();
            return path;
        }
    }
}
