using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EncryptedDiary
{
    /// <summary>
    /// A custom panel control with rounded corners
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int _cornerRadius = 10;
        private Color _borderColor = Color.DarkGray;
        private int _borderThickness = 1;
        private bool _drawShadow = true;
        private Color _shadowColor = Color.FromArgb(60, 0, 0, 0);

        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        public int BorderThickness
        {
            get => _borderThickness;
            set { _borderThickness = value; Invalidate(); }
        }
        
        public bool DrawShadow
        {
            get => _drawShadow;
            set { _drawShadow = value; Invalidate(); }
        }

        public Color ShadowColor
        {
            get => _shadowColor;
            set { _shadowColor = value; Invalidate(); }
        }

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            Padding = new Padding(5);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Define the rounded rectangle path
            GraphicsPath path = new GraphicsPath();
            Rectangle rectBorder = new Rectangle(
                _borderThickness / 2,
                _borderThickness / 2,
                Width - _borderThickness,
                Height - _borderThickness);

            if (_cornerRadius > 0)
            {
                int radius = _cornerRadius * 2;
                path.AddArc(rectBorder.X, rectBorder.Y, radius, radius, 180, 90);
                path.AddArc(rectBorder.Right - radius, rectBorder.Y, radius, radius, 270, 90);
                path.AddArc(rectBorder.Right - radius, rectBorder.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(rectBorder.X, rectBorder.Bottom - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
            }
            else
            {
                path.AddRectangle(rectBorder);
            }

            // Draw shadow if enabled
            if (_drawShadow)
            {
                using (GraphicsPath shadowPath = new GraphicsPath())
                {
                    Rectangle shadowRect = new Rectangle(
                        _borderThickness / 2 + 2,
                        _borderThickness / 2 + 2,
                        Width - _borderThickness,
                        Height - _borderThickness);

                    if (_cornerRadius > 0)
                    {
                        int radius = _cornerRadius * 2;
                        shadowPath.AddArc(shadowRect.X, shadowRect.Y, radius, radius, 180, 90);
                        shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Y, radius, radius, 270, 90);
                        shadowPath.AddArc(shadowRect.Right - radius, shadowRect.Bottom - radius, radius, radius, 0, 90);
                        shadowPath.AddArc(shadowRect.X, shadowRect.Bottom - radius, radius, radius, 90, 90);
                        shadowPath.CloseAllFigures();
                    }
                    else
                    {
                        shadowPath.AddRectangle(shadowRect);
                    }

                    using (SolidBrush shadowBrush = new SolidBrush(_shadowColor))
                    {
                        e.Graphics.FillPath(shadowBrush, shadowPath);
                    }
                }
            }

            // Fill the background
            using (SolidBrush bgBrush = new SolidBrush(BackColor))
            {
                e.Graphics.FillPath(bgBrush, path);
            }

            // Draw the border
            if (_borderThickness > 0)
            {
                using (Pen borderPen = new Pen(_borderColor, _borderThickness))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            // Draw the content
            base.OnPaint(e);
        }
    }
}
