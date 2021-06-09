using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using WorkLauncher.Properties;

namespace WorkLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly OpenFileDialog _kerioFileDialog = new();
        private readonly OpenFileDialog _rdpFileDialog = new();
        private ServiceController _service = new(Settings.Default.scName);
        private bool _autoRunRdp;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private void fileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            _kerioFileDialog.Filter = "EXE Файлы (*.EXE)|*.EXE|Все файлы (*.*)|*.*";
            if (_kerioFileDialog.ShowDialog() == true)
            {
                Settings.Default.filePath = _kerioFileDialog.FileName;
                Settings.Default.fileName = _kerioFileDialog.SafeFileName.Replace(".exe", "");
                Settings.Default.Save();
                pathTexBox.Text = Settings.Default.filePath;
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Исполняемый файл Kerio VPN изменен.{Environment.NewLine}";
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Новое имя процесса: {Settings.Default.fileName}" + Environment.NewLine;
            }
            else
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Выбор файла отменен.{Environment.NewLine}";
            }

        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_service.Status != ServiceControllerStatus.Running)
                {
                    _service.Start();
                    _service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
                    infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Служба запущена.{Environment.NewLine}";
                    _ = WatchAndStartProcessAsync();
                }
            }
            catch (InvalidOperationException)
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Указанная служба не установлена.{Environment.NewLine}";
                return;
            }

            Process.Start(Settings.Default.filePath);
            infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Kerio запущен.{Environment.NewLine}";
            infoTextBox.PageDown();

        }

        private async Task WatchAndStartProcessAsync()
        {
            await Task.Run(() =>
            {
                while (!_autoRunRdp)
                {
                    Thread.Sleep(500);
                    foreach (var process in Process.GetProcessesByName("kvpncgui"))
                    {
                        var wHnd = process.MainWindowHandle;
                        if (IsWindowVisible(wHnd)) continue;
                        _autoRunRdp = true;
                        break;
                    }
                }
            });
            infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Соединение установлено.{Environment.NewLine}";
            Process.Start("mstsc", Settings.Default.rdpPath);
            infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Подключение запущено.{Environment.NewLine}";
            infoTextBox.PageDown();
            _autoRunRdp = false;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_service.Status == ServiceControllerStatus.Running)
                {
                    _service.Stop();
                    _service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                    infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Служба остановлена.{Environment.NewLine}";
                }
            }
            catch (InvalidOperationException)
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Указанная служба не установлена.{Environment.NewLine}";
            }

            var processes = Process.GetProcessesByName(Settings.Default.fileName);
            foreach (var process in processes)
            {
                process.CloseMainWindow();
                process.Kill();
            }
            infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Все процессы Kerio завершены.{Environment.NewLine}";
            infoTextBox.PageDown();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pathTexBox.Text = Settings.Default.filePath;
            try
            {
                if (_service.Status == ServiceControllerStatus.Running)
                {
                    infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Служба Kerio работает.{Environment.NewLine}";
                }
                else
                {
                    infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Служба Kerio не запущена.{Environment.NewLine}";
                }
            }
            catch (InvalidOperationException)
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Указанная служба не установлена.{Environment.NewLine}";
            }
            if (Process.GetProcessesByName(Settings.Default.fileName).Length > 0)
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Процесс Kerio VPN работает.{Environment.NewLine}";
            }
            else
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Процесс Kerio VPN не запущен.{Environment.NewLine}";
            }

            svTexBox.Text = Settings.Default.scName;
            pathRDPTextBox.Text = Settings.Default.rdpPath;

        }

        private void setSvButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.scName = svTexBox.Text;
            Settings.Default.Save();
            _service = new ServiceController(Settings.Default.scName);
            infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Название службы изменено.{Environment.NewLine}";
            infoTextBox.PageDown();
        }

        private void changeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            _rdpFileDialog.Filter = "Remote Desktop (*.RDP)|*.RDP|Все файлы (*.*)|*.*";
            if (_rdpFileDialog.ShowDialog() == true)
            {
                Settings.Default.rdpPath = _rdpFileDialog.FileName;
                Settings.Default.Save();
                pathRDPTextBox.Text = Settings.Default.rdpPath;
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Путь к RDP подключению изменен.{Environment.NewLine}";
            }
            else
            {
                infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Выбор RDP подключения отменен.{Environment.NewLine}";
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mstsc", Settings.Default.rdpPath);
            infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: Подключение запущено.{Environment.NewLine}";
            infoTextBox.PageDown();
        }

        private void clearButton_MouseEnter(object sender, MouseEventArgs e)
        {
            clearButton.Opacity = 1;
        }

        private void clearButton_MouseLeave(object sender, MouseEventArgs e)
        {
            clearButton.Opacity = 0.5;
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            infoTextBox.Clear();
        }
    }

}
