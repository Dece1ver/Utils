using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using ClosedXML.Excel;
using System.Threading;
using System.Globalization;

namespace OrdersCounter
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            _ProgressBar.Visibility = Visibility.Collapsed;
        }

        private string _fileName = string.Empty;
        private DateTime _startDateTime;
        private DateTime _endDateTime;

        const string _goodwayName = "Goodway GS-1500";
        const string _230Name = "Hyundai L230A";
        const string _skt21_1Name = "Hyundai WIA SKT21 №105";
        const string _skt21_2Name = "Hyundai WIA SKT21 №104";
        const string _6300Name = "Hyundai XH6300";
        const string _200Name = "Mazak QTS200ML";
        const string _350Name = "Mazak QTS350";
        const string _integrexName = "Mazak Integrex i200";
        const string _5000Name = "Mazak Nexus 5000";
        const string _quaserName = "Quaser MV134";
        const string _victorName = "Victor A110";

        List<string> GoodwayOrders = new();
        List<string> _230Orders = new();
        List<string> Skt21_1Orders = new();
        List<string> Skt21_2Orders = new();
        List<string> _6300Orders = new();
        List<string> _200Orders = new();
        List<string> _350Orders = new();
        List<string> IntegrexOrders = new();
        List<string> _5000Orders = new();
        List<string> QuaserOrders = new();
        List<string> VictorOrders = new();

        private void SetSrcButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Книга с макросами (*.xlsm)|*.xlsm",
                DefaultExt = "xlsm"
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

        private void GetInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DateTime.TryParseExact(StartDate.Text.ToString(), "dd.MM.yyyy", null, DateTimeStyles.None, out var startDateTime))
            {
                MessageBox.Show("начальная дата введена некорректно.");
                return;
            };
            if (!DateTime.TryParseExact(EndDate.Text.ToString(), "dd.MM.yyyy", null, DateTimeStyles.None, out var endDateTime))
            {
                MessageBox.Show("конечная дата введена некорректно.");
                return;
            };
            _startDateTime = startDateTime;
            _endDateTime = endDateTime;
            var _ = CreateReport();
        }

        private async Task CreateReport()
        {

            await Task.Run(() => {

                try
                {
                    ShowProgressBar();
                    SetStatus("Открытие файла...");
                    using (var fs = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        var wb = new XLWorkbook(fs, new ClosedXML.Excel.LoadOptions() { RecalculateAllFormulas = false });
                        GoodwayOrders = new List<string>();
                        _230Orders = new List<string>();
                        Skt21_1Orders = new List<string>();
                        Skt21_2Orders = new List<string>();
                        _6300Orders = new List<string>();
                        _200Orders = new List<string>();
                        _350Orders = new List<string>();
                        IntegrexOrders = new List<string>();
                        _5000Orders = new List<string>();
                        QuaserOrders = new List<string>();
                        VictorOrders = new List<string>();

                        var cnt = 0;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            _ProgressBar.IsIndeterminate = false;
                            _ProgressBar.Value = cnt;
                            _ProgressBar.Maximum = wb.Worksheet(1).Rows().Count();
                        }));
                        foreach (var xlRow in wb.Worksheet(1).Rows().Skip(1))
                        {
                            if (!xlRow.Cell(1).Value.IsNumber) break;
                            Dispatcher.Invoke(new Action(() => 
                            {
                                _ProgressBar.Value = cnt;
                            }));
                            cnt++;
                            var partName = xlRow.Cell(10).Value.GetText();
                            Dispatcher.Invoke(new Action(() => { StatusTextBlock.Text = $"Чтение: [{(partName.Length > 66 ? partName[..66] : partName)}]"; }));
                            var order = xlRow.Cell(11).Value.GetText();
                            var dateTime = xlRow.Cell(6).Value.GetDateTime();
                            switch (xlRow.Cell(7).Value.GetText())
                            {
                                case _goodwayName:
                                    if (!GoodwayOrders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        GoodwayOrders.Add(order);
                                    };
                                    break;
                                case _230Name:
                                    if (!_230Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        _230Orders.Add(order);
                                    };
                                    break;
                                case _skt21_1Name:
                                    if (!Skt21_1Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        Skt21_1Orders.Add(order);
                                    };
                                    break;
                                case _skt21_2Name:
                                    if (!Skt21_2Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        Skt21_2Orders.Add(order);
                                    };
                                    break;
                                case _6300Name:
                                    if (!_6300Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        _6300Orders.Add(order);
                                    };
                                    break;
                                case _200Name:
                                    if (!_200Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        _200Orders.Add(order);
                                    };
                                    break;
                                case _350Name:
                                    if (!_350Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        _350Orders.Add(order);
                                    };
                                    break;
                                case _integrexName:
                                    if (!IntegrexOrders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        IntegrexOrders.Add(order);
                                    };
                                    break;
                                case _5000Name:
                                    if (!_5000Orders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        _5000Orders.Add(order);
                                    };
                                    break;
                                case _quaserName:
                                    if (!QuaserOrders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        QuaserOrders.Add(order);
                                    };
                                    break;
                                case _victorName:
                                    if (!VictorOrders.Contains(order) && dateTime >= _startDateTime && dateTime <= _endDateTime)
                                    {
                                        VictorOrders.Add(order);
                                    };
                                    break;
                                default:
                                    break;
                            }
                        }
                        SetStatus($"Завершено. Прочитано {cnt} записей.");
                        Dispatcher.Invoke(new Action(() => {

                            ResultTextBox.Text =
                            $"Количество выполненных М/Л за период с {StartDate.Text} по {EndDate.Text}:\n\n" +
                            $"{_goodwayName}: {GoodwayOrders.Count}\n" +
                            $"{_230Name}: {_230Orders.Count}\n" +
                            $"{_skt21_1Name}: {Skt21_1Orders.Count}\n" +
                            $"{_skt21_2Name}: {Skt21_2Orders.Count}\n" +
                            $"{_6300Name}: {_6300Orders.Count}\n" +
                            $"{_200Name}: {_200Orders.Count}\n" +
                            $"{_350Name}: {_350Orders.Count}\n" +
                            $"{_integrexName}: {IntegrexOrders.Count}\n" +
                            $"{_5000Name}: {_5000Orders.Count}\n" +
                            $"{_quaserName}: {QuaserOrders.Count}\n" +
                            $"{_victorName}: {VictorOrders.Count}\n";
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", $"Ошибочка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    CollapseProgressBar();
                }
            });
        }

        private void SaveInfoButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Текстовый файл (*.txt)|*.txt",
                DefaultExt = "txt"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var content = $"{ResultTextBox.Text}\n\nПодробнее:\n\n";
                    content += $"{_goodwayName}\n\n";
                    foreach (var order in GoodwayOrders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_230Name}\n";
                    foreach (var order in _230Orders)
                    {
                        content += $"\t{order}\n\n";
                    }

                    content += $"{_skt21_1Name}\n\n";
                    foreach (var order in Skt21_1Orders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_skt21_2Name}\n\n";
                    foreach (var order in Skt21_2Orders)
                    {
                        content += $"\t{order}\n";
                    }


                    content += $"{_6300Name}\n\n";
                    foreach (var order in _6300Orders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_200Name}\n\n";
                    foreach (var order in _200Orders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_350Name}\n\n";
                    foreach (var order in _350Orders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_integrexName}\n\n";
                    foreach (var order in IntegrexOrders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_5000Name}\n\n";
                    foreach (var order in _5000Orders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_quaserName}\n\n";
                    foreach (var order in QuaserOrders)
                    {
                        content += $"\t{order}\n";
                    }

                    content += $"{_victorName}\n\n";
                    foreach (var order in VictorOrders)
                    {
                        content += $"\t{order}\n";
                    }

                    File.WriteAllText(saveFileDialog.FileName, content);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", $"Ошибочка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                SetStatus("Выбор файла отменён");
            }
        }
    }
}
