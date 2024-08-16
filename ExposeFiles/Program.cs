using Spectre.Console;
using Spectre.Console.Cli;

namespace ExposeFiles
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            var app = new CommandApp<RemoveHiddenCommand>();
            app.Run(args);
            Console.ReadLine();
        }
    }

    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<directory>")]
        public string DirectoryPath { get; set; } = "";
    }

    public class RemoveHiddenCommand : Command<Settings>
    {
        public override int Execute(CommandContext context, Settings settings)
        {
            if (!Directory.Exists(settings.DirectoryPath))
            {
                AnsiConsole.MarkupLine("[red]Указанная директория не существует.[/]");
                return 1;
            }

            AnsiConsole.MarkupLine($"[yellow]Проверка директории: {settings.DirectoryPath}[/]");

            string[] files = Array.Empty<string>();
            AnsiConsole.WriteLine();
            AnsiConsole.Status()
            .Spinner(Spinner.Known.Default)
            .Start("Подсчет файлов", ctx => {
                files = Directory.GetFiles(settings.DirectoryPath, "*", SearchOption.AllDirectories);
            });
            
            if (files.Length < 1) 
            {
                AnsiConsole.MarkupLine("[red]Указанная директория пустая.[/]");
                return 1;
            }
            ProcessFiles(files);
            return 0;
        }

        private void ProcessFiles(string[] files)
        {
            int fileCount = files.Length;
            int exposed = 0;
                AnsiConsole.Progress()
                    .Start(ctx =>
                    {
                        var task = ctx.AddTask("Обработка файлов", new ProgressTaskSettings() 
                        { 
                            MaxValue = fileCount
                        });

                        foreach (var file in files)
                        {
                            var attributes = File.GetAttributes(file);

                            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                attributes &= ~FileAttributes.Hidden;
                                File.SetAttributes(file, attributes);
                                exposed++;
                            }

                            task.Increment(1);
                        }
                    });

            AnsiConsole.MarkupLine($"Процесс завершен! Раскрыто файлов: {exposed}.");
        }
    }
}