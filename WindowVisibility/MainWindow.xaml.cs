using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowVisibility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        bool watch = false;
        bool visibilityInWindowVisible = false;
        bool visibilityFindWindow = false;
        bool visibilityIsIconic = false;

        private async void setProcessButton_Click(object sender, RoutedEventArgs e)
        {
            watch = !watch;

            try
            {
                statusTB.Text = "Запуск наблюдения";
                Process[] processes = Process.GetProcessesByName(processNameTB.Text);

                while (watch)
                {
                    visibilityInWindowVisible = false;
                    visibilityFindWindow = false;
                    visibilityIsIconic = false;
                    setProcessButton.Content = "Стоп";
                    processNameTB.IsEnabled = false;
                    if(processes.Length > 0)
                    {
                        foreach (Process process in processes)
                        {
                            statusTB.Text = "Наблюдение";
                            IntPtr wHnd = process.MainWindowHandle;
                            if (wHnd != IntPtr.Zero)
                            {
                                visibilityFindWindow = true;

                            }
                            if (IsWindowVisible(wHnd))
                            {
                                visibilityInWindowVisible = true;

                            }
                            if (IsIconic(wHnd))
                            {
                                visibilityIsIconic = true;
                            }
                            else
                            {
                                resultTBIsIconic.Text = $"Окно процесса {processNameTB.Text} не найдено.";
                            }
                        }

                        resultTBFindWindow.Text = visibilityInWindowVisible.ToString();
                        resultTBIsWindowVisible.Text = visibilityFindWindow.ToString();
                        resultTBIsIconic.Text = visibilityIsIconic.ToString();
                    }

                    else
                    {
                        statusTB.Text = "Процесс не найден";
                        watch = false;
                    }
                    await Task.Delay(500);
                }
                setProcessButton.Content = "Старт";
                processNameTB.IsEnabled = true;
                statusTB.Text = "Наблюдение остановлено";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                statusTB.Text = "Ошибка";
            }
        }
    }
}
