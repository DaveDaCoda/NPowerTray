﻿using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NPowerTray
{
    static class Program
    {
        public static readonly string AuthorEmail = "gigaherz@gmail.com";
        public static readonly string ProgramUrl = "https://github.com/gigaherz/NPowerTray/releases/latest";
        public static readonly string UpdateUrl = "https://raw.githubusercontent.com/gigaherz/NPowerTray/master/version.txt";

        public static readonly RegistryKey BaseKey = Registry.CurrentUser;
        public static readonly string AppKey = "SOFTWARE\\NPowerTray";
        private const string RunKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static bool StartupState
        {
            get
            {
                var startupKey = BaseKey.OpenSubKey(RunKey);

                if (startupKey == null)
                    return false;

                var text = (string)startupKey.GetValue(Application.ProductName);

                bool value = String.CompareOrdinal(text, Application.ExecutablePath) == 0;

                startupKey.Close();

                return value;
            }
            set
            {
                var startupKey = BaseKey.OpenSubKey(RunKey, true);

                if (startupKey == null)
                    return;

                var old = (string)startupKey.GetValue(Application.ProductName);
                var oldSame = old != null && String.CompareOrdinal(old, Application.ExecutablePath) == 0;

                if (value && oldSame)
                {
                    startupKey.Close();
                    return;
                }

                if (old != null)
                    startupKey.DeleteValue(Application.ProductName, false);
                else if (value)
                    startupKey.SetValue(Application.ProductName, Application.ExecutablePath);

                startupKey.Close();
            }
        }

        /// <summary>
        /// Used to ensure the application closes when the main window closes.
        /// </summary>
        private class CustomAppContext : ApplicationContext
        {
            public CustomAppContext(Form form)
            {
                form.Closed += OnFormClosed;
            }

            void OnFormClosed(object sender, EventArgs e)
            {
                OnMainFormClosed(sender, e);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // The config form doubles up as the handler of the notification icon events.
            var form = new NPowerTray();
            form.Hide(); 
            
            var ac = new CustomAppContext(form);

            Application.Run(ac);
        }
    }
}
