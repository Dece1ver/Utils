using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AutoElma.Infrastructure
{
    public static class Util
    {
        public static void WriteConfig(string path, Settings settings)
        {
            JsonSerializer serializer = new();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using StreamWriter sw = new(path);
            using JsonWriter writer = new JsonTextWriter(sw);
            serializer.Serialize(writer, settings);
        }

        public static Settings SetUp(Settings settings)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("* * * Параметры Auto Elma * * *\n" +
                    "\nВведите номер параметра для изменения:");
                Console.WriteLine($"[1] Наименование рабочего процесса: {settings?.WorkName}");
                Console.WriteLine($"[2] Наименование обеденного процесса: {settings?.DinnerName}");
                Console.WriteLine($"[3] Время обеденного процесса, мин: {settings.DinnerTime}");
                Console.WriteLine($"[4] Автоматическое закрытие задачи: {(settings.AutoConfim ? "Да" : "Нет")}");
                Console.WriteLine($"\n[0] Сохранить и вернуться");

                Console.Write("\n>");
                var choice = Console.ReadKey().Key;
                Console.Clear();
                switch (choice)
                {
                    case ConsoleKey.NumPad1 or ConsoleKey.D1:
                        Console.Write("Наименование рабочего процесса: ");
                        settings.WorkName = Console.ReadLine();
                        break;
                    case ConsoleKey.NumPad2 or ConsoleKey.D2:
                        Console.Write("Наименование обеденного процесса: ");
                        settings.DinnerName = Console.ReadLine();
                        break;
                    case ConsoleKey.NumPad3 or ConsoleKey.D3:
                        Console.Write("Время обеденного процесса, мин: ");
                        Int32.TryParse(Console.ReadLine(), out int result);
                        settings.DinnerTime = result;
                        break;
                    case ConsoleKey.NumPad4 or ConsoleKey.D4:
                        Console.Write("Автоматическое закрытие задачи [y/n]: ");
                        if (new string[] { "true", "+", "y", "yes", "да" }.Contains(Console.ReadLine().ToLower()))
                        {
                            settings.AutoConfim = true;
                        }
                        else
                        {
                            settings.AutoConfim = false;
                        }
                        break;
                    case ConsoleKey.NumPad0 or ConsoleKey.D0:
                        return settings;
                    default:
                        break;
                }
            }
        }

        public static string Work()
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl("https://www.google.com/ncr");
                driver.FindElement(By.Name("q")).SendKeys("cheese" + Keys.Enter);
                wait.Until(webDriver => webDriver.FindElement(By.CssSelector("h3")).Displayed);
                IWebElement firstResult = driver.FindElement(By.CssSelector("h3"));
                Console.Clear();
                return firstResult.GetAttribute("textContent");
            }
        }
    }

    
}
