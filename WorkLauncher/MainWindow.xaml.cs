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
using Utils.WinAPI.Windows;
using Keyboard = Utils.WinAPI.Windows.Keyboard;
using Window = Utils.WinAPI.Windows.Window;
using System.IO;

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
        Window _kerioWindow;
        //string infoText = string.Empty;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pathTexBox.Text = Settings.Default.filePath;
            try
            {
                if (_service.Status == ServiceControllerStatus.Running)
                {
                    WriteInfo("Служба Kerio работает.");
                }
                else
                {
                    WriteInfo("Cлужба Kerio не запущена.");
                }
            }
            catch (InvalidOperationException)
            {
                WriteInfo("Указанная служба не установлена.");
            }
            if (Process.GetProcessesByName(Settings.Default.fileName).Length > 0)
            {
                WriteInfo("Процесс Kerio VPN работает.");
            }
            else
            {
                WriteInfo("Процесс Kerio VPN не запущен.");
            }

            svTexBox.Text = Settings.Default.scName;
            pathRDPTextBox.Text = Settings.Default.rdpPath;
            //await Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        infoTextBox.Text = infoText;
            //        Task.Delay(200);
            //    }
            //});
        }

        /// <summary>
        /// Пишет время и сообщение в форму
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        private void WriteInfo(string message)
        {
            Application.Current.Dispatcher.InvokeAsync(() => { infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: {message}\n"; });
            //infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: {message}\n";
            Application.Current.Dispatcher.InvokeAsync(() => { infoTextBox.PageDown(); });
        }

        /// <summary>
        /// Пишет время и сообщение в форму
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        //private void SendInfo(string message)
        //{
        //    Application.Current.Dispatcher.InvokeAsync(() => { infoTextBox.Text += $"[{DateTime.Now:MM.dd.yyyy HH:mm:ss}]: {message}\n"; });
        //}

        // файлдаиалог к керио
        private void fileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            _kerioFileDialog.Filter = "EXE Файлы (*.EXE)|*.EXE|Все файлы (*.*)|*.*";
            if (_kerioFileDialog.ShowDialog() == true)
            {
                Settings.Default.filePath = _kerioFileDialog.FileName;
                Settings.Default.fileName = Path.GetFileNameWithoutExtension(_kerioFileDialog.FileName);
                Settings.Default.Save();
                pathTexBox.Text = Settings.Default.filePath;
                WriteInfo($"Исполняемый файл Kerio VPN изменен. Новое имя процесса: {Settings.Default.fileName}");
            }
            else
            {
                WriteInfo("Выбор файла отменен.");
            }

        }

        private void FindAndLaunchKerio()
        {
            while (_kerioWindow.Handle == IntPtr.Zero)
            {
                Thread.Sleep(500);
                var kerioWindows = Window.Find(w => w.Text.Equals("Kerio VPN Client"));
                foreach (var window in kerioWindows)
                {
                    _kerioWindow = window;
                    break;
                }
            }
            Thread.Sleep(700);
            Keyboard.KeyPress(Keys.Enter);
            if (File.Exists(Settings.Default.rdpPath) && Path.GetExtension(Settings.Default.rdpPath).Equals(".rdp", StringComparison.OrdinalIgnoreCase))
            {
                WriteInfo("Ожидание соединения с VPN");
                while (_kerioWindow.Visible)
                {
                    Thread.Sleep(500);
                    if (Process.GetProcessesByName(Settings.Default.fileName).Length == 0)
                    {
                        WriteInfo("Процесс Kerio VPN остановлен. Запуск соединения прерван.");
                        return;
                    }
                }
                WriteInfo("Соединение установлено. Запуск RDP.");
                Process.Start("mstsc", Settings.Default.rdpPath);
            }
            else
            {
                WriteInfo("Путь к RDP не указан. Автоматического запуска не будет.");
            }
        }


        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            _kerioWindow = new Window(IntPtr.Zero);
            
            try
            {
                if (_service.Status != ServiceControllerStatus.Running)
                {
                    _service.Start();
                    _service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
                    WriteInfo("Служба запущена.");
                }
            }
            catch (InvalidOperationException)
            {
                WriteInfo("Указанная служба не установлена.");
                return;
            }

            if (File.Exists(Settings.Default.filePath) && Path.GetFileNameWithoutExtension(Settings.Default.filePath).Equals("kvpncgui", StringComparison.OrdinalIgnoreCase))
            {
                var processes = Process.GetProcessesByName(Settings.Default.fileName);
                if (processes.Length > 0)
                {
                    foreach (var process in processes)
                    {
                        process.CloseMainWindow();
                        process.Kill();
                    }
                }
                var kerioProcess = Process.Start(Settings.Default.filePath);
                WriteInfo("Kerio VPN Client запущен.");
                Thread kerioFindThread = new(new ThreadStart(FindAndLaunchKerio));
                kerioFindThread.Start();
                //if (File.Exists(Settings.Default.rdpPath) && Path.GetExtension(Settings.Default.rdpPath).Equals(".rdp", StringComparison.OrdinalIgnoreCase))
                //{
                //    WriteInfo("Ожидание соединения с VPN и запуска RDP");
                //}
                //else
                //{
                //    WriteInfo("Путь к RDP не указан. Автоматического запуска не будет.");
                //}
            }
            else
            {
                MessageBox.Show("Указан неверный файл Kerio VPN Client.");
            }

        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_service.Status == ServiceControllerStatus.Running)
                {
                    _service.Stop();
                    _service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(1));
                    WriteInfo("Служба остановлена.");
                }
                else
                {
                    WriteInfo("Служба не запущена.");
                }
            }
            catch (InvalidOperationException)
            {
                WriteInfo("Указанная служба не установлена.");
            }

            var processes = Process.GetProcessesByName(Settings.Default.fileName);
            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    process.CloseMainWindow();
                    process.Kill();
                    WriteInfo("Все процессы Kerio завершены.");
                }
            }
            else
            {
                WriteInfo("Kerio VPN не запущен.");
            }
        }

        private void setSvButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.scName = svTexBox.Text;
            Settings.Default.Save();
            _service = new ServiceController(Settings.Default.scName);
            WriteInfo("Название службы изменено.");
        }

        private void changeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            _rdpFileDialog.Filter = "Remote Desktop (*.RDP)|*.RDP|Все файлы (*.*)|*.*";
            if (_rdpFileDialog.ShowDialog() == true)
            {
                Settings.Default.rdpPath = _rdpFileDialog.FileName;
                Settings.Default.Save();
                pathRDPTextBox.Text = Settings.Default.rdpPath;
                WriteInfo("Путь к RDP изменен.");
            }
            else
            {
                WriteInfo("Выбор RDP отменен.");
            }
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mstsc", Settings.Default.rdpPath);
            WriteInfo("Подключение запущено.");
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
