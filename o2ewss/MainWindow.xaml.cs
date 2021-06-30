
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Utils.WinAPI.pInvoke;
using Window = Utils.WinAPI.Windows.Window;

namespace o2ewss
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private static readonly string conigFile = Path.Combine(Directory.GetCurrentDirectory(), "o2ewss.cfg");
        private const string defaultConfig = "#o2ewss_config\n<пусть к левому окну>\n<путь к правому окну>\nfalse";

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Пишет время и сообщение в форму
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        private void WriteInfo(string message)
        {
            _ = Application.Current.Dispatcher.InvokeAsync(() => { infoTextBox.Text += message + "\n"; });
            _ = Application.Current.Dispatcher.InvokeAsync(() => { infoTextBox.PageDown(); });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(conigFile))
            {
                string[] config = File.ReadAllLines(conigFile, System.Text.Encoding.UTF8);
                if (config.Length > 1 && config[0].Equals("#o2ewss_config"))
                {
                    if (Directory.Exists(config[1]) && Directory.Exists(config[2]))
                    {
                        if (Window.Find(w => w.Text.Equals(new DirectoryInfo(config[1]).Name)).Length == 0)
                        {
                            Process.Start("explorer", config[1]);
                        }
                        if (Window.Find(w => w.Text.Equals(new DirectoryInfo(config[2]).Name)).Length == 0)
                        {
                            Process.Start("explorer", config[2]);
                        }

                        var leftWindow = new Window(IntPtr.Zero);
                        var rightWindow = new Window(IntPtr.Zero);
                        while (leftWindow.Handle == IntPtr.Zero && rightWindow.Handle == IntPtr.Zero)
                        {
                            Thread.Sleep(200);
                            var leftWindows = Window.Find(w => w.Text.Equals(new DirectoryInfo(config[1]).Name));
                            var rightWindows = Window.Find(w => w.Text.Equals(new DirectoryInfo(config[2]).Name));
                            if (leftWindows.Length > 0 && rightWindows.Length > 0)
                            {
                                leftWindow = leftWindows[0];
                                rightWindow = rightWindows[0];
                            }
                        }
                        WriteInfo($"Окна найдены\nЛевое: \"{config[1]}\"\nПравое: \"{config[2]}\"");
                        var screenWidth = SystemParameters.PrimaryScreenWidth;
                        var screenHeight = SystemParameters.PrimaryScreenHeight;

                        WriteInfo("Перемещение левого окна.");
                        if (leftWindow.Iconic)
                        {
                            leftWindow.ShowWindow(SW.SHOWNORMAL);
                        }
                        leftWindow.Location = new System.Drawing.Point(-7, 0);
                        leftWindow.Height = (int)screenHeight - 33;
                        leftWindow.Width = (int)screenWidth / 2 + 15;
                        WriteInfo($"Координаты: X{leftWindow.Location.X} Y{leftWindow.Location.Y}, размер: {leftWindow.Height}x{leftWindow.Width}");

                        WriteInfo("Перемещение правого окна.");
                        if (rightWindow.Iconic)
                        {
                            rightWindow.ShowWindow(SW.SHOWNORMAL);
                        }
                        rightWindow.Location = new System.Drawing.Point((int)screenWidth / 2 - 7, 0);
                        rightWindow.Height = (int)screenHeight - 33;
                        rightWindow.Width = (int)screenWidth / 2 + 15;
                        WriteInfo($"Координаты: X{rightWindow.Location.X} Y{rightWindow.Location.Y}, размер: {rightWindow.Height}x{rightWindow.Width}");
                        WriteInfo("Завершено.");
                        if(config[3] == "true")
                        {
                            this.Close();
                        }
                    }
                    else
                    {
                        WriteInfo($"Пути указанные в файле конфигурации не существуют.");
                    }
                }
                else
                {
                    WriteInfo($"Файл конфигурации содержит некорректную информацию.");
                }
            }
            else
            {
                try
                {
                    WriteInfo($"Файл конфигурации не обранужен. Попытка создать новый.");
                    File.WriteAllText(conigFile, defaultConfig, System.Text.Encoding.UTF8);
                    WriteInfo($"Успешно.\n" +
                        $"Расположение: {conigFile}\n" +
                        $"Необходимо заполнить его и заново запустить программу.");
                }
                catch (UnauthorizedAccessException)
                {
                    WriteInfo("Ошибка доступа при создании файла конфигурации.");
                }
                catch (Exception exception)
                {
                    WriteInfo($"Необработанное исключение: {exception}\n{exception.Message}\n{exception.StackTrace}");
                }
            }
        }
    }
} 