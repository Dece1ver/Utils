using System.Security.Cryptography;
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
                Console.WriteLine($"[1] Логин Elma: {settings.Login}");
                Console.WriteLine($"[2] Пароль Elma: {(settings.Pass.Length > 0 ? new string('*', settings.Pass.Length) : string.Empty)}");
                Console.WriteLine($"[3] Наименование рабочего процесса: {settings.WorkName}");
                Console.WriteLine($"[4] Наименование обеденного процесса: {settings.DinnerName}");
                Console.WriteLine($"[5] Время обеденного процесса, мин: {settings.DinnerTime}");
                Console.WriteLine($"[6] Автоматическое закрытие задачи: {(settings.AutoConfim ? "Да" : "Нет")}");
                Console.WriteLine($"\n[0] Сохранить и вернуться");

                Console.Write("\n>");
                var choice = Console.ReadKey().Key;
                Console.Clear();
                switch (choice)
                {
                    case ConsoleKey.NumPad1 or ConsoleKey.D1:
                        Console.Write("Логин Elma: ");
                        settings.Login = Console.ReadLine();
                        break;
                    case ConsoleKey.NumPad2 or ConsoleKey.D2:
                        Console.Write("Пароль Elma: ");
                        settings.Pass = Encrypt(Console.ReadLine(), "http://areopag");
                        break;
                    case ConsoleKey.NumPad3 or ConsoleKey.D3:
                        Console.Write("Наименование рабочего процесса: ");
                        settings.WorkName = Console.ReadLine();
                        break;
                    case ConsoleKey.NumPad4 or ConsoleKey.D4:
                        Console.Write("Наименование обеденного процесса: ");
                        settings.DinnerName = Console.ReadLine();
                        break;
                    case ConsoleKey.NumPad5 or ConsoleKey.D5:
                        Console.Write("Время обеденного процесса, мин: ");
                        Int32.TryParse(Console.ReadLine(), out int result);
                        settings.DinnerTime = result;
                        break;
                    case ConsoleKey.NumPad6 or ConsoleKey.D6:
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

        public static string Work(Settings settings)
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                Console.Clear();
                driver.Navigate().GoToUrl("http://elma:8000/Tasks/AllTasks/Incoming?FilterId=0");
                driver.FindElement(By.Id("login")).SendKeys(settings.Login);
                driver.FindElement(By.Id("password")).SendKeys(Decrypt(settings.Pass, "http://areopag") + Keys.Enter);

                // поиск задачи на распределение времени
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.LinkText("Распределить рабочее время")).Displayed);
                        break;
                    }
                    catch (NoSuchElementException)
                    {
                        Console.Clear();
                        Console.WriteLine($"Поиск задач...попытка {i + 1}");
                        if (i == 4) return "Нет задач на распределение рабочего времени.";
                    }
                }

                // выбор задачи
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        driver.FindElement(By.LinkText("Распределить рабочее время")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Выбор задачи...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // открываем поиск
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.ClassName("combobox-icon")).Displayed);
                        driver.FindElement(By.ClassName("combobox-icon")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Открытие поиска...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // ищем
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.XPath("//input[@placeholder='Что искать?']")).Displayed);
                        driver.FindElement(By.XPath("//input[@placeholder='Что искать?']")).SendKeys(settings.DinnerName + Keys.Enter);
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Ввод наименования обеденного процесса...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // тыкаем первый найденный вариант
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Displayed);
                        driver.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Подтверждение...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // указываем время в минутах
                var time = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(settings.DinnerTime));
                if (time.Hour >= 1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            wait.Until(driver => driver.FindElement(By.Id("Entity_WorkedTime_hours")).Displayed);
                            driver.FindElement(By.Id("Entity_WorkedTime_hours")).SendKeys(time.Hour.ToString());
                            break;
                        }
                        catch
                        {
                            Console.Clear();
                            Console.WriteLine($"Ввод времени...попытка {i + 1}");
                            if (i == 4) return "Не удалось.";
                        }
                    }
                }
                if (time.Minute >= 1)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            wait.Until(driver => driver.FindElement(By.Id("Entity_WorkedTime_minutes")).Displayed);
                            driver.FindElement(By.Id("Entity_WorkedTime_minutes")).SendKeys(time.Minute.ToString());
                            break;
                        }
                        catch
                        {
                            Console.Clear();
                            Console.WriteLine($"Ввод времени...попытка {i + 1}");
                            if (i == 4) return "Не удалось.";
                        }
                    }
                }

                // добавляем
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Displayed);
                        driver.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Добавление...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // открываем поиск
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.ClassName("combobox-icon")).Displayed);
                        driver.FindElement(By.ClassName("combobox-icon")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Открытие поиска...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // ищем
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.XPath("//input[@placeholder='Что искать?']")).Displayed);
                        driver.FindElement(By.XPath("//input[@placeholder='Что искать?']")).SendKeys(settings.WorkName + Keys.Enter);
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Ввод наименования рабочего процесса...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // тыкаем первый найденный вариант
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Displayed);
                        driver.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Подтверждение...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }

                // указываем остаточное время
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.Id("Entity_WorkedTime_days")).Displayed);
                        driver.FindElement(By.Id("Entity_WorkedTime_days")).SendKeys("1");
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Ввод дня...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }


                // добавляем
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        wait.Until(driver => driver.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Displayed);
                        driver.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Click();
                        break;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine($"Добавление...попытка {i + 1}");
                        if (i == 4) return "Не удалось.";
                    }
                }
                Console.Clear();
                if (settings.AutoConfim)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            wait.Until(driver => driver.FindElement(By.XPath("//input[contains(@value,\"Выполнено\")]")).Displayed);
                            driver.FindElement(By.XPath("//input[contains(@value,\"Выполнено\")]")).Click();
                            break;
                        }
                        catch
                        {
                            Console.Clear();
                            Console.WriteLine($"Добавление...попытка {i + 1}");
                            if (i == 4) return "Не удалось.";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Подтвердите вручную и нажмите Enter в текущем окне.");
                    Console.ReadKey();
                }
                Thread.Sleep(2000);
                return "Завершено.";
            }
        }

        public static string Encrypt(string plainText, string password,
       string salt = "dece1ver", string hashAlgorithm = "SHA1",
       int passwordIterations = 2, string initialVector = "OFRna73m*aze01xY",
       int keySize = 256)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);
            AesCng symmetricKey = new();
            symmetricKey.Mode = CipherMode.CFB;

            byte[] cipherTextBytes = null;

            using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initialVectorBytes))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memStream.ToArray();
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmetricKey.Clear();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText, string password,
       string salt = "dece1ver", string hashAlgorithm = "SHA1",
       int passwordIterations = 2, string initialVector = "OFRna73m*aze01xY",
       int keySize = 256)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

            byte[] initialVectorBytes = Encoding.ASCII.GetBytes(initialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes derivedPassword = new PasswordDeriveBytes(password, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = derivedPassword.GetBytes(keySize / 8);

            AesCng symmetricKey = new();
            symmetricKey.Mode = CipherMode.CFB;

            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int byteCount = 0;

            using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initialVectorBytes))
            {
                using (MemoryStream memStream = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
                    {
                        byteCount = cryptoStream.Read(plainTextBytes);
                        memStream.Close();
                        cryptoStream.Close();
                    }
                }
            }

            symmetricKey.Clear();
            return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
        }
    }

    
}
