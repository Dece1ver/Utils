using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Window = Utils.WinAPI.Windows.Window;

namespace NCToolTable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        bool autoclose;
        public MainWindow(bool autoclose)
        {
            InitializeComponent();
            this.autoclose = autoclose;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = Run();
        }

        private async Task Run()
        {
            await GetWindows(false);
            if (windowsLV.Items.Count > 0)
            {
                GetTable(GetFileName(windowsLV.Items[0]));
            }
            if (autoclose)
            {
                Close();
            }
            else
            {
                _ = GetWindows(true);
            }
        }

        private void GetTable(string filePath)
        {
            if (File.Exists(filePath))
            {
                var tools = new List<string>();
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (new Regex(@"T(\d+)", RegexOptions.Compiled).IsMatch(line) && !line.StartsWith('(') && line.Contains("("))
                    {
                        if (line.Contains("M6"))
                        {
                            var toolLine = "T" + line.Split('T', 2)[1] + line.Split("(")[1].Trim();
                            if (toolLine.Contains('X')) toolLine = toolLine.Split('X')[0];
                            if (toolLine.Contains('Y')) toolLine = toolLine.Split('Y')[0];
                            if (toolLine.Contains('Z')) toolLine = toolLine.Split('Z')[0];
                            if (toolLine.Contains('A')) toolLine = toolLine.Split('A')[0];
                            if (toolLine.Contains('F')) toolLine = toolLine.Split('F')[0];
                            if (toolLine.Contains('M')) toolLine = toolLine.Split('M')[0];
                            if (toolLine.Contains('G')) toolLine = toolLine.Split('G')[0];
                            var fLine = $"({toolLine,-3} - {line.Split("(")[1].Trim()}";
                            if (!tools.Contains(fLine)) tools.Add(fLine);
                        }
                        else
                        {
                            var fLine = "(" + line.Split("(")[1].Trim();
                            if (!tools.Contains(fLine)) tools.Add(fLine);
                        }
                    }
                }
                if (tools.Count > 0)
                {
                    Clipboard.SetText(string.Join(Environment.NewLine, tools));
                    statusTB.Text = "Таблица сформирована и скопирована в буфер обмена";
                    if (autoclose)
                    {
                        MessageBox.Show($"Таблица сформирована и скопирована в буфер обмена:\n{string.Join(Environment.NewLine, tools)}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                else
                {
                    statusTB.Text = "Инструмента не найдено";
                    if (autoclose)
                    {
                        MessageBox.Show("Инструмента не найдено", "Что-то не так", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                if (autoclose)
                {
                    Close();
                }
            }
            else
            {
                statusTB.Text = "Файла не существует";
                if (autoclose)
                {
                    MessageBox.Show("Не удалось прочитать файл открытый в CIMCO Edit. Скорее всего он не сохранен на компьютере.", "Что-то не так", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new();

            if (fileDialog.ShowDialog().Value)
            {
                GetTable(fileDialog.FileName);
            }
            else
            {
                MessageBox.Show("Выбор файла отменен.");
            }
            Close();
        }

        private async Task GetWindows(bool loop)
        {
            do
            {
                await Task.Delay(200);
                List<string> windows = new();
                foreach (var window in Window.Find(w => w.Text.Contains("CIMCO Edit")))
                {
                    windows.Add(window.Text);
                }
                windowsLV.ItemsSource = windows;
            } while (loop);
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (windowsLV.SelectedItem is null)
            {
                statusTB.Text = "Ничего не выбрано";
            }
            else
            {
                try
                {
                    GetTable(GetFileName(windowsLV.SelectedItem));
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибочка", MessageBoxButton.OK, MessageBoxImage.Error);
                    statusTB.Text = "Ошибка";
                }

            }
        }

        private string GetFileName(object item)
        {
            if (item.ToString().TrimEnd(']').EndsWith('*')) MessageBox.Show("Файл был изменен, но не сохранен. Будет прочитана последняя сохраненная версия.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            return item.ToString().Split('[', 2)[1].TrimEnd(']').TrimEnd('*').TrimEnd();
        }
    }
}
