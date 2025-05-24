#pragma warning disable CS0618 // Suppress "Type or member is obsolete" warnings

using System;
using System.Windows.Forms;

namespace EncryptedDiary
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                // Generate application icons and resources
                DiaryIconGenerator.GenerateAndSaveIcons();
                
                // Use the modernized login form for a better UI experience
                Application.Run(new ModernizedLoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}