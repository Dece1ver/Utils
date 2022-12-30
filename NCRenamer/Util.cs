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
        public static readonly string[] pdfExtensions = { ".pdf" };

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
                if (!string.IsNullOrWhiteSpace(File.ReadAllText(filePath)) && File.ReadAllLines(filePath)[0] == "%")
                {
                    string nameLine = File.ReadAllLines(filePath)[1];
                    if (nameLine.StartsWith("<"))
                    {
                        name = File.ReadAllLines(filePath)[1].Split('<')[1].Split('>')[0];
                    }
                    else
                    {
                        name = File.ReadAllLines(filePath)[1].Split('(')[1].Split(')')[0];
                    }
                }
                else if (File.ReadAllLines(filePath)[0][..4] == "MSG ")
                {
                    return GetSinumerikName(filePath) + ".mpf";
                }
                else if (File.ReadAllLines(filePath)[0][..9] == "BEGIN PGM")
                {
                    return GetHeidenhainName(filePath) + ".h";
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
        /// Читает файл УП СЧПУ Mazatrol Smart в поисках названия УП.
        /// </summary>
        /// <param name="filePath">Путь к файлу УП Mazatrol Smart</param>
        /// <returns>Возвращает строку содержащую имя УП, при неудаче возвращает значение NONAME</returns>
        public static string GetMazatrolIntegrexName(string filePath)
        {
            try
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
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
                return GetMazatrolSmartName(filePath) + Path.GetExtension(filePath);
            }
            else if (sinumerikExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetSinumerikName(filePath);
            }
            else if (heidenhainExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetHeidenhainName(filePath);
            }
            else if (pdfExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetAreopagPdfName(filePath);
            }
            else
            {
                return GetFanucName(filePath);
            }
        }

        private static string GetAreopagPdfName(string filePath)
        {
            try
            {
                foreach (var line in File.ReadLines(filePath))
                {
                    if (line.Contains("ОБОЗНАЧЕНИЕ"))
                    {
                        var name = line.Split('>')[1].Split('<')[0];
                        foreach (char badSymbol in Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()))
                        {
                            name = name.Replace(badSymbol, '-');
                        }
                        return name + ".pdf";
                    }
                }
                return Nameless;
            }
            catch
            {
                return Nameless;
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
            if (file.FullName == newFilePath) return;
            int i = 1;
            string originalFilePath = newFilePath;
            while (File.Exists(newFilePath))
            {
                newFilePath = $"{Path.GetFileNameWithoutExtension(originalFilePath)} ({i++}){Path.GetExtension(originalFilePath)}";
            }
            file.MoveTo(newFilePath);
            //Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fileInfo.Directory.FullName, newName);
        }
        #endregion

        #region CLI
        /// <summary>
        /// Сохраняет иконку приложения рядом с собой
        /// </summary>
        public static string SaveIcon()
        {
            try
            {
                File.WriteAllBytes("NCRenamer.ico", Properties.Resources.rename);
                return "Иконка записана в директорию с программой.";
            }
            catch (UnauthorizedAccessException)
            {
                return "Недостаточно прав доступа для создания иконки в данной директории.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string AddToContextManu()
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
                return "Программа успешно добавлена в контекстное меню Windows\n\nДля продолжения нажмите любую клавишу...";
            }
            catch (UnauthorizedAccessException)
            {
                return "\nНедостаточно прав для выполнения операции. Попробуте запустить от имени администратора.";
            }
        }

        public static string RemoveFromContextManu()
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
                return "Программа успешно удалена из контекстного меню Windows\n\nДля продолжения нажмите любую клавишу...";
            }
            catch (UnauthorizedAccessException)
            {
                return "\nНедостаточно прав для выполнения операции. Попробуте запустить от имени администратора.";
            }
            catch (ArgumentException)
            {
                return "Программы и так нет в контекстном меню\n\nДля продолжения нажмите любую клавишу...";
            }


        }
        #endregion
    }
}
