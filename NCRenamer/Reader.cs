using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCRenamer
{
    public static class Reader
    {
        public const string Nameless = "NONAME";

        public static readonly string[] mazatrolExtensions = { ".pbg", ".pbd", ".eia" };
        public static readonly string[] heidenhainExtensions = { ".h" };
        public static readonly string[] sinumerikExtensions = { ".mpf", ".spf" };
        public static readonly string[] otherNcExtensions = { ".nc", ".tap" };

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
        /// Читает файл УП СЧПУ Heidenhain в поисках названия УП.
        /// </summary>
        /// <param name="filePath">Путь к файлу УП Fanuc</param>
        /// <returns>Возвращает строку содержащую имя УП, при неудаче возвращает значение NONAME</returns>
        public static string GetHeidenhainName(string filePath)
        {
            string name;
            try
            {
                // если файл не пустой и начинается с %
                if (!string.IsNullOrWhiteSpace(File.ReadAllText(filePath)))
                {
                    name = File.ReadAllLines(filePath)[1].Replace("BEGIN PGM ", string.Empty) ; // берем значение между скобок во второй строке
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


        public static string GetNcName(string filePath)
        {
            if (mazatrolExtensions.Contains(Path.GetExtension(filePath)?.ToLower()))
            {
                return GetMazatrolSmartName(filePath);
            }
            else if (sinumerikExtensions.Contains(Path.GetExtension(filePath)?.ToLower())) 
            {
                return string.Empty; // написать обработчик
            }
            else if (heidenhainExtensions.Contains(Path.GetExtension(filePath)?.ToLower())) 
            {
                return string.Empty; // написать обработчик
            }
            else
            {
                return GetFanucName(filePath);
            }
        }

        /// <summary>
        /// Переименовывает файл
        /// </summary>
        /// <param name="fileInfo">Файл</param>
        /// <param name="newName">Новое имя</param>
        public static void Rename(this FileInfo file, string newName)
        {
            string tempExtension = string.Empty;
            if (mazatrolExtensions.Contains(file.Extension)) tempExtension = file.Extension; 
            file.MoveTo(Path.Combine(file.Directory.FullName, newName + tempExtension), true);
            //Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fileInfo.Directory.FullName, newName);
        }
    }
}
