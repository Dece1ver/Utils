
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Window = Utils.WinAPI.Windows.Window;

namespace o2ewss
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private static readonly string conigFile = Path.Combine(Directory.GetCurrentDirectory(), "o2ewss.cfg");

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
                string[] pathes = File.ReadAllLines(conigFile, System.Text.Encoding.UTF8);
                if (pathes.Length > 1)
                {
                    Process.Start("explorer", pathes[0]);
                    Process.Start("explorer", pathes[1]);
                    var leftWindow = new Window(IntPtr.Zero);
                    var rightWindow = new Window(IntPtr.Zero);
                    while (leftWindow.Handle == IntPtr.Zero && rightWindow.Handle == IntPtr.Zero)
                    {
                        Thread.Sleep(200);
                        var leftWindows = Window.Find(w => w.Text.Equals(new DirectoryInfo(pathes[0]).Name));
                        var rightWindows = Window.Find(w => w.Text.Equals(new DirectoryInfo(pathes[1]).Name));
                        if (leftWindows.Length > 0 && rightWindows.Length > 0)
                        {
                            leftWindow = leftWindows[0];
                            rightWindow = rightWindows[0];
                        }
                    }
                    WriteInfo($"Окна найдены\nЛевое: \"{pathes[0]}\"\nПравое: \"{pathes[1]}\"");
                    var screenWidth = SystemParameters.PrimaryScreenWidth;
                    var screenHeight = SystemParameters.PrimaryScreenHeight;

                    WriteInfo("Перемещение левого окна.");
                    leftWindow.Location = new System.Drawing.Point(0, 0);
                    leftWindow.Height = (int)screenHeight;
                    leftWindow.Width = (int)screenWidth / 2;
                    WriteInfo($"Координаты: X{leftWindow.Location.X} Y{leftWindow.Location.Y}, размер: {leftWindow.Height}x{leftWindow.Width}");

                    WriteInfo("Перемещение правого окна.");
                    rightWindow.Location = new System.Drawing.Point((int)screenWidth / 2, 0);
                    rightWindow.Height = (int)screenHeight;
                    rightWindow.Width = (int)screenWidth / 2;
                    WriteInfo($"Координаты: X{rightWindow.Location.X} Y{rightWindow.Location.Y}, размер: {rightWindow.Height}x{rightWindow.Width}");
                    WriteInfo("Завершено.");
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
                    File.WriteAllText(conigFile, string.Empty, System.Text.Encoding.UTF8);
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
