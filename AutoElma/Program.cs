using AutoElma;
using AutoElma.Infrastructure;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

bool appWork = true;
bool silent = (Environment.GetCommandLineArgs().Length > 1) && (Environment.GetCommandLineArgs()[1]) == "-auto" ? true : false;

string settingDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dece1ver", "AutoElma");
string settingFile = Path.Combine(settingDir, "settings.json");
Settings settings;
Console.Title = "Auto Elma";
Console.OutputEncoding = Encoding.UTF8;

new DriverManager().SetUpDriver(new ChromeConfig());

if (!Directory.Exists(settingDir))
{
    Directory.CreateDirectory(settingDir);
}
try
{
    Console.Write("Чтение параметров...");
    settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingFile));
    if (settings is null) throw new Exception();
    Console.Write("успешно.\n");
}
catch
{
    Console.Write("неудача.\nЗапись новых параметров...");
    Util.WriteConfig(settingFile, new Settings());
    settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingFile));
    Console.Write("успешно.\n");
}

while (appWork)
{
    
    Console.Clear();
    Console.WriteLine(!silent ? "* * * Auto Elma * * *\n" : "* * * Auto Elma [AUTO]* * *\n");
    Console.WriteLine($"Логин Elma: {settings?.Login}");
    Console.WriteLine($"Пароль Elma: {(settings?.Pass.Length > 0 ? new string('*', settings.Pass.Length) : string.Empty)}");
    Console.WriteLine($"Наименование рабочего процесса: {settings.WorkName}");
    Console.WriteLine($"Наименование обеденного процесса: {settings.DinnerName}");
    Console.WriteLine($"Время обеденного процесса, мин: {settings.DinnerTime}");
    Console.WriteLine($"Автоматическое закрытие задачи: {(settings.AutoConfim ? "Да" : "Нет")}");
    Console.WriteLine("\nВведите номер действия:");
    Console.WriteLine($"[1] Отметить работу");
    Console.WriteLine($"[2] Изменить параметры");
    Console.WriteLine($"\n[0] Выход");
    Console.Write("\n>");
    ConsoleKey choice;
    if (silent)
    {
        choice = ConsoleKey.NumPad1;
    }
    else
    {
        choice = Console.ReadKey().Key;
    }

    switch (choice)
    {
        case ConsoleKey.NumPad1 or ConsoleKey.D1:
            Console.Clear();
            try
            {
                string result = Util.Work(settings, silent);
                if (result == "Завершено.")
                {
                    Console.WriteLine(result + "\nДля завершения нажмите любую клавишу...");
                    appWork = false;
                    if (silent) break;
                    if (!settings.AutoConfim) Console.ReadKey();
                    break;
                }
                if (silent) appWork = false;
                if (silent) break;
                Console.WriteLine(result + "\nДля завершения нажмите любую клавишу...");
            }
            catch (InvalidOperationException)
            {
                Console.Clear();
                Console.WriteLine("Не удалось отметить процесс. Скорее всего устарел браузер Google Chrome.\n\n" +
                    "Для обновления сделайте следующее:\n" +
                    "1. Откройте Google Chrome;\n" +
                    "2. Откройте страницу обновления (⁞ -> Справка -> О браузере Google Chrome);\n" +
                    "3. Дождитесь завершения обновления и закройте Google Chrome);\n\n" +
                    "4. Снова попробуйте запустить отметку рабочего процесса.");
                Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
            }
            catch (WebDriverException ex)
            {
                if(ex is NoSuchWindowException)
                {
                    Console.Clear();
                    Console.WriteLine("Процесс прерван. Скорее всего браузер Google Chrome был закрыт.");
                    Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Не удалось отметить процесс.\n\n" +
                        $"{ex.Message}\n" +
                        $"{ex.StackTrace}\n");
                    Console.WriteLine("\nДля продолжения нажмите любую клавишу...");
                }
                
            }
            
            Console.ReadKey();
            break;
        case ConsoleKey.NumPad2 or ConsoleKey.D2:
            settings = Util.SetUp(settings);
            Util.WriteConfig(settingFile, settings);
            break;
        case ConsoleKey.NumPad3 or ConsoleKey.D3:

            break;
        case ConsoleKey.NumPad0 or ConsoleKey.D0:
            appWork = false;
            break;
        default:
            break;
    }
}