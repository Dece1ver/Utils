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
            if (args.Length == 2)
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
                    System.Collections.Specialized.StringCollection replacementList = new System.Collections.Specialized.StringCollection();
                    replacementList.Add(fileName);
                    Clipboard.SetFileDropList(replacementList);
                    Console.WriteLine($"Файл \"{Path.GetFileName(fileName)}\" скопирован в буфер обмена.");
                }
            }
            else
            {
                Console.WriteLine("Программа ищет в целевой папке последний созданный файл с указанным расширением и добавляет его в буфер обмена. \n" +
                    "Необходимо запускать программу передав 2 аргумента:\n\n" +
                    "\t* Расширение файла;\n" +
                    "\t* Путь к целевой папке.");
                Console.ReadKey();
            }
        }
    }
}
