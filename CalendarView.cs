using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Configuration;

namespace EncryptedDiary
{
    public class CalendarView : UserControl
    {
        private DateTime currentMonth;
        private List<Rectangle> dayRects;
        private Dictionary<int, EntryStatus> dayEntryStatus;
        private bool darkMode = false;
        private int selectedDay = -1;
        private int userId = -1;
        private string connectionString;
        
        // Colors
        private Color headerBackColor;
        private Color dayBackColor;
        private Color dayTextColor;
        private Color selectedDayBackColor;
        private Color highlightDayBackColor;
        private Color todayBorderColor;
        private Color weekendTextColor;
        private Color entryIndicatorColor;
        
        // Event to notify when a date is selected
        public event EventHandler<DateSelectedEventArgs> DateSelected;
        
        public class DateSelectedEventArgs : EventArgs
        {
            public DateTime SelectedDate { get; }
            
            public DateSelectedEventArgs(DateTime selectedDate)
            {
                SelectedDate = selectedDate;
            }
        }
        
        // Status of entries for each day
        public enum EntryStatus
        {
            None,
            HasEntry,
            LongEntry
        }
        
        public CalendarView()
        {
            InitializeComponent();
            dayRects = new List<Rectangle>();
            dayEntryStatus = new Dictionary<int, EntryStatus>();
            currentMonth = DateTime.Today;
            
            // Set connection string
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["DiaryDb"].ConnectionString;
            }
            catch { }
            
            SetColorScheme();
        }
        
        public void Initialize(int userId, bool darkMode)
        {
            this.userId = userId;
            this.darkMode = darkMode;
            SetColorScheme();
            LoadMonthEntries();
            Invalidate();
        }
        
        public void SetDarkMode(bool dark)
        {
            this.darkMode = dark;
            SetColorScheme();
            Invalidate();
        }
        
        private void SetColorScheme()
        {
            if (darkMode)
            {
                headerBackColor = ModernTheme.DarkBackgroundColor;
                dayBackColor = Color.FromArgb(57, 62, 70);
                dayTextColor = Color.FromArgb(238, 238, 238);
                selectedDayBackColor = ModernTheme.PrimaryColor;
                highlightDayBackColor = Color.FromArgb(70, ModernTheme.PrimaryColor.R, ModernTheme.PrimaryColor.G, ModernTheme.PrimaryColor.B);
                todayBorderColor = Color.FromArgb(52, 152, 219); // Light blue
                weekendTextColor = Color.FromArgb(240, 147, 43); // Orange
                entryIndicatorColor = Color.FromArgb(46, 204, 113); // Green
            }
            else
            {
                headerBackColor = ModernTheme.PrimaryColor;
                dayBackColor = Color.White;
                dayTextColor = Color.FromArgb(44, 62, 80);
                selectedDayBackColor = ModernTheme.AccentColor;
                highlightDayBackColor = Color.FromArgb(70, ModernTheme.AccentColor.R, ModernTheme.AccentColor.G, ModernTheme.AccentColor.B);
                todayBorderColor = Color.FromArgb(41, 128, 185); // Dark blue
                weekendTextColor = Color.FromArgb(192, 57, 43); // Red
                entryIndicatorColor = Color.FromArgb(39, 174, 96); // Green
            }
        }
        
        private void LoadMonthEntries()
        {
            // Clear previous status
            dayEntryStatus.Clear();
            
            if (userId < 0 || string.IsNullOrEmpty(connectionString))
                return;
                
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Get the first and last day of the month
                    DateTime firstDay = new DateTime(currentMonth.Year, currentMonth.Month, 1);
                    DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                    
                    string query = @"
                        SELECT DAY(EntryDate) as Day, 
                               LEN(Content) as ContentLength
                        FROM DiaryEntries 
                        WHERE UserId = @UserId 
                          AND EntryDate BETWEEN @FirstDay AND @LastDay";
                          
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);
                        command.Parameters.AddWithValue("@FirstDay", firstDay);
                        command.Parameters.AddWithValue("@LastDay", lastDay);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int day = reader.GetInt32(0);
                                int contentLength = reader.GetInt32(1);
                                
                                // Determine entry status based on content length
                                EntryStatus status = contentLength > 500 ? EntryStatus.LongEntry : EntryStatus.HasEntry;
                                dayEntryStatus[day] = status;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        
        private void InitializeComponent()
        {
            this.DoubleBuffered = true;
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Size = new Size(400, 350);
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.Name = "CalendarView";
            
            this.Paint += CalendarView_Paint;
            this.MouseClick += CalendarView_MouseClick;
            this.MouseMove += CalendarView_MouseMove;
            this.Resize += CalendarView_Resize;
        }
        
        private void CalendarView_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Calculate dimensions
            int width = this.Width;
            int height = this.Height;
            int headerHeight = 50;
            int daysPerWeek = 7;
            
            // Draw header background
            using (SolidBrush headerBrush = new SolidBrush(headerBackColor))
            {
                g.FillRectangle(headerBrush, 0, 0, width, headerHeight);
            }
            
            // Draw month and year
            using (Font monthFont = new Font("Segoe UI", 16, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                string monthYear = currentMonth.ToString("MMMM yyyy");
                SizeF textSize = g.MeasureString(monthYear, monthFont);
                g.DrawString(monthYear, monthFont, textBrush, 
                    (width - textSize.Width) / 2, (headerHeight - textSize.Height) / 2);
            }
            
            // Draw navigation arrows
            DrawNavigationArrows(g, headerHeight);
            
            // Draw day names
            string[] dayNames = { "P", "S", "Ç", "P", "C", "C", "P" }; // Turkish day abbreviations
            int dayNameY = headerHeight + 5;
            int dayWidth = width / daysPerWeek;
            
            using (Font dayNameFont = new Font("Segoe UI", 10, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(dayTextColor))
            {
                for (int i = 0; i < daysPerWeek; i++)
                {
                    string dayName = dayNames[i];
                    SizeF textSize = g.MeasureString(dayName, dayNameFont);
                    float x = (i * dayWidth) + (dayWidth - textSize.Width) / 2;
                    g.DrawString(dayName, dayNameFont, textBrush, x, dayNameY);
                }
            }
            
            // Calculate calendar layout
            int calendarStartY = dayNameY + 30;
            int dayHeight = (height - calendarStartY) / 6; // Max 6 rows for days
            
            // Get the first day of the month and determine the first cell to start filling
            DateTime firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            int firstCellOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7; // Adjust for Monday as first day
            
            // Calculate the number of days in the month
            int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
            
            // Clear previous day rects
            dayRects.Clear();
            
            // Draw calendar cells
            using (Font dayFont = new Font("Segoe UI", 12, FontStyle.Regular))
            using (Font todayFont = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                int day = 1;
                bool isToday;
                
                for (int row = 0; row < 6; row++)
                {
                    for (int col = 0; col < daysPerWeek; col++)
                    {
                        int cellIndex = row * daysPerWeek + col;
                        int cellOffset = cellIndex - firstCellOffset;
                        
                        if (cellOffset >= 0 && cellOffset < daysInMonth)
                        {
                            day = cellOffset + 1;
                            
                            // Calculate cell rectangle
                            Rectangle dayRect = new Rectangle(col * dayWidth, 
                                calendarStartY + row * dayHeight, dayWidth, dayHeight);
                                
                            // Store the rectangle for hit testing
                            dayRects.Add(dayRect);
                            
                            // Check if this is today
                            isToday = (day == DateTime.Today.Day && 
                                      currentMonth.Month == DateTime.Today.Month && 
                                      currentMonth.Year == DateTime.Today.Year);
                                      
                            // Check if this is selected day
                            bool isSelected = (selectedDay == day);
                            
                            // Check if day has an entry
                            bool hasEntry = dayEntryStatus.TryGetValue(day, out EntryStatus status) && status != EntryStatus.None;
                            
                            // Check if weekend (Saturday or Sunday)
                            bool isWeekend = (col == 5 || col == 6);
                            
                            // Draw cell background
                            using (SolidBrush backBrush = new SolidBrush(
                                isSelected ? selectedDayBackColor : dayBackColor))
                            {
                                g.FillRectangle(backBrush, dayRect);
                            }
                            
                            // Draw day number
                            using (SolidBrush textBrush = new SolidBrush(
                                isWeekend ? weekendTextColor : dayTextColor))
                            {
                                string dayText = day.ToString();
                                SizeF textSize = g.MeasureString(dayText, isToday ? todayFont : dayFont);
                                float x = dayRect.X + (dayRect.Width - textSize.Width) / 2;
                                float y = dayRect.Y + 5;
                                g.DrawString(dayText, isToday ? todayFont : dayFont, textBrush, x, y);
                            }
                            
                            // Draw border for today
                            if (isToday)
                            {
                                using (Pen todayPen = new Pen(todayBorderColor, 2))
                                {
                                    g.DrawRectangle(todayPen, dayRect.X + 1, dayRect.Y + 1, 
                                        dayRect.Width - 3, dayRect.Height - 3);
                                }
                            }
                            
                            // Draw entry indicator
                            if (hasEntry)
                            {
                                // Draw dot indicator
                                using (SolidBrush indicatorBrush = new SolidBrush(entryIndicatorColor))
                                {
                                    int dotSize = status == EntryStatus.LongEntry ? 8 : 6;
                                    g.FillEllipse(indicatorBrush, 
                                        dayRect.X + (dayRect.Width - dotSize) / 2, 
                                        dayRect.Y + dayRect.Height - dotSize - 4,
                                        dotSize, dotSize);
                                }
                            }
                            
                            // Draw cell border
                            using (Pen borderPen = new Pen(Color.FromArgb(50, dayTextColor), 1))
                            {
                                g.DrawRectangle(borderPen, dayRect);
                            }
                        }
                    }
                }
            }
        }
        
        private void DrawNavigationArrows(Graphics g, int headerHeight)
        {
            int arrowWidth = 10;
            int arrowHeight = 16;
            int padding = 20;
            
            // Left arrow (Previous month)
            using (SolidBrush arrowBrush = new SolidBrush(Color.White))
            {
                Point[] leftArrow = new Point[3];
                leftArrow[0] = new Point(padding, headerHeight / 2);
                leftArrow[1] = new Point(padding + arrowWidth, headerHeight / 2 - arrowHeight / 2);
                leftArrow[2] = new Point(padding + arrowWidth, headerHeight / 2 + arrowHeight / 2);
                g.FillPolygon(arrowBrush, leftArrow);
                
                // Right arrow (Next month)
                Point[] rightArrow = new Point[3];
                rightArrow[0] = new Point(Width - padding, headerHeight / 2);
                rightArrow[1] = new Point(Width - padding - arrowWidth, headerHeight / 2 - arrowHeight / 2);
                rightArrow[2] = new Point(Width - padding - arrowWidth, headerHeight / 2 + arrowHeight / 2);
                g.FillPolygon(arrowBrush, rightArrow);
            }
        }
        
        private void CalendarView_MouseClick(object sender, MouseEventArgs e)
        {
            // Check for arrows click
            int headerHeight = 50;
            int padding = 20;
            int arrowWidth = 10;
            int arrowHeight = 16;
            
            // Left arrow (Previous month)
            Rectangle leftArrowRect = new Rectangle(
                padding - 10, headerHeight / 2 - arrowHeight, 
                arrowWidth + 20, arrowHeight * 2);
                
            // Right arrow (Next month)
            Rectangle rightArrowRect = new Rectangle(
                Width - padding - arrowWidth - 10, headerHeight / 2 - arrowHeight, 
                arrowWidth + 20, arrowHeight * 2);
                
            if (leftArrowRect.Contains(e.Location))
            {
                // Previous month
                currentMonth = currentMonth.AddMonths(-1);
                LoadMonthEntries();
                Invalidate();
                return;
            }
            else if (rightArrowRect.Contains(e.Location))
            {
                // Next month
                currentMonth = currentMonth.AddMonths(1);
                LoadMonthEntries();
                Invalidate();
                return;
            }
            
            // Check for day cell click
            DateTime firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            int firstCellOffset = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;
            
            for (int i = 0; i < dayRects.Count; i++)
            {
                if (dayRects[i].Contains(e.Location))
                {
                    int day = i + 1 - firstCellOffset;
                    if (day > 0 && day <= DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month))
                    {
                        selectedDay = day;
                        Invalidate();
                        
                        // Raise event
                        DateTime selectedDate = new DateTime(currentMonth.Year, currentMonth.Month, day);
                        DateSelected?.Invoke(this, new DateSelectedEventArgs(selectedDate));
                    }
                    break;
                }
            }
        }
        
        private void CalendarView_MouseMove(object sender, MouseEventArgs e)
        {
            // Change cursor for arrows
            int headerHeight = 50;
            int padding = 20;
            int arrowWidth = 10;
            int arrowHeight = 16;
            
            Rectangle leftArrowRect = new Rectangle(
                padding - 10, headerHeight / 2 - arrowHeight, 
                arrowWidth + 20, arrowHeight * 2);
                
            Rectangle rightArrowRect = new Rectangle(
                Width - padding - arrowWidth - 10, headerHeight / 2 - arrowHeight, 
                arrowWidth + 20, arrowHeight * 2);
                
            if (leftArrowRect.Contains(e.Location) || rightArrowRect.Contains(e.Location))
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }
        
        private void CalendarView_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
        
        // Public methods
        public void NavigateToMonth(DateTime date)
        {
            currentMonth = new DateTime(date.Year, date.Month, 1);
            selectedDay = date.Day;
            LoadMonthEntries();
            Invalidate();
        }
        
        public void RefreshEntries()
        {
            LoadMonthEntries();
            Invalidate();
        }
    }
}
