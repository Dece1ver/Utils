using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
using System.Windows.Threading;
using System.Xml.Linq;

namespace NCProgramStat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        const string NotSended = "Не помещена в архив (не скинута)";
        private string _fileName = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ProgramInfo> _NotSendedPrograms;
        public ObservableCollection<ProgramInfo> NotSendedPrograms
        {
            get { return _NotSendedPrograms; }
            set {
                _NotSendedPrograms = value;
                OnPropertyChanged(nameof(NotSendedPrograms));
            }

        }


        public MainWindow()
        {
            InitializeComponent();
            _ProgressBar.Visibility = Visibility.Collapsed;
            _NotSendedPrograms = new();
        }
        

        private void SetSrcButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Книга Excel (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _fileName = openFileDialog.FileName;
                SrcTextBox.Text = _fileName;
                SetStatus($"Выбран файл: \"{_fileName}\"");
            }
            else
            {
                SetStatus("Выбор файла отменён");
            }
        }

        void SetStatus(string message) => Dispatcher.Invoke(new Action(() => { StatusTextBlock.Text = message; }));
        void ShowProgressBar() => Dispatcher.Invoke(new Action(() =>
        {
            _ProgressBar.IsIndeterminate = true;
            _ProgressBar.Visibility = Visibility.Visible;
        }));
        void CollapseProgressBar() => Dispatcher.Invoke(new Action(() => { _ProgressBar.Visibility = Visibility.Collapsed; }));

        private async void GetInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(_fileName))
            {
                MessageBox.Show("Файл не существует.");
                return;
            }

            try
            {
                Dispatcher.Invoke(() =>
                {
                    NotSendedPrograms.Clear();
                    ShowProgressBar();
                    SetStatus("Открытие файла...");
                });
                var progress = new Progress<int>(value =>
                {
                    _ProgressBar.Value = value;
                });
                var totalPrograms = 0;
                await Task.Run(() =>
                {
                    using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, true))
                    {
                        var wb = new XLWorkbook(fs, new ClosedXML.Excel.LoadOptions { RecalculateAllFormulas = false });
                        var rows = wb.Worksheet(1).Rows().Skip(1).Where(r => r.Cell(1).Value.IsDateTime).ToList();

                        Dispatcher.Invoke(() =>
                        {
                            _ProgressBar.Maximum = rows.Count;
                            _ProgressBar.IsIndeterminate = false;
                        });

                        var disqualifiedKeys = new HashSet<(string PartName, int Setup, string Machine)>();
                        var filteredPrograms = new List<ProgramInfo>();

                        for (int i = 0; i < rows.Count; i++)
                        {
                            var xlRow = rows[i];
                            var date = xlRow.Cell(1).Value.GetDateTime();
                            var machine = xlRow.Cell(2).Value.GetText();
                            var shift = xlRow.Cell(3).Value.GetText();
                            var @operator = xlRow.Cell(4).Value.GetText();
                            var partName = xlRow.Cell(5).Value.GetText();
                            var setup = (int)xlRow.Cell(8).Value.GetNumber();
                            var status = xlRow.Cell(10).Value.IsText ? xlRow.Cell(10)?.Value.GetText() : "";

                            var key = (partName, setup, machine);

                            if (status != NotSended)
                            {
                                disqualifiedKeys.Add(key);
                                filteredPrograms.RemoveAll(p => p.PartName == partName && p.Setup == setup && p.Machine == machine);
                            }
                            else if (!disqualifiedKeys.Contains(key) && !filteredPrograms.Any(p => p.PartName == partName && p.Setup == setup && p.Machine == machine))
                            {
                                filteredPrograms.Add(new ProgramInfo(date, machine, shift, @operator, partName, setup, status));
                            }

                            (progress as IProgress<int>)?.Report(i);
                        }
                        totalPrograms = rows.Count;
                        Dispatcher.Invoke(() =>
                        {
                            NotSendedPrograms.Clear();
                            foreach (var pi in filteredPrograms)
                            {
                                NotSendedPrograms.Add(pi);
                            }
                        });
                    }
                });

                SetStatus($"Выполнено. Не скинуто программ: {NotSendedPrograms.Count} из {totalPrograms}.");
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                    SetStatus("");
                });
            }
            finally
            {
                CollapseProgressBar();
            }
        }

        private void SaveInfoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Лист1");
                ws.Range(1, 1, 1, 7).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(1, 1).Value = "Дата";
                ws.Column(1).Width = 10;
                ws.Cell(1, 2).Value = "Станок";
                ws.Column(2).Width = 25;
                ws.Cell(1, 3).Value = "Смена";
                ws.Column(3).Width = 7;
                ws.Cell(1, 4).Value = "Оператор";
                ws.Column(4).Width = 35;
                ws.Cell(1, 5).Value = "Деталь";
                ws.Column(5).Width = 70;
                ws.Cell(1, 6).Value = "Установка";
                ws.Column(6).Width = 10;
                ws.Cell(1, 7).Value = "Статус";
                ws.Column(7).Width = 35;

                wb.Author = Environment.UserName;
                var data = ws.Cell(2, 1).InsertData(NotSendedPrograms.AsEnumerable());
                ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                var row = ws.LastRowUsed().RowNumber() + 2;

                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "Таблица Excel(*.xlsx)|*.xlsx";
                saveFileDialog.DefaultExt = "xslx";
                if (saveFileDialog.ShowDialog() == true)
                {

                    wb.SaveAs(saveFileDialog.FileName);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetStatus("");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            var handlers = PropertyChanged;
            if (handlers is null) return;

            var invokationList = handlers.GetInvocationList();
            var args = new PropertyChangedEventArgs(PropertyName);

            foreach (var action in invokationList)
            {
                if (action.Target is DispatcherObject dispatcherObject)
                {
                    dispatcherObject.Dispatcher.Invoke(action, this, args);
                }
                else
                {
                    action.DynamicInvoke(this, args);
                }
            }
        }
    }
}
