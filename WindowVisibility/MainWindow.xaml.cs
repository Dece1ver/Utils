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
using System.Windows.Controls.Primitives;
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


        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        bool watch = false;
        bool visibilityInWindowVisible = false;
        bool visibilityFindWindow = false;
        bool visibilityIsIconic = false;
        List<string> windows = new();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread winSearchThread = new Thread(new ThreadStart(SearchWindows));
            //winSearchThread.Start();
            SearchWindows();
        }

        string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd) + 1;
            StringBuilder sb = new StringBuilder(len);
            len = GetWindowText(hWnd, sb, len);
            return sb.ToString(0, len);
        }

        async void SearchWindows()
        {
            
            while (true)
            {
                List<string> tempList = new();
                EnumWindows((hWnd, lParam) => {
                    if (IsWindowVisible(hWnd) && GetWindowTextLength(hWnd) != 0)
                    {
                        tempList.Add(GetWindowText(hWnd));
                    }
                    return true;
                }, IntPtr.Zero);
                windows = tempList;
                windowsLV.ItemsSource = windows;
                await Task.Delay(500);
            }
        }

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

                    IntPtr wHnd = FindWindowByCaption(IntPtr.Zero, processNameTB.Text);
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
                    
                    resultTBFindWindow.Text = visibilityInWindowVisible.ToString();
                    resultTBIsWindowVisible.Text = visibilityFindWindow.ToString();
                    resultTBIsIconic.Text = visibilityIsIconic.ToString();

                    //if (processes.Length > 0)
                    //{
                    //    foreach (Process process in processes)
                    //    {
                    //        statusTB.Text = "Наблюдение";
                    //        IntPtr wHnd = process.MainWindowHandle;
                    //        if (wHnd != IntPtr.Zero)
                    //        {
                    //            visibilityFindWindow = true;

                    //        }
                    //        if (IsWindowVisible(wHnd))
                    //        {
                    //            visibilityInWindowVisible = true;

                    //        }
                    //        if (IsIconic(wHnd))
                    //        {
                    //            visibilityIsIconic = true;
                    //        }
                    //        else
                    //        {
                    //            resultTBIsIconic.Text = $"Окно процесса {processNameTB.Text} не найдено.";
                    //        }
                    //    }

                    //    resultTBFindWindow.Text = visibilityInWindowVisible.ToString();
                    //    resultTBIsWindowVisible.Text = visibilityFindWindow.ToString();
                    //    resultTBIsIconic.Text = visibilityIsIconic.ToString();
                    //}

                    //else
                    //{
                    //    statusTB.Text = "Процесс не найден";
                    //    watch = false;
                    //}
                    await Task.Delay(500);
                }
                setProcessButton.Content = "Старт";
                processNameTB.IsEnabled = true;
                statusTB.Text = "Наблюдение остановлено";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.StackTrace);
                watch = false;
                statusTB.Text = "Ошибка";
            }
        }

        private void windowsLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as System.Windows.Controls.ListView).SelectedItem;
            if (item != null)
            {
                if (!watch)
                {
                    processNameTB.Text = item.ToString();
                }
            }
        }
    }
}
