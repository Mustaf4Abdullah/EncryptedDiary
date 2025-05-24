using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace EncryptedDiary
{
    /// <summary>
    /// Utility class to generate a diary icon and save it to the Resources folder
    /// </summary>
    public static class DiaryIconGenerator
    {
        public static void GenerateAndSaveIcons()
        {
            string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(resourcesPath))
            {
                Directory.CreateDirectory(resourcesPath);
            }
            
            // Generate and save main diary icon
            using (Bitmap diaryIcon = CreateDiaryIcon(64, 64))
            {
                string iconPath = Path.Combine(resourcesPath, "diary_icon.png");
                diaryIcon.Save(iconPath, ImageFormat.Png);
            }
            
            // Generate and save small diary icon for UI elements
            using (Bitmap smallIcon = CreateDiaryIcon(32, 32))
            {
                string smallIconPath = Path.Combine(resourcesPath, "diary_icon_small.png");
                smallIcon.Save(smallIconPath, ImageFormat.Png);
            }
            
            // Generate additional icons for the application
            GenerateSaveIcon(resourcesPath);
            GenerateTagIcon(resourcesPath);
            GenerateSearchIcon(resourcesPath);
            GenerateCalendarIcon(resourcesPath);
        }
        
        private static Bitmap CreateDiaryIcon(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Fill background with a gradient
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                    new Rectangle(0, 0, width, height),
                    Color.FromArgb(41, 128, 185), // ModernTheme.PrimaryColor
                    Color.FromArgb(52, 152, 219), // ModernTheme.AccentColor
                    45f))
                {
                    g.FillRectangle(bgBrush, 0, 0, width, height);
                }
                
                // Draw diary book shape
                int padding = width / 10;
                Rectangle bookRect = new Rectangle(padding, padding, width - padding * 2, height - padding * 2);
                
                // Draw book cover
                using (SolidBrush coverBrush = new SolidBrush(Color.FromArgb(44, 62, 80)))
                {
                    g.FillRectangle(coverBrush, bookRect);
                }
                
                // Draw book binding
                using (SolidBrush bindingBrush = new SolidBrush(Color.FromArgb(52, 73, 94)))
                {
                    g.FillRectangle(bindingBrush, padding, padding, padding/2, height - padding * 2);
                }
                
                // Draw horizontal lines to represent pages
                using (Pen linePen = new Pen(Color.FromArgb(200, 255, 255, 255), 1))
                {
                    int lineCount = 5;
                    int lineSpacing = (height - padding * 2) / (lineCount + 1);
                    
                    for (int i = 1; i <= lineCount; i++)
                    {
                        int y = padding + i * lineSpacing;
                        g.DrawLine(linePen, padding * 2, y, width - padding * 2, y);
                    }
                }
                
                // Draw a small lock icon
                int lockSize = width / 5;
                Rectangle lockRect = new Rectangle(
                    width - padding - lockSize,
                    height - padding - lockSize,
                    lockSize,
                    lockSize);
                    
                using (SolidBrush lockBrush = new SolidBrush(Color.FromArgb(255, 221, 89)))
                {
                    g.FillEllipse(lockBrush, lockRect);
                    
                    // Draw keyhole
                    using (Pen keyholeOuterPen = new Pen(Color.FromArgb(44, 62, 80), 2))
                    {
                        int keyholeCenterX = lockRect.X + lockRect.Width / 2;
                        int keyholeCenterY = lockRect.Y + lockRect.Height / 2;
                        g.DrawEllipse(keyholeOuterPen, 
                            keyholeCenterX - lockSize/6, 
                            keyholeCenterY - lockSize/6, 
                            lockSize/3, 
                            lockSize/3);
                            
                        g.DrawLine(keyholeOuterPen, 
                            keyholeCenterX, 
                            keyholeCenterY + lockSize/6, 
                            keyholeCenterX, 
                            keyholeCenterY + lockSize/3);
                    }
                }
            }
            
            return bitmap;
        }
        
        private static void GenerateSaveIcon(string resourcesPath)
        {
            Bitmap saveIcon = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(saveIcon))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                
                // Draw floppy disk outline
                Rectangle diskRect = new Rectangle(2, 2, 28, 28);
                using (SolidBrush diskBrush = new SolidBrush(Color.FromArgb(52, 152, 219)))
                {
                    g.FillRectangle(diskBrush, diskRect);
                }
                
                // Draw label part
                Rectangle labelRect = new Rectangle(6, 6, 20, 10);
                using (SolidBrush labelBrush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(labelBrush, labelRect);
                }
                
                // Draw write-protect notch
                Rectangle notchRect = new Rectangle(22, 22, 6, 6);
                using (SolidBrush notchBrush = new SolidBrush(Color.White))
                {
                    g.FillRectangle(notchBrush, notchRect);
                }
            }
            
            saveIcon.Save(Path.Combine(resourcesPath, "save_icon.png"), ImageFormat.Png);
        }
        
        private static void GenerateTagIcon(string resourcesPath)
        {
            Bitmap tagIcon = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(tagIcon))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                
                // Create tag shape points
                Point[] tagPoints = new Point[]
                {
                    new Point(2, 8),
                    new Point(24, 8),
                    new Point(30, 16),
                    new Point(24, 24),
                    new Point(2, 24)
                };
                
                // Draw tag shape
                using (SolidBrush tagBrush = new SolidBrush(Color.FromArgb(230, 126, 34)))
                {
                    g.FillPolygon(tagBrush, tagPoints);
                }
                
                // Draw hole
                using (SolidBrush holeBrush = new SolidBrush(Color.White))
                {
                    g.FillEllipse(holeBrush, 6, 14, 4, 4);
                }
            }
            
            tagIcon.Save(Path.Combine(resourcesPath, "tag_icon.png"), ImageFormat.Png);
        }
        
        private static void GenerateSearchIcon(string resourcesPath)
        {
            Bitmap searchIcon = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(searchIcon))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                
                // Draw magnifying glass circle
                using (Pen glassPen = new Pen(Color.FromArgb(52, 152, 219), 3))
                {
                    g.DrawEllipse(glassPen, 4, 4, 16, 16);
                }
                
                // Draw handle
                using (Pen handlePen = new Pen(Color.FromArgb(52, 152, 219), 3))
                {
                    g.DrawLine(handlePen, 18, 18, 26, 26);
                }
            }
            
            searchIcon.Save(Path.Combine(resourcesPath, "search_icon.png"), ImageFormat.Png);
        }
        
        private static void GenerateCalendarIcon(string resourcesPath)
        {
            Bitmap calendarIcon = new Bitmap(32, 32);
            using (Graphics g = Graphics.FromImage(calendarIcon))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);
                
                // Draw calendar outline
                Rectangle calRect = new Rectangle(2, 6, 28, 24);
                using (SolidBrush calBrush = new SolidBrush(Color.FromArgb(41, 128, 185)))
                {
                    g.FillRectangle(calBrush, calRect);
                }
                
                // Draw top bar (header)
                Rectangle headerRect = new Rectangle(2, 2, 28, 6);
                using (SolidBrush headerBrush = new SolidBrush(Color.FromArgb(192, 57, 43)))
                {
                    g.FillRectangle(headerBrush, headerRect);
                }
                
                // Draw calendar grid lines
                using (Pen linePen = new Pen(Color.White, 1))
                {
                    // Vertical lines
                    g.DrawLine(linePen, 11, 8, 11, 28);
                    g.DrawLine(linePen, 21, 8, 21, 28);
                    
                    // Horizontal lines
                    g.DrawLine(linePen, 2, 14, 30, 14);
                    g.DrawLine(linePen, 2, 21, 30, 21);
                }
                
                // Draw hangers
                using (Pen hangerPen = new Pen(Color.FromArgb(192, 57, 43), 2))
                {
                    g.DrawLine(hangerPen, 8, 2, 8, 0);
                    g.DrawLine(hangerPen, 24, 2, 24, 0);
                }
            }
            
            calendarIcon.Save(Path.Combine(resourcesPath, "calendar_icon.png"), ImageFormat.Png);
        }
    }
}
