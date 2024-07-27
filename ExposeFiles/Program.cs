using Spectre.Console;
using Spectre.Console.Cli;

namespace ExposeFiles
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.UTF8;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var app = new CommandApp<RemoveHiddenCommand>();
            app.Run(args);
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

            AnsiConsole.MarkupLine($"[yellow]Начинаем обработку директории: {settings.DirectoryPath}[/]");

            string[] files = Array.Empty<string>();
            AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBall)
            .Start("Подсчет файлов", ctx => {
                Thread.Sleep(5000);
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