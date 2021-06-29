using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Utils.WinAPI.Windows;

namespace o2ewss
{
    class Program
    {
        readonly static string conigFile = Path.Combine(Directory.GetCurrentDirectory(), "o2ewss.cfg");

        static void Main()
        {
            if (File.Exists(conigFile))
            {
                string[] pathes = File.ReadAllLines(conigFile);
                if(pathes.Length > 1)
                {
                    Process.Start("explorer", pathes[0]);
                    Process.Start("explorer", pathes[1]);
                    var leftWindow = new Window(IntPtr.Zero);
                    var rightWindow = new Window(IntPtr.Zero);
                    while (leftWindow.Handle != IntPtr.Zero && rightWindow.Handle != IntPtr.Zero)
                    {
                        Thread.Sleep(200);
                        leftWindow = Window.Find(w => w.Text.Equals(new DirectoryInfo(pathes[0]).Name))[0];
                        rightWindow = Window.Find(w => w.Text.Equals(new DirectoryInfo(pathes[1]).Name))[0];
                    }
                    var screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                    var screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

                    leftWindow.Location = new System.Drawing.Point(0, 0);
                    leftWindow.Height = (int)screenHeight;
                    leftWindow.Width = (int)screenWidth / 2;

                    rightWindow.Location = new System.Drawing.Point((int)screenWidth / 2, 0);
                    rightWindow.Height = (int)screenHeight;
                    rightWindow.Width = (int)screenWidth / 2;
                }
            }
            else
            {
                try
                {
                    Console.WriteLine($"Конфигурации не обранужен. Попытка создать новый.");
                    File.Create(conigFile).Dispose();
                    Console.WriteLine($"Успешно.\n" +
                        $"Расположение: {conigFile}\n" +
                        $"Необходимо заполнить его и заново запустить программу.");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Ошибка доступа при создании файла конфигурации.");
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Необработанное исключение: {e}\n{e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    Console.WriteLine("\nДля закрытия этого окна нажмите любую клавишу...");
                    Console.ReadKey();
                }
            }
        }
    }
}
