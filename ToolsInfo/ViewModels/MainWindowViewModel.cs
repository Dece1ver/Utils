using ToolsInfo.Infrastructure;
using ToolsInfo.Infrastructure.Commands;
using ToolsInfo.ViewModels.Base;
using ToolsInfo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace ToolsInfo.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        Thread getFilesThread;

        private string _Status = string.Empty;
        /// <summary>
        /// Статус
        /// </summary>
        public string Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }

        private bool _ModeComboboxEnabled = true;

        public bool ModeComboboxEnabled
        {
            get => _ModeComboboxEnabled;
            set => Set(ref _ModeComboboxEnabled, value);
        }


        private bool _BrowseButtonEnabled = true;

        public bool BrowseButtonEnabled
        {
            get => _BrowseButtonEnabled;
            set => Set(ref _BrowseButtonEnabled, value);
        }

        private bool _SaveButtonEnabled;

        public bool SaveButtonEnabled
        {
            get => _SaveButtonEnabled;
            set => Set(ref _SaveButtonEnabled, value);
        }

        private bool _FindButtonEnabled;

        public bool FindButtonEnabled
        {
            get => _FindButtonEnabled;
            set => Set(ref _FindButtonEnabled, value);
        }

        private string _FindButtonText = "Сформировать";

        public string FindButtonText
        {
            get => _FindButtonText;
            set => Set(ref _FindButtonText, value);
        }


        private bool _getToolsThreadFlag = false;

        public bool GetToolsThreadFlag
        {
            get => _getToolsThreadFlag;
            set => Set(ref _getToolsThreadFlag, value);
        }

        private double _Progress;
        /// <summary>
        /// Значение прогрессбара
        /// </summary>
        public double Progress
        {
            get => _Progress;
            set => Set(ref _Progress, value);
        }

        private double _ProgressMaxValue;
        /// <summary>
        /// Максимальное значение прогрессбара
        /// </summary>
        public double ProgressMaxValue
        {
            get => _ProgressMaxValue;
            set => Set(ref _ProgressMaxValue, value);
        }

        private Visibility _ProgressBarVisibility = Visibility.Collapsed;

        public Visibility ProgressBarVisibility
        {
            get => _ProgressBarVisibility;
            set => Set(ref _ProgressBarVisibility, value);
        }


        private string _TargetPath;

        public string TargetPath
        {
            get => _TargetPath;
            set => Set(ref _TargetPath, value);
        }

        /// <summary>
        /// Список файлов
        /// </summary>
        private List<string> _Files;

        public List<string> Files
        {
            get => _Files;
            set => Set(ref _Files, value);
        }

        public int? FilesCount => Files?.Count;

        /// <summary>
        /// Список инструмента
        /// </summary>
        private ObservableCollection<Models.ToolsInfo> _tools;

        public ObservableCollection<Models.ToolsInfo> Tools
        {
            get => _tools;
            set => Set(ref _tools, value);
        }

        public int? DetailsCount => Tools?.Count;

        //public string DetailsText {
        //    get
        //    {
        //        List<string> result = new List<string>();
        //        foreach (Models.ToolsInfo tool in Tools is null ? new List<Models.ToolsInfo>() : Tools)
        //        {
        //            result.Add($"{tool.ToolName} : {tool.Count}");
        //        }
        //        result.Sort();
        //        return string.Join("\n", result);
        //    }
        //}


        #region Команды


        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        #endregion


        #region SetPathCommand
        public ICommand SetPathCommand { get; }
        private void OnSetPathCommandExecuted(object p)
        {
            FolderBrowserDialog folderDialog = new();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                TargetPath = folderDialog.SelectedPath;
                FindButtonEnabled = true;
            }
            else
            {
                Status = "Выбор отменен";
            }
        }
        private bool CanSetPathCommandExecute(object p) => true;
        #endregion


        #region FindToolsCommand
        public ICommand FindToolsCommand { get; }
        private void OnFindToolsCommandExecuted(object p)
        {
            if (!GetToolsThreadFlag)
            {
                GetToolsThreadFlag = true;
                getFilesThread = new(() => FindTools(TargetPath));
                getFilesThread.IsBackground = true;
                getFilesThread.Start();
            }
            else
            {
                GetToolsThreadFlag = false;
            }
        }
        private bool CanFindToolsCommandExecute(object p) => true;
        #endregion


        #region SaveDetailsToFileCommand
        public ICommand SaveDetailsToFileCommand { get; }
        private void OnSaveDetailsToFileCommandExecuted(object p)
        {
            if (Tools.Count <= 0) return;
            int row = 1;
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Лист1");

            worksheet.Cell("A" + row).Value = "Количество";
            worksheet.Cell("B" + row).Value = "Инструмент";
            row++;
            foreach (Models.ToolsInfo tool in Tools)
            {
                worksheet.Cell("A" + row).Value = $"{tool.Count}";
                worksheet.Cell("B" + row).Value = tool.ToolName.Trim();
                row++;
            }
            _ = worksheet.Columns().AdjustToContents();
            workbook.Author = "dece1ver";

            SaveFileDialog saveFileDialog = new ();
            saveFileDialog.Filter = "Таблица Excel(*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "xslx";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                
                workbook.SaveAs(saveFileDialog.FileName);
                Status = $"Файл сохранен: \"{saveFileDialog.FileName}\"";

                if (MessageBox.Show("Файл сохранен. Открыть?", "Вопросик", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Process process = new() { StartInfo = new ProcessStartInfo() { UseShellExecute = true } };
                    _ = Process.Start(new ProcessStartInfo() { UseShellExecute = true, FileName = saveFileDialog.FileName });
                    Status = $"Открытие файла: \"{saveFileDialog.FileName}\"";
                }
            }
            else
            {
                Status = "Сохранение отменено";
            }
        }
        private bool CanSaveDetailsToFileCommandExecute(object p) => true;
        #endregion

        #endregion



        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            SetPathCommand = new LambdaCommand(OnSetPathCommandExecuted, CanSetPathCommandExecute);
            FindToolsCommand = new LambdaCommand(OnFindToolsCommandExecuted, CanFindToolsCommandExecute);
            SaveDetailsToFileCommand = new LambdaCommand(OnSaveDetailsToFileCommandExecuted, CanSaveDetailsToFileCommandExecute);
        }


        private void GetFiles(string path)
        {

            try
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    if (!GetToolsThreadFlag) break;
                    GetFiles(folder);
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    if (!GetToolsThreadFlag) break;
                    Files.Add(file);
                    OnPropertyChanged(nameof(FilesCount));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.ToString());
            }
        }

        private void FindTools(string path)
        {
            FindButtonText = "Остановить";
            Files = new();
            Tools = new();
            Status = "Подсчет файлов";
            BrowseButtonEnabled = false;
            SaveButtonEnabled = false;
            ModeComboboxEnabled = false;
            OnPropertyChanged(nameof(FilesCount));
            OnPropertyChanged(nameof(DetailsCount));
            OnPropertyChanged(nameof(Tools));
            ProgressBarVisibility = Visibility.Collapsed;
            GetFiles(path);
            ProgressMaxValue = (double)FilesCount;
            Progress = 0;
            ProgressBarVisibility = Visibility.Visible;
            foreach (var file in Files)
            {
                Status = "Проверка файлов на наличие УП";
                if (!GetToolsThreadFlag) 
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                    break;
                }
                Progress++;
                try
                {
                    bool first = true;
                    foreach (var line in File.ReadLines(file))
                    {
                        var added = false;
                        if (first && line.Trim() != "%") break;
                        first = false;
                        if (!line.StartsWith('T') || !line.Contains("M6") || !line.Contains('(')) continue;
                        var toolInfo = line.Split('(')[1].Split(')')[0];
                        if (Tools.Count == 0)
                        {

                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                Tools.Add(new Models.ToolsInfo(toolInfo, 1));
                            });
                            continue;
                        }
                        foreach (Models.ToolsInfo tool in Tools)
                        {
                            if (!toolInfo.Equals(tool.ToolName)) continue;
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                tool.Count += 1;
                            });
                            
                            added = true;
                            break;
                        }

                        if (!added)
                        {
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                Tools.Add(new Models.ToolsInfo(toolInfo, 1));
                            });
                        }
                    }
                    OnPropertyChanged(nameof(DetailsCount));
                    OnPropertyChanged(nameof(Tools));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.ToString());
                }
            }
            Status = "Завершено";
            if(DetailsCount > 0) SaveButtonEnabled = true;
            BrowseButtonEnabled = true;
            ModeComboboxEnabled = true;
            ProgressBarVisibility = Visibility.Collapsed;
            FindButtonText = "Сформировать";
            GetToolsThreadFlag = false;
            
        }
    }
}
