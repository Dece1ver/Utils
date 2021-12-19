using AutoElma;
using AutoElma.Infrastructure;
using Newtonsoft.Json;

string settingDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dece1ver", "AutoElma");
string settingFile = Path.Combine(settingDir, "settings.json");
Settings settings = new Settings();

if (!Directory.Exists(settingDir))
{
    Directory.CreateDirectory(settingDir);
}
try
{
    settings = JsonConvert.DeserializeObject<Settings>(settingFile);
}
catch
{
    Util.CreateConfig(settingFile, new Settings());
    settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingFile));
}

while (true)
{
    Console.Clear();
    Console.WriteLine("Главное меню");
    Console.WriteLine($"Наименование рабочего процесса: {settings.WorkName}");
    Console.WriteLine($"Наименование обеденного процесса: {settings.DinnerName}");
    Console.WriteLine($"Время обеденного процесса: {settings.DinnerTime}");
    Console.Write("\n>");
    var choice = Console.ReadKey().Key;
    switch (choice)
    {
        case ConsoleKey.NumPad1 or ConsoleKey.D1:

            Console.ReadKey();
                break;
        default:
            break;
    }
}
