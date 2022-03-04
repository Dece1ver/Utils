using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace VictorXML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string rawData;
        List<Part> parts;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            saveButton.Visibility = Visibility.Collapsed;
            expandButton.Visibility = Visibility.Collapsed;
            hideButton.Visibility = Visibility.Visible;
        }

        void SetStatus(string message)
        {
            statusTextBlock.Text = message;
        }

        private void readButton_Click(object sender, RoutedEventArgs e)
        {
            parts = new();
            if (sourceTextBlock.Text.StartsWith('<') || (sourceTextBlock.Text.Contains('<') && !sourceTextBlock.Text.StartsWith('<')))
            {
                if (!sourceTextBlock.Text.StartsWith('<')) sourceTextBlock.Text = '<' + sourceTextBlock.Text.Split('<', 2)[1];
                readButton.IsEnabled = false;
                SetStatus("Чтение содержимого");
                try
                {
                    XmlDocument xmlDoc = new();
                    rawData = sourceTextBlock.Text;
                    sourceTextBlock.Clear();
                    xmlDoc.LoadXml(rawData);

                    var xmlElement = xmlDoc.DocumentElement;
                    if (xmlElement != null)
                    {
                        foreach (XmlNode xmlNode in xmlElement.ChildNodes)
                        {
                            
                            if (xmlNode.Name == "MainSegment")
                            {
                                Part part;
                                foreach (XmlNode mainSegmentNode in xmlNode.ChildNodes[0].ChildNodes)
                                {
                                    if (mainSegmentNode.ChildNodes[0].InnerText == "Сборочные единицы")
                                    {
                                        foreach (XmlNode usedObjectsNode in mainSegmentNode.ChildNodes)
                                        {
                                            if (usedObjectsNode.Name == "SpecRow")
                                            {
                                                part = new();
                                                foreach (XmlNode usedPartsNode in usedObjectsNode.ChildNodes)
                                                {
                                                    if (usedPartsNode.Name == "Number") part.Number = usedPartsNode.InnerText;
                                                    if (usedPartsNode.Name == "Name") part.Name = usedPartsNode.InnerText;
                                                    if (usedPartsNode.Name == "Qty") part.Quantity = int.Parse(usedPartsNode.InnerText);
                                                }
                                                parts.Add(part);
                                            }
                                        }
                                    }
                                    if (mainSegmentNode.ChildNodes[0].InnerText == "Детали")
                                    {
                                        foreach (XmlNode usedObjectsNode in mainSegmentNode.ChildNodes)
                                        {
                                            if (usedObjectsNode.Name == "SpecRow")
                                            {
                                                part = new();
                                                foreach (XmlNode usedPartsNode in usedObjectsNode.ChildNodes)
                                                {
                                                    if (usedPartsNode.Name == "Number") part.Number = usedPartsNode.InnerText;
                                                    if (usedPartsNode.Name == "Name") part.Name = usedPartsNode.InnerText;
                                                    if (usedPartsNode.Name == "Qty") part.Quantity = int.Parse(usedPartsNode.InnerText);
                                                }
                                                parts.Add(part);
                                            }
                                        }
                                    }
                                    if (mainSegmentNode.ChildNodes[0].InnerText == "Стандартные изделия")
                                    {
                                        foreach (XmlNode usedObjectsNode in mainSegmentNode.ChildNodes)
                                        {
                                            if (usedObjectsNode.Name == "SpecRow")
                                            {
                                                part = new();
                                                foreach (XmlNode usedPartsNode in usedObjectsNode.ChildNodes)
                                                {
                                                    if (usedPartsNode.Name == "Number") part.Number = string.Empty;
                                                    if (usedPartsNode.Name == "Name") part.Name = usedPartsNode.InnerText;
                                                    if (usedPartsNode.Name == "Qty") part.Quantity = int.Parse(usedPartsNode.InnerText);
                                                }
                                                parts.Add(part);
                                            }
                                        }
                                    }
                                    if (mainSegmentNode.ChildNodes[0].InnerText == "Прочие изделия")
                                    {
                                        foreach (XmlNode usedObjectsNode in mainSegmentNode.ChildNodes)
                                        {
                                            if (usedObjectsNode.Name == "SpecRow")
                                            {
                                                part = new();
                                                foreach (XmlNode usedPartsNode in usedObjectsNode.ChildNodes)
                                                {
                                                    if (usedPartsNode.Name == "Number") part.Number = string.Empty;
                                                    if (usedPartsNode.Name == "Name") part.Name = usedPartsNode.InnerText;
                                                    if (usedPartsNode.Name == "Qty") part.Quantity = int.Parse(usedPartsNode.InnerText);
                                                }
                                                parts.Add(part);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    HideLeftPanel();
                    saveButton.Visibility = Visibility.Visible;
                    resultDataGrid.ItemsSource = parts;
                    SetStatus($"Содержимое прочитано, позиций: {parts.Count}. Записать в Excel таблицу можно тыкнув на дискетку.");
                }
                catch (Exception ex)
                {
                    SetStatus("Некорректное содержимое");
                    _ = MessageBox.Show($"Если ошибка не из-за того, что ты вписал какую-то хрень, то сообщи мне.\n" +
                        $"\nПодробности ошибки:\n{ex.Message}", "Ошибочка вышла.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    readButton.IsEnabled = true;
                }
            }
            else
            {
                SetStatus("Некорректное содержимое");
            }
            
            //resultDataGrid.ItemsSource = ds.Tables[0].DefaultView;
        }

        void HideLeftPanel()
        {
            expandButton.Visibility = Visibility.Visible;
            hideButton.Visibility = Visibility.Collapsed;
            sourceDockPanel.Visibility = Visibility.Collapsed;
            sourceTextBlock.Clear();
        }

        void ExpandLeftPanel()
        {
            expandButton.Visibility = Visibility.Collapsed;
            hideButton.Visibility = Visibility.Visible;
            sourceDockPanel.Visibility = Visibility.Visible;
            sourceTextBlock.Text = rawData;
        }

        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            ExpandLeftPanel();
        }

        private void hideButton_Click(object sender, RoutedEventArgs e)
        {
            HideLeftPanel();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            int row = 1;
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");

            worksheet.Cell("A" + row).Value = "Номенклатура";
            worksheet.Cell("B" + row).Value = "Количество";
            row++;
            foreach (Part part in parts)
            {
                worksheet.Cell("A" + row).Value = $"{part.Name.Trim()} {part.Number.Trim()}";
                worksheet.Cell("B" + row).Value = part.Quantity;
                row++;
            }
            _ = worksheet.Columns().AdjustToContents();
            workbook.Author = "dece1ver";

            SaveFileDialog saveFileDialog = new ();
            saveFileDialog.Filter = "Таблица Excel(*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "xslx";
            if (saveFileDialog.ShowDialog() == true)
            {
                
                workbook.SaveAs(saveFileDialog.FileName);
                SetStatus($"Файл сохранен: \"{saveFileDialog.FileName}\"");

                if (MessageBox.Show("Файл сохранен. Открыть?", "Вопросик", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Process process = new() { StartInfo = new ProcessStartInfo() { UseShellExecute = true } };
                    _ = Process.Start(new ProcessStartInfo() { UseShellExecute = true, FileName = saveFileDialog.FileName });
                    SetStatus($"Открытие файла: \"{saveFileDialog.FileName}\"");
                }
            }
            else
            {
                SetStatus("Сохранение отменено");
            }
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Файл XML(*.xml)|*.xml";
            openFileDialog.DefaultExt = "xslx";
            if (openFileDialog.ShowDialog() == true)
            {
                sourceTextBlock.Text = File.ReadAllText(openFileDialog.FileName);
                SetStatus($"Открыт файл: \"{openFileDialog.FileName}\", для чтения содержимого нажмите \">\"");
            }
            else
            {
                SetStatus("Выбор файла отменён");
            }
        }
    }
}
