using System;
using System.IO;

namespace NCRenamer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Rename(args);
            }
            else
            {
                Rename(Directory.GetFiles(Directory.GetCurrentDirectory()));
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
    }
}
