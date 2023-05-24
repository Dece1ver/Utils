using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuaserSMD
{
    public partial class MainWindow : Form
    {
        private const string BasePath = "C:\\ProgramData\\dece1ver\\QuaserSMD";
        private static readonly string ConfigPath = Path.Combine(BasePath, "config.ini");
        private int timeOut = 5;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            var paths = new string[3];
            if (!File.Exists(ConfigPath))
            {
                if (!Directory.Exists(BasePath)) Directory.CreateDirectory(BasePath);
                File.WriteAllText(ConfigPath, "!Конфигурация Quaser SMD. Следующие 2 строки должны быть путями синхронизации.");
                Process.Start(ConfigPath);
                ShowNotification("Требуется настройка", "Создан и открыт файл конфигурации, необходимо его заполнить и заново запустить программу.", ToolTipIcon.Warning);
            }
            else
            {
                paths = File.ReadAllLines(ConfigPath).Skip(1).ToArray();
                if (paths.Length != 2 || !Directory.Exists(paths[0]) || !Directory.Exists(paths[1]))
                {
                    Process.Start(ConfigPath);
                    ShowNotification("Требуется настройка", "Открыт файл конфигурации, необходимо его корректно заполнить и заново запустить программу.", ToolTipIcon.Warning);
                }
                else
                {
                    var count = 0;
                    foreach (var file in Directory.EnumerateFiles(paths[0]))
                    {
                        var guessFile = Path.Combine(paths[1], Path.GetFileName(file));
                        if (!File.Exists(guessFile))
                        {
                            File.Copy(file, guessFile);
                            count++;
                        }
                    }
                    foreach (var file in Directory.EnumerateFiles(paths[1]))
                    {
                        var guessFile = Path.Combine(paths[0], Path.GetFileName(file));
                        if (!File.Exists(guessFile))
                        {
                            File.Copy(file, guessFile);
                            count++;
                        }
                    }

                    ShowNotification("Завершено.",
                        count == 0 ? "Копирование не требуется." : $"Скопировано файлов: {count}.");
                }
            }
            Thread.Sleep(timeOut * 1000);
            Close();
        }

        private void ShowNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.ShowBalloonTip(timeOut);
        }
    }
}
