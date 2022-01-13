using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace loopa
{
    internal class Program
    {
        private static bool _loading;

        private static void Main(string[] args)
        {

            string targetDir;
            if (args.Length == 1 && Directory.Exists(args[0]))
            {
                targetDir = args[0];
            }
            else if (args.Length == 1 && (args[0] == "/?"))
            {
                Console.Clear();
                Console.WriteLine("Ищет файлы по указанному содержимому.\n" +
                    "Если просто запустить, то будет искать в папке с собой и во всех вложенных каталогах.\n" +
                    "Также можно перетащить нужную папку на этот файл, тогда будет искать в ней и всех вложенных каталогах.");
                Console.Write("\nДля продолжения нажмите любую клавишу...");
                Console.ReadKey();
                targetDir = Directory.GetCurrentDirectory();
            }
            else
            {
                targetDir = Directory.GetCurrentDirectory();
            }


            IEnumerable<string> files;
            string searchTarget = string.Empty;


            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Поиск в \"{targetDir}\"");
                Console.WriteLine("Что ищем? (справка /?)");
                searchTarget = Console.ReadLine();
                if (searchTarget == "/?")
                {
                    Console.Clear();
                    Console.WriteLine("Ищет файлы по указанному содержимому.\n" +
                        "Если просто запустить, то будет искать в папке с собой и во всех вложенных каталогах.\n" +
                        "Также можно перетащить нужную папку на этот файл, тогда будет искать в ней и всех вложенных каталогах.");
                    Console.Write("\nДля продолжения нажмите любую клавишу...");
                    Console.ReadKey();
                }
                else if (!string.IsNullOrEmpty(searchTarget))
                {
                    break;
                }
            }
            try
            {
                Console.Clear();
                Console.WriteLine($"Поиск \"{searchTarget}\" во всех файлах в указанной и всех вложенных папках.");
                //Console.Write("Подсчет файлов  ");
                Stopwatch stopWatch = new();
                Thread load = new(Loading);
                _loading = true;
                load.Start();
                stopWatch.Start();
                files = Directory.GetFiles(targetDir, "*.*", SearchOption.AllDirectories).AsEnumerable();
                stopWatch.Stop();
                var ts = stopWatch.Elapsed;
                _loading = false;
                int filesCount = files.Count();
                int filesFound = 0;
                int filesReaded = 0;
                List<string> goodFiles = new();

                Console.WriteLine($"\rПодсчет файлов завершен. Всего файлов: {filesCount}. Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.");
                stopWatch.Restart();
                foreach (var file in files)
                {
                    filesReaded++;
                    ts = stopWatch.Elapsed;
                    Console.Write($"\rПрочитано файлов: {filesReaded}. Из них подходящих: {filesFound}. Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.");
                    foreach (var line in File.ReadLines(file))
                    {
                        if (line.Contains(searchTarget))
                        {
                            goodFiles.Add(file);
                            filesFound++;
                            break;
                        }
                    }
                }
                stopWatch.Stop();
                ts = stopWatch.Elapsed;
                Console.WriteLine($"\rЧтение файлов завершено. Из них подходящих: {filesFound} Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.\n");
                Console.WriteLine(String.Join(Environment.NewLine, goodFiles).Trim());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Console.WriteLine("\nНажмите любую клавишу, чтобы закрыть это окно...");
                Console.ReadKey();
            }
        }


        private static void Loading()
        {
            int i = 0;
            Stopwatch stopWatch = new();
            stopWatch.Start();
            while (_loading)
            {
                i++;
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                var ts = stopWatch.Elapsed;
                switch (i)
                {
                    case 1:
                        Console.Write($"\rПодсчет файлов | Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.");
                        break;
                    case 2:
                        Console.Write($"\rПодсчет файлов / Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.");
                        break;
                    case 3:
                        Console.Write($"\rПодсчет файлов - Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.");
                        break;
                    default:
                        Console.Write($"\rПодсчет файлов \\ Затрачено времени: {(int)ts.TotalSeconds}.{ts.Milliseconds:D3}с.");
                        i = 0;
                        break;
                }
                Thread.Sleep(50);
            }
            stopWatch.Stop();
        }
    }
}
