using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Security.Cryptography;
using System.Text;


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

        public static void Log(this List<string> output, string text, bool replaceLast = false, bool addDate = true)
        {
            Console.Clear();
            if (addDate) text = $"[{DateTime.Now:dd.MM.yy HH:mm:ss]}: {text}";
            if (!replaceLast || output.Count == 0)
            {
                output.Add(text);
            }
            else
            {
                output[^1] = text;
            }
            Console.WriteLine(string.Join('\n', output), true);
        }

        public static string Work(Settings settings)
        {
            int tryout = 10;
            List<string> output = new();
            output.Log("* * * Отметка работы * * *\n", addDate: false);
            ChromeDriverService chromeservice = ChromeDriverService.CreateDefaultService();
            chromeservice.HideCommandPromptWindow = true;
            using (IWebDriver driver = new ChromeDriver(chromeservice))
            {
                string link = "http://elma:8000/Tasks/AllTasks/Incoming?FilterId=0";
                output.Log("Вход в Elma...");
                WebDriverWait wait = new(driver, TimeSpan.FromSeconds(10));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(ElementNotVisibleException));
                output.Log("Вход в Elma...[ОК]", true);
                driver.Navigate().GoToUrl(link);
                driver.FindElement(By.Id("login")).SendKeys(settings.Login);
                driver.FindElement(By.Id("password")).SendKeys(Decrypt(settings.Pass, "http://areopag") + Keys.Enter);

                // поиск задачи на распределение времени
                output.Log("Проверка задач...");
                for (int i = 0; i < tryout; i++)
                {
                    Thread.Sleep(500);
                    try
                    {
                        //wait.Until(wd => wd.FindElement(By.LinkText("Распределить рабочее время")).Displayed);
                        driver.FindElement(By.LinkText("Распределить рабочее время"));
                        output.Log($"Проверка задач...[ОК]", true);
                        break;
                    }
                    catch (NoSuchElementException)
                    {
                        output.Log($"Проверка задач...попытка {i + 1}", true);
                        if (i == 4) 
                        {
                            output.Log($"Проверка задач...[НЕУДАЧА]", true);
                            return "Нет задач на распределение рабочего времени.";
                        }
                    }
                }

                int tasksCount = driver.FindElements(By.LinkText("Распределить рабочее время")).Count;
                if (tasksCount == 0) {
                    return "Задач по распределению рабочего времени не обнаружено";
                }
                while (tasksCount > 0)
                {
                    if (driver.Url != link) driver.Navigate().GoToUrl(link);
                    output.Log("Поиск задачи...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.LinkText("Распределить рабочее время")).Displayed);
                            output.Log($"Поиск задачи...[ОК]", true);
                            break;
                        }
                        catch (NoSuchElementException)
                        {
                            output.Log($"Поиск задачи...попытка {i + 1}", true);
                            if (i == 4)
                            {
                                output.Log($"Поиск задачи...[НЕУДАЧА]", true);
                                return "Нет задач на распределение рабочего времени.";
                            }
                        }
                    }

                    // выбор задачи
                    output.Log("Выбор задачи...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            driver.FindElement(By.LinkText("Распределить рабочее время")).Click();
                            output.Log("Выбор задачи...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Выбор задачи...попытка {i + 1}", true);
                            if (i == 4)
                            {
                                output.Log($"Выбор задачи...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // открываем поиск
                    output.Log("Открытваем список...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.ClassName("combobox-icon")).Displayed);
                            driver.FindElement(By.ClassName("combobox-icon")).Click();
                            output.Log("Открытваем список...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Открытваем список...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Открытваем список...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // ищем
                    output.Log($"Ввод наименования обеденного процесса...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.XPath("//input[@placeholder='Что искать?']")).Displayed);
                            driver.FindElement(By.XPath("//input[@placeholder='Что искать?']")).SendKeys(settings.DinnerName + Keys.Enter);
                            output.Log("Ввод наименования обеденного процесса...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Ввод наименования обеденного процесса...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Ввод наименования обеденного процесса...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // тыкаем первый найденный вариант
                    output.Log($"Выбор первого найденного варианта...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Displayed);
                            driver.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Click();
                            output.Log($"Выбор первого найденного варианта...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Выбор первого найденного варианта...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Выбор первого найденного варианта...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // указываем время в минутах
                    output.Log($"Ввод времени обеда...");
                    var time = TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(settings.DinnerTime));
                    if (time.Hour >= 1)
                    {
                        for (int i = 0; i < tryout; i++)
                        {
                            Thread.Sleep(500);
                            try
                            {
                                //wait.Until(wd => wd.FindElement(By.Id("Entity_WorkedTime_hours")).Displayed);
                                driver.FindElement(By.Id("Entity_WorkedTime_hours")).SendKeys(time.Hour.ToString());
                                output.Log($"Ввод времени обеда...[ОК]", true);
                                break;
                            }
                            catch
                            {
                                output.Log($"Ввод времени обеда...попытка {i + 1}", true);
                                if (i == 4) {
                                    output.Log($"Ввод времени обеда...[НЕУДАЧА]", true);
                                    return "Не удалось.";
                                }
                            }
                        }
                    }
                    if (time.Minute >= 1)
                    {
                        for (int i = 0; i < tryout; i++)
                        {
                            Thread.Sleep(500);
                            try
                            {
                                //wait.Until(wd => wd.FindElement(By.Id("Entity_WorkedTime_minutes")).Displayed);
                                driver.FindElement(By.Id("Entity_WorkedTime_minutes")).SendKeys(time.Minute.ToString());
                                output.Log($"Ввод времени обеда...[ОК]", true);
                                break;
                            }
                            catch
                            {
                                output.Log($"Ввод времени обеда...попытка {i + 1}", true);
                                if (i == 4) {
                                    output.Log($"Ввод времени обеда...[НЕУДАЧА]", true);
                                    return "Не удалось.";
                                }
                            }
                        }
                    }

                    // добавляем
                    output.Log($"Подтверждаем обед...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Displayed);
                            driver.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Click();
                            output.Log($"Подтверждаем обед...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Подтверждаем обед...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Подтверждаем обед...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // открываем поиск
                    output.Log("Открытваем список...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.ClassName("combobox-icon")).Displayed);
                            driver.FindElement(By.ClassName("combobox-icon")).Click();
                            output.Log($"Открытваем список...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Открытваем список...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Открытваем список...[НЕУДАЧА] {i + 1}", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // ищем
                    output.Log($"Ввод наименования рабочего процесса...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.XPath("//input[@placeholder='Что искать?']")).Displayed);
                            driver.FindElement(By.XPath("//input[@placeholder='Что искать?']")).SendKeys(settings.WorkName + Keys.Enter);
                            output.Log($"Ввод наименования рабочего процесса...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Ввод наименования рабочего процесса...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Ввод наименования рабочего процесса...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // тыкаем первый найденный вариант
                    output.Log($"Выбор первого найденного варианта...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Displayed);
                            driver.FindElement(By.XPath("//span[contains(@id,\"EntityxCompanyProject\")]")).Click();
                            output.Log($"Выбор первого найденного варианта...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Выбор первого найденного варианта...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Выбор первого найденного варианта...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // указываем остаточное время
                    output.Log($"Ввод оставшегося времени работы...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.Id("Entity_WorkedTime_days")).Displayed);
                            driver.FindElement(By.Id("Entity_WorkedTime_days")).SendKeys("1");
                            output.Log($"Ввод оставшегося времени работы...[ОК]", true);
                            break;
                        }
                        catch
                        {

                            output.Log($"Ввод оставшегося времени работы...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Ввод оставшегося времени работы...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }


                    // добавляем
                    output.Log($"Подтверждаем работу...");
                    for (int i = 0; i < tryout; i++)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            //wait.Until(wd => wd.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Displayed);
                            driver.FindElement(By.XPath("//a[contains(@class,\"t-button\")]")).Click();
                            output.Log($"Подтверждаем работу...[ОК]", true);
                            break;
                        }
                        catch
                        {
                            output.Log($"Подтверждаем работу...попытка {i + 1}", true);
                            if (i == 4) {
                                output.Log($"Подтверждаем работу...[НЕУДАЧА]", true);
                                return "Не удалось.";
                            }
                        }
                    }

                    // выполнение
                    output.Log($"Подтверждаем выполнение задачи...");
                    if (settings.AutoConfim)
                    {
                        for (int i = 0; i < tryout; i++)
                        {
                            Thread.Sleep(500);
                            try
                            {
                                //wait.Until(wd => wd.FindElement(By.XPath("//input[contains(@value,\"Выполнено\")]")).Displayed);
                                driver.FindElement(By.XPath("//input[contains(@value,\"Выполнено\")]")).Click();
                                output.Log($"Подтверждаем выполнение задачи...[ОК]", true);
                                Thread.Sleep(3000);
                                break;
                            }
                            catch
                            {

                                output.Log($"Подтверждаем выполнение задачи...попытка {i + 1}", true);
                                if (i == 4) {
                                    output.Log($"Подтверждаем выполнение задачи...[НЕУДАЧА]", true);
                                    return "Не удалось.";
                                }
                            }
                        }
                    }
                    else
                    {
                        while (driver.FindElements(By.XPath("//input[contains(@value,\"Выполнено\")]")).Count >= 1)
                        {
                            Thread.Sleep(1000);
                            output.Log("\n* * * Ожидание ручного подтверждения (кнопка \"Выполнено\") * * *", true, false);
                        }
                    }
                    if (tasksCount > 1) output.Log($"[{DateTime.Now:dd.MM.yy HH:mm:ss}]: Запуск следующей задачи", true, false);
                    tasksCount--;
                }
                
                return "Завершено.";
            }
        }

        public static string Encrypt(string plainText, string password,
       string salt = "dece1ver", string hashAlgorithm = "SHA1",
       int passwordIterations = 2, string initialVector = "OFRna73m*aze01xY",
       int keySize = 256)
        {
            if (string.IsNullOrEmpty(plainText))
                {
                return "";
            }

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
                {
                return "";
            }

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
