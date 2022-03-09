using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NCToolTable
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void AppStartup(object sender, StartupEventArgs e)
        {
            // Application is running
            // Process command line args
            bool autoclose = false;
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == "-autoclose")
                {
                    autoclose = true;
                }
            }

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow(autoclose);
            if (autoclose)
            {
                mainWindow.Visibility = Visibility.Collapsed;
                mainWindow.Width = 0;
                mainWindow.Height = 0;
                mainWindow.WindowStyle = WindowStyle.None;
            }
            mainWindow.Show();
        }
    }
}
