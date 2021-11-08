using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace NCRenamer
{
    public static class Util
    {
        private const string registryName = "ncRenamer";
        private const string Nameless = "NONAME";

        public static readonly string[] mazatrolExtensions = { ".pbg", ".pbd", ".eia" };
        public static readonly string[] heidenhainExtensions = { ".h" };
        public static readonly string[] sinumerikExtensions = { ".mpf", ".spf" };
        public static readonly string[] otherNcExtensions = { ".nc", ".tap" };

        public static readonly string[] machineExtensions = Array.Empty<string>()
            .Concat(heidenhainExtensions)
            .Concat(mazatrolExtensions)
            .Concat(sinumerikExtensions)
            .ToArray();

        #region Чтение УП
        /// <summary>
        /// Читает файл УП СЧПУ Fanuc в поисках названия УП.
        /// </summary>
        /// <param name="filePath">Путь к файлу УП Fanuc</param>
        /// <returns>Возвращает строку содержащую имя УП, при неудаче возвращает значение NONAME</returns>
        public static string GetFanucName(string filePath)
        {
            string name;
            try
            {
                // если файл не пустой и начинается с %
                if (!string.IsNullOrWhiteSpace(File.ReadAllText(filePath)) && File.ReadAllLines(filePath)[0] == "%")
                {
                    name = File.ReadAllLines(filePath)[1].Split('(')[1].Split(')')[0]; // берем значение между скобок во второй строке
                }
                // если нет
                else
                {
                    name = string.Empty;
                }

            }
            catch (IndexOutOfRangeException)
            {
                name = Nameless;
            }
            // любая другая ошибка, н-р ошибка доступа к файлу или что-нибудь еще
            catch (Exception)
            {
                name = string.Empty;
            }
            // заменяет все плохие символы на -, чтобы ошибки при записи не было
            foreach (char badSymbol in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
            {
                name = name.Replace(badSymbol, '-');
            }
            return name;
        }

        /// <summary>
        /// Читает файл УП СЧПУ Sinumerik в поисках названия УП.
        /// </summary>
        /// <param name="filePath">Путь к файлу УП Fanuc</param>
        /// <returns>Возвращает строку содержащую имя УП, при неудаче возвращает значение NONAME</returns>
        public static string GetSinumerikName(string filePath)
        {
            string name;
            try
            {
                if (!string.IsNullOrWhiteSpace(File.ReadAllLines(filePath)[0]))
                {
                    name = File.ReadAllLines(filePath)[0].Split('(')[1].Split(')')[0].Trim('\"');
                }
                else
                {
                    name = string.Empty;
                }
            }
            catch (IndexOutOfRangeException)
            {
                name = Nameless;
            }
            // любая другая ошибка, н-р ошибка доступа к файлу или что-нибудь еще
            catch (Exception)
            {
                name = string.Empty;
            }
            // заменяет все плохие символы на -, чтобы ошибки при записи не было
            foreach (char badSymbol in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
            {
                name = name.Replace(badSymbol, '-');
            }
            return name;
        }

        /// <summary>
        /// Читает файл УП СЧПУ Heidenhain в поисках названия УП.
        /// </summary>
        /// <param name="filePath">Путь к файлу УП Fanuc</param>
        /// <returns>Возвращает строку содержащую имя УП, при неудаче возвращает значение NONAME</returns>
        public static string GetHeidenhainName(string filePath)
        {
            string name;
            try
            {
                // если файл не пустой
                if (!string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
                {
                    name = File.ReadAllLines(filePath)[0].Replace("BEGIN PGM ", string.Empty).TrimStart('0').Trim(); 
                    name = name.Remove(name.Length - 3);
                }
                else
                {
                    return string.Empty;
                }

            }
            catch (IndexOutOfRangeException)
            {
                name = Nameless;
            }
            // любая другая ошибка, н-р ошибка доступа к файлу или что-нибудь еще
            catch (Exception)
            {
                name = string.Empty;
            }
            // заменяет все плохие символы на -, чтобы ошибки при записи не было
            foreach (char badSymbol in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
            {
                name = name.Replace(badSymbol, '-');
            }
            return name;
        }

        /// <summary>
        /// Читает файл УП СЧПУ Mazatrol Smart в поисках названия УП.
        /// </summary>
        /// <param name="filePath">Путь к файлу УП Mazatrol Smart</param>
        /// <returns>Возвращает строку содержащую имя УП, при неудаче возвращает значение NONAME</returns>
        public static string GetMazatrolSmartName(string filePath)
        {
            try
            {
                string name = File.ReadAllText(filePath).Substring(80, 32).Trim().Trim('\0');
                foreach (char badSymbol in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
                {
                    name = name.Replace(badSymbol, '-');
                }
                return name;
            }
            catch
            {
                return Nameless;
            }
        }

        /// <summary>
        /// Получает имя УП в зависимости от расширения файла
        /// </summary>
        /// <param name="filePath">Путь к файлу УП</param>
        /// <returns></returns>
        public static string GetNcName(string filePath)
        {
            if (mazatrolExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetMazatrolSmartName(filePath);
            }
            else if (sinumerikExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetSinumerikName(filePath);
            }
            else if (heidenhainExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetHeidenhainName(filePath);
            }
            else
            {
                return GetFanucName(filePath);
            }
        }
        #endregion

        #region Расширения
        /// <summary>
        /// Переименовывает файл
        /// </summary>
        /// <param name="fileInfo">Файл</param>
        /// <param name="newName">Новое имя</param>
        public static void Rename(this FileInfo file, string newName)
        {
            string tempExtension = string.Empty;
            if (machineExtensions.Contains(file.Extension)) tempExtension = file.Extension;
            string newFilePath = Path.Combine(file.Directory.FullName, newName + tempExtension);
            if (file.FullName + tempExtension == newFilePath) return;
            int i = 1;
            string originalFilePath = newFilePath;
            while (File.Exists(newFilePath))
            {
                newFilePath = $"{originalFilePath} ({i++})";
            }
            file.MoveTo(newFilePath);
            //Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fileInfo.Directory.FullName, newName);
        }
        #endregion

        #region CLI
        /// <summary>
        /// Сохраняет иконку приложения рядом с собой
        /// </summary>
        private static void SaveIcon()
        {
            File.WriteAllBytes("NCRenamer.ico", Properties.Resources.rename);
        }

        public static void AddToContextManu()
        {
            SaveIcon();
            RegistryKey key;
            WindowsIdentity identify = WindowsIdentity.GetCurrent();
            RegistrySecurity regSecurity = new();
            RegistryAccessRule accessRule = new(identify.User, RegistryRights.FullControl, AccessControlType.Allow);
            regSecurity.SetAccessRule(accessRule);
            try
            {
                key = Registry.ClassesRoot.CreateSubKey(@"*\\shell\\" + registryName, RegistryKeyPermissionCheck.ReadWriteSubTree, regSecurity);
                key.SetValue("", "Переименовать УП");
                key.SetValue("Icon", Path.GetFullPath("NCRenamer.ico"));
                key = Registry.ClassesRoot.CreateSubKey(@"*\\shell\\" + registryName + "\\command", RegistryKeyPermissionCheck.ReadWriteSubTree, regSecurity);
                key.SetValue("", $"\"{Path.GetFullPath(Process.GetCurrentProcess().ProcessName)}.exe\" \"%1\"");
                Console.Write("Программа успешно добавлена в контекстное меню Windows\n\nДля продолжения нажмите любую клавишу...");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("\nНедостаточно прав для выполнения операции. Попробуте запустить от имени администратора.");
            }
            Console.ReadKey();
        }

        public static void RemoveFromContextManu()
        {
            RegistryKey key;
            WindowsIdentity identify = WindowsIdentity.GetCurrent();
            RegistrySecurity regSecurity = new();
            RegistryAccessRule accessRule = new(identify.User, RegistryRights.FullControl, AccessControlType.Allow);
            regSecurity.SetAccessRule(accessRule);

            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"*\\shell\\", true);
                key.DeleteSubKeyTree(registryName);
                Console.Write("Программа успешно удалена из контекстного меню Windows\n\nДля продолжения нажмите любую клавишу...");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("\nНедостаточно прав для выполнения операции. Попробуте запустить от имени администратора.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Программы и так нет в контекстном меню\n\nДля продолжения нажмите любую клавишу...");
            }
            Console.ReadKey();

        }
        #endregion
    }
}
