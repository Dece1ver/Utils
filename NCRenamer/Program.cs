using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace NCRenamer
{
    class Program
    {
        private static bool IsElevated => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Rename(args);
            }
            else
            {
                //Rename(Directory.GetFiles(Directory.GetCurrentDirectory()));
                RunConsole();
            }
        }

        private static void Rename(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = new(files[i]);
                string newName = Util.GetNcName(file.FullName);
                if (newName == string.Empty) continue;
                //if (File.Exists(Path.Combine(file.Directory.FullName, newName))) file.Delete();
                file.Rename(newName);
            }
        }

        private static void RunConsole()
        {
            // Запускаем консоль.
            if (AllocConsole())
            {
                Console.Title = $"Переименовыватель УП {(IsElevated ? string.Empty : "(Ограниченный режим)")}";
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"{(IsElevated ? string.Empty : "Программа запущена без прав Администратора, работа в ограниченном режиме\n\n")}" +
                        $"[1] Добавить программу в контекстное меню Windows{(IsElevated ? string.Empty : " (недоступно в ограниченном режиме)")}\n" +
                        $"[2] Убрать программу из контекстного меню Windows{(IsElevated ? string.Empty : " (недоступно в ограниченном режиме)")}\n" +
                        $"[3] О программе\n" +
                        "\n" +
                        "[0] Закрыть программу\n");
                    Console.Write(">");
                    var output = Console.ReadKey().Key;
                    if (output is ConsoleKey.D0 or ConsoleKey.NumPad0)
                    {
                        break;
                    }
                    if (output is ConsoleKey.D1 or ConsoleKey.NumPad1 && IsElevated)
                    {
                        Console.Clear();
                        Util.AddToContextManu();
                    }
                    if (output is ConsoleKey.D2 or ConsoleKey.NumPad2 && IsElevated)
                    {
                        Console.Clear();
                        Util.RemoveFromContextManu();
                    }
                    if (output is ConsoleKey.D3 or ConsoleKey.NumPad3)
                    {
                        Console.Clear();
                        Console.WriteLine("Программа переименовывает файлы управляющих программ по названию самой управляющей программы.\n\n" +
                            "Поддерживаемые СЧПУ:\n" +
                            "* Fanuc 0i           [ O0001(НАЗВАНИЕ) ]\n" +
                            "* Fanuc 0i-*F        [ <НАЗВАНИЕ> ]\n" +
                            "* Mazatrol Smart     [ .PBG | .PBD ]\n" +
                            "* Sinumerik 840D sl  [ MSG (\"НАЗВАНИЕ\") ]\n" +
                            "* Hiedenhain         [ BEGIN PGM НАЗВАНИЕ MM ]\n");
                        Console.WriteLine("Использование программы подразумевается вызовом через контекстное меню (ПКМ) по переменовываемым файлам.\n" +
                            "Работа программы происходит без каких-либо уведомлений.\n" +
                            "Если программа определяет файл как УП, то происходит переименование.\n" +
                            "Если файл уже существует, он не перезаписывается, а создается копия с добавлением номера.");
                        Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                }
                // Закрываем консоль.
                _ = FreeConsole();
            }
        }
    }
}
