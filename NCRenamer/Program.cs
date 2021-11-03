using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace NCRenamer
{
    class Program
    {
        private const string registryName = "ncRenamer";

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
                RunConsole();
            }
        }

        private static void Rename(string[] files)
        {
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = new(files[i]);
                string newName = Reader.GetNcName(file.FullName);
                if (newName == string.Empty) break;
                //if (File.Exists(Path.Combine(file.Directory.FullName, newName))) file.Delete();
                file.Rename(newName);
            }
        }

        /// <summary>
        /// Сохраняет иконку приложения рядом с собой
        /// </summary>
        private static void SaveIcon()
        {
            File.WriteAllBytes("NCRenamer.ico", Properties.Resources.rename);
        }

        private static void AddToContextManu()
        {
            SaveIcon();
            RegistryKey key;
            WindowsIdentity identify = WindowsIdentity.GetCurrent();
            RegistrySecurity regSecurity = new RegistrySecurity();
            RegistryAccessRule accessRule = new RegistryAccessRule(identify.User, RegistryRights.FullControl, AccessControlType.Allow);
            regSecurity.SetAccessRule(accessRule);
            Console.Write("Создание раздела реестра...");
            try
            {
                key = Registry.ClassesRoot.CreateSubKey(@"*\\shell\\" + registryName, RegistryKeyPermissionCheck.ReadWriteSubTree, regSecurity);
                Console.Write("Успешно\nДобавление программы в контекстное меню...");
                key.SetValue("", "Переименовать УП");
                Console.Write("Успешно\nДобавление иконки в контекстное меню...");
                key.SetValue("Icon", Path.GetFullPath("NCRenamer.ico"));
                Console.Write("Успешно\nДля продолжения нажмите любую клавишу...\n");

                key = Registry.ClassesRoot.CreateSubKey(@"*\\shell\\" + registryName + "\\command", RegistryKeyPermissionCheck.ReadWriteSubTree, regSecurity);
                //путь к программе для запуска
                key.SetValue("", $"\"{Path.GetFullPath(Process.GetCurrentProcess().ProcessName)}.exe\" \"%1\""); ;
            }
            catch(UnauthorizedAccessException)
            {
                Console.WriteLine("\nНедостаточно прав для выполнения операции. Попробуте запустить от имени администратора.");
            }
            Console.ReadKey();

        }

        private static void RemoveFromContextManu()
        {
            RegistryKey key;
            WindowsIdentity identify = WindowsIdentity.GetCurrent();
            RegistrySecurity regSecurity = new RegistrySecurity();
            RegistryAccessRule accessRule = new RegistryAccessRule(identify.User, RegistryRights.FullControl, AccessControlType.Allow);
            regSecurity.SetAccessRule(accessRule);
            key = Registry.ClassesRoot.OpenSubKey(@"*\\shell\\", true);
            
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"*\\shell\\", true);
                key.DeleteSubKeyTree(registryName);
                Console.WriteLine("Удалено.\nДля продолжения нажмите любую клавишу...");
            }
            catch (UnauthorizedAccessException)
            {

                Console.WriteLine("\nНедостаточно прав для выполнения операции. Попробуте запустить от имени администратора.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Программы нет в контекстном меню.\nДля продолжения нажмите любую клавишу...");
            }
            Console.ReadKey();

        }

        private static void RunConsole()
        {
            // Запускаем консоль.
            if (AllocConsole())
            {
                Console.Title = "Переименовыватель УП";
                
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Настройка программы:\n\n" +
                        $"[1] Добавить программу в контекстное меню Windows\n" +
                        $"[2] Убрать программу из контекстного меню Windows\n" +
                        $"[3] {(!File.Exists("NCRenamer.ico") ? "Создать" : "Перезаписать")} иконку приложения.\n" +
                        "\n" +
                        "[0] Закрыть программу.\n");
                    Console.Write(">");
                    // Считываем данные.
                    var output = Console.ReadKey().Key;
                    if (output == ConsoleKey.D0 || output == ConsoleKey.NumPad0)
                    {
                        break;
                    }
                    if (output == ConsoleKey.D1 || output == ConsoleKey.NumPad1)
                    {
                        Console.Clear();
                        AddToContextManu();
                    }
                    if (output == ConsoleKey.D2 || output == ConsoleKey.NumPad2)
                    {
                        Console.Clear();
                        RemoveFromContextManu();
                    }
                    if (output == ConsoleKey.D3 || output == ConsoleKey.NumPad3)
                    {
                        SaveIcon();
                        Console.Clear();
                        Console.WriteLine("Иконка создана. Для продолжения нажмите любую клавишу...");
                        Console.ReadKey();
                    }
                }
                // Закрываем консоль.
                FreeConsole();
            }
        }
    }
}
