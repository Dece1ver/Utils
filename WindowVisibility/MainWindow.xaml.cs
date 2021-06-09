using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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


        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

        private bool _watch;
        private bool _visibilityInWindowVisible;
        private bool _visibilityFindWindow;
        private bool _visibilityIsIconic;
        private List<string> _windows = new();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Thread winSearchThread = new Thread(new ThreadStart(SearchWindows));
            //winSearchThread.Start();
            SearchWindows();
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd) + 1;
            var sb = new StringBuilder(len);
            len = GetWindowText(hWnd, sb, len);
            return sb.ToString(0, len);
        }

        private async void SearchWindows()
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
                _windows = tempList;
                windowsLV.ItemsSource = _windows;
                await Task.Delay(500);
            }
        }

        private async void setProcessButton_Click(object sender, RoutedEventArgs e)
        {
            _watch = !_watch;

            try
            {
                statusTB.Text = "Запуск наблюдения";
                var processes = Process.GetProcessesByName(processNameTB.Text);
                

                while (_watch)
                {
                    _visibilityInWindowVisible = false;
                    _visibilityFindWindow = false;
                    _visibilityIsIconic = false;
                    setProcessButton.Content = "Стоп";
                    processNameTB.IsEnabled = false;

                    var wHnd = FindWindowByCaption(IntPtr.Zero, processNameTB.Text);
                    if (wHnd != IntPtr.Zero)
                    {
                        _visibilityFindWindow = true;

                    }
                    if (IsWindowVisible(wHnd))
                    {
                        _visibilityInWindowVisible = true;

                    }
                    if (IsIconic(wHnd))
                    {
                        _visibilityIsIconic = true;
                    }
                    
                    resultTBFindWindow.Text = _visibilityInWindowVisible.ToString();
                    resultTBIsWindowVisible.Text = _visibilityFindWindow.ToString();
                    resultTBIsIconic.Text = _visibilityIsIconic.ToString();

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
                _watch = false;
                statusTB.Text = "Ошибка";
            }
        }

        private void windowsLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                if (!_watch)
                {
                    processNameTB.Text = item.ToString();
                }
            }
        }
    }
}
