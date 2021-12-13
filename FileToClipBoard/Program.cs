using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FileToClipBoard
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string fileName = args[0];
                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                {
                    System.Collections.Specialized.StringCollection replacementList = new System.Collections.Specialized.StringCollection
                    {
                        fileName
                    };
                    Clipboard.SetFileDropList(replacementList);
                    Console.WriteLine($"Файл \"{Path.GetFileName(fileName)}\" скопирован в буфер обмена.");
                }
                else
                {
                    Console.WriteLine($"Файл \"{Path.GetFileName(fileName)}\" не существует.");
                }
            }
            else if (args.Length == 2)
            {
                string fileExtension = args[0];
                if(!fileExtension.StartsWith(".")) fileExtension = "." + fileExtension;
                string targetDir = args[1];
                Console.WriteLine($"Поиск самого свежего файла с расширением {fileExtension} в следующей директории:\n{targetDir}");
                string fileName = string.Empty;
                DateTime dt = new DateTime(1990, 1, 1);
                FileSystemInfo[] fileSystemInfo = new DirectoryInfo(targetDir).GetFileSystemInfos();
                foreach (FileSystemInfo fileSI in fileSystemInfo)
                {
                    if (fileSI.Extension == fileExtension)
                    {
                        if (dt < Convert.ToDateTime(fileSI.CreationTime))
                        {
                            dt = Convert.ToDateTime(fileSI.CreationTime);
                            fileName = fileSI.FullName;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    System.Collections.Specialized.StringCollection replacementList = new System.Collections.Specialized.StringCollection
                    {
                        fileName
                    };
                    Clipboard.SetFileDropList(replacementList);
                    Console.WriteLine($"Файл \"{Path.GetFileName(fileName)}\" скопирован в буфер обмена.");
                }
            }
            else
            {
                Console.WriteLine("При вызове с одним аргументом программа добавляет в буфер обмена файл, путь к которому был передан.\n\n" +
                    "При вызове с двумя аргументами программа добавляет в буфер обмена последний созданный файл с указанным расширением находщийся в целевой папке. \n" +
                    "При передаче двух аргументов нужно передавать:\n" +
                    "\t* Расширение файла;\n" +
                    "\t* Путь к целевой папке.");
                Console.ReadKey();
            }
        }
    }
}
