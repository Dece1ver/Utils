using NCAnalyzer.Infrastructure;
using NCAnalyzer.Infrastructure.Commands;
using NCAnalyzer.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Forms;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace NCAnalyzer.ViewModels
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


        private bool _analyzeFilesThreadFlag = false;

        public bool AnalyzeFilesThreadFlag
        {
            get => _analyzeFilesThreadFlag;
            set => Set(ref _analyzeFilesThreadFlag, value);
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

        private double _NcFiles;
        /// <summary>
        /// УП
        /// </summary>
        public double NcFiles
        {
            get => _NcFiles;
            set => Set(ref _NcFiles, value);
        }

        /// <summary>
        /// УП с проблемами
        /// </summary>
        private double _BadNcFiles;
        public double BadNcFiles
        {
            get => _BadNcFiles;
            set => Set(ref _BadNcFiles, value);
        }

        /// <summary>
        /// Отчет
        /// </summary>
        private string _Report;

        public string Report
        {
            get => _Report;
            set => Set(ref _Report, value);
        }
      

        #region Команды


        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private static void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        private static bool CanCloseApplicationCommandExecute(object p) => true;
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
        private static bool CanSetPathCommandExecute(object p) => true;
        #endregion


        #region AnalyzeFilesCommand
        public ICommand AnalyzeFilesCommand { get; }
        private void OnAnalyzeFilesCommandExecuted(object p)
        {
            if (!AnalyzeFilesThreadFlag)
            {
                AnalyzeFilesThreadFlag = true;
                getFilesThread = new Thread(() => AnalyzePrograms(TargetPath)) {IsBackground = true};
                getFilesThread.Start();
            }
            else
            {
                AnalyzeFilesThreadFlag = false;
            }
        }
        private static bool CanAnalyzeFilesCommandExecute(object p) => true;
        #endregion


        #region SaveReportCommand
        public ICommand SaveReportCommand { get; }
        private void OnSaveReportCommandExecuted(object p)
        {
            if (string.IsNullOrEmpty(Report)) return;
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            saveFileDialog.FileName = "Анализы " + Path.GetFileName(TargetPath) + ".txt";
            if (saveFileDialog.ShowDialog() != true) return;
            File.WriteAllText(saveFileDialog.FileName, Report);
            Status = $"Отчет записан в файл \"{saveFileDialog.FileName}\"";
        }
        private static bool CanSaveReportCommandExecute(object p) => true;
        #endregion

        #endregion



        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            SetPathCommand = new LambdaCommand(OnSetPathCommandExecuted, CanSetPathCommandExecute);
            AnalyzeFilesCommand = new LambdaCommand(OnAnalyzeFilesCommandExecuted, CanAnalyzeFilesCommandExecute);
            SaveReportCommand = new LambdaCommand(OnSaveReportCommandExecuted, CanSaveReportCommandExecute);
        }

        private void AnalyzePrograms(string path)
        {
            
            FindButtonText = "Остановить";
            Files = new List<string>();
            NcFiles = 0;
            BadNcFiles = 0;
            Report = string.Empty;
            Status = "Подсчет файлов";
            BrowseButtonEnabled = false;
            SaveButtonEnabled = false;
            Progress = 0;
            OnPropertyChanged(nameof(FilesCount));
            OnPropertyChanged(nameof(Report));
            GetFiles(path);
            ProgressMaxValue = (double)FilesCount;
            OnPropertyChanged(nameof(Progress));
            ProgressBarVisibility = Visibility.Visible;
            string reportHeader = "* Результаты анализов\n" +
                                  $"* Дата: {DateTime.Now:dd.MM.yy - HH:mm:ss}\n" +
                                  $"* Путь: {TargetPath}\n";
            Report += reportHeader + '\n';
            foreach (var file in Files)
            {
                Status = $"Проверка: {Path.GetFileName(file)}";
                if (!AnalyzeFilesThreadFlag) 
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                    break;
                }
                try
                {
                    var lines = File.ReadLines(file).Take(1).ToList();
                    if (lines.Count is 0 || lines[0] != "%")
                    {
                        Progress++;
                        continue;
                    }

                    NcFiles++;
                    AnalyzeProgram(file, 
                        out var programType, 
                        out var coordinates, 
                        out var warningsH, 
                        out var warningsD,
                        out var warningsBracket,
                        out var warningsDots,
                        out var warningsEmptyAddress,
                        out var warningsCoolant,
                        out var warningStartPercent,
                        out var warningEndPercent,
                        out var warningEndProgram);
                    if (warningsH.Count == 0 &&
                        warningsD.Count == 0 &&
                        warningsBracket.Count == 0 &&
                        warningsDots.Count == 0 &&
                        warningsEmptyAddress.Count == 0 &&
                        warningsCoolant.Count == 0 &&
                        !warningStartPercent &&
                        !warningEndPercent &&
                        !warningEndProgram)
                    {
                        Progress++;
                        continue;
                    }

                    BadNcFiles++;
                    Report += $"({BadNcFiles})\nПроблемы в УП \"{file}\":\n";
                    switch (warningStartPercent)
                    {
                        case true when !warningEndPercent:
                            Report += $"  Отсутствует процент в начале УП;\n";
                            break;
                        case false when warningEndPercent:
                            Report += $"  Отсутствует процент в конце УП;\n";
                            break;
                        case true when warningEndPercent:
                            Report += $"  Отсутствуют проценты в начале и в конце УП;\n";
                            break;
                    }

                    if (warningEndProgram)
                    {
                        Report += $"  Отсутствует команда завершения программы: ( M30 / M99 );\n";
                    }
                    if (warningsH.Count > 0)
                    {
                        Report += $"  Несовпадений корректора на длину: {warningsH.Count}\n{string.Join("\n", warningsH)}\n";
                    }
                    if (warningsD.Count > 0)
                    {
                        Report += $"  Несовпадений корректора на радиус: {warningsD.Count}\n{string.Join("\n", warningsD)}\n";
                    }
                    if (warningsBracket.Count > 0)
                    {
                        Report += $"  Несовпадений скобок: {warningsBracket.Count}\n{string.Join("\n", warningsBracket)}\n";
                    }
                    if (warningsDots.Count > 0)
                    {
                        Report += $"  Лишние точки: {warningsDots.Count}\n{string.Join("\n", warningsDots)}\n";
                    }
                    if (warningsEmptyAddress.Count > 0)
                    {
                        Report += $"  Пустые адреса: {warningsEmptyAddress.Count}\n{string.Join("\n", warningsEmptyAddress)}\n";
                    }
                    if (warningsCoolant.Count > 0)
                    {
                        Report += $"  Несовпадений СОЖ: {warningsCoolant.Count}\n{string.Join("\n", warningsCoolant)}\n";
                    }

                    Report += '\n';
                    Progress++;
                    OnPropertyChanged(nameof(Report));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.ToString());
                }
            }
            Status = AnalyzeFilesThreadFlag ? "Завершено" : "Прервано";
            if (AnalyzeFilesThreadFlag) ProgressBarVisibility = Visibility.Collapsed;
            if(!string.IsNullOrEmpty(Report) && Report != reportHeader) SaveButtonEnabled = true;
            Report = Report.Replace(reportHeader, reportHeader + 
                                         $"* Файлов: {FilesCount}\n" + 
                                         $"* УП всего: {NcFiles}\n" + 
                                         $"* УП с проблемами: {BadNcFiles}\n");
            BrowseButtonEnabled = true;
            FindButtonText = "Анализ";
            AnalyzeFilesThreadFlag = false;
        }

        private void GetFiles(string path)
        {

            try
            {
                foreach (var folder in Directory.GetDirectories(path))
                {
                    if (!AnalyzeFilesThreadFlag) break;
                    GetFiles(folder);
                }
                foreach (var file in Directory.GetFiles(path))
                {
                    if (!AnalyzeFilesThreadFlag) break;
                    Files.Add(file);
                    OnPropertyChanged(nameof(FilesCount));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.ToString());
            }
        }

        public void AnalyzeProgram(
            string programPath, 
            out string caption, 
            out string coordinates, 
            out List<string> warningsH, 
            out List<string> warningsD, 
            out List<string> warningsBracket, 
            out List<string> warningsDots,
            out List<string> warningsEmptyAddress,
            out List<string> warningsCoolant,
            out bool warningStartPercent,
            out bool warningEndPercent,
            out bool warningEndProgram)
        {
            //Stopwatch sw = Stopwatch.StartNew();
            warningsH = new List<string>();            // корректор на длину
            warningsD = new List<string>();            // корректор на радиус
            warningsBracket = new List<string>();      // скобки
            warningsDots = new List<string>();         // точки
            warningsEmptyAddress = new List<string>(); // пустые адреса
            warningsCoolant = new List<string>();      // СОЖ
            warningStartPercent = false;               // процент в начале
            warningEndPercent = false;                 // процент в конце
            warningEndProgram = true;                  // процент в конце
            var millProgram = false;
            var lines = File.ReadLines(programPath).ToImmutableList();
            List<string> coordinateSystems = new();
            var currentToolNo = 0;
            var currentToolComment = string.Empty;
            var currentH = 0;
            var currentD = 0;

            var indent = new string(' ', 5);

            // проценты
            if (!lines.First().Trim().Equals("%")) warningStartPercent = true;
            if (!lines.Last().Trim().Equals("%")) warningEndPercent = true;
            var fString = "D" + lines.Count.ToString().Length;
            double i = 0;
            foreach (var line in lines)
            {
                i++;
                var percent = i / lines.Count * 100;
                if (percent > 100) percent = 100;
                Status = $"Проверка: {Path.GetFileName(programPath)} [{Math.Round(percent)}%]";

                if (!AnalyzeFilesThreadFlag) 
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                    warningEndProgram = false;
                    break;
                }
                if (line.Trim().Equals("%")) continue;
                if (line.StartsWith('<'))
                {
                    if (line.Count(c => c is '<') != line.Count(c => c is '>'))
                    {
                        warningsBracket.Add($"[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line}");
                    }
                    continue;
                }

                var lineWithoutParenthesis = line.Trim();
                lineWithoutParenthesis = new Regex(@"[(][^)]+[)]", RegexOptions.Compiled).Matches(line)
                    .Aggregate(lineWithoutParenthesis, (current, match) => current.Replace(match.Value, string.Empty));
                if (string.IsNullOrEmpty(lineWithoutParenthesis)) continue;

                // системы координат
                if (line.Contains("G54") && !coordinateSystems.Contains("G54") && !line.Contains("G54.1") && !line.Contains("G54P"))
                {
                    coordinateSystems.Add("G54");
                }
                if (line.Contains("G55") && !coordinateSystems.Contains("G55"))
                {
                    coordinateSystems.Add("G55");
                }
                if (line.Contains("G56") && !coordinateSystems.Contains("G56"))
                {
                    coordinateSystems.Add("G56");
                }
                if (line.Contains("G57") && !coordinateSystems.Contains("G57"))
                {
                    coordinateSystems.Add("G57");
                }
                if (line.Contains("G58") && !coordinateSystems.Contains("G58"))
                {
                    coordinateSystems.Add("G58");
                }
                if (line.Contains("G59") && !coordinateSystems.Contains("G59"))
                {
                    coordinateSystems.Add("G59");
                }

                if (new Regex(@"G54[.]1P\d{1,3}", RegexOptions.Compiled) is { } regex541 && regex541.IsMatch(line))
                {
                    var match = regex541.Match(line);
                    if (!coordinateSystems.Contains(match.Value)) coordinateSystems.Add(match.Value);
                    
                }
                if (new Regex(@"G54P\d{1,3}", RegexOptions.Compiled) is { } regex54 && regex54.IsMatch(line))
                {
                    var match = regex54.Match(line);
                    if (!coordinateSystems.Contains(match.Value)) coordinateSystems.Add(match.Value);
                    
                }
                
                // несовпадения скобок
                if (line.Count(c => c is '(') != line.Count(c => c is ')'))
                {
                    warningsBracket.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line}");
                }
                if (line.Count(c => c is '[') != line.Count(c => c is ']'))
                {
                    warningsBracket.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line}");
                }

                // пустые адреса
                if (!line.Contains('#') && new Regex("[A-Z]+[A-Z]|[A-Z]$", RegexOptions.Compiled) is { } matchEmptyAddress && 
                    matchEmptyAddress.IsMatch(lineWithoutParenthesis) && 
                    !matchEmptyAddress.Match(lineWithoutParenthesis).Value.Contains("GOTO") &&
                    !matchEmptyAddress.Match(lineWithoutParenthesis).Value.Contains("END"))
                {
                    warningsEmptyAddress.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line}");
                }

                // лишние точки
                if (new Regex(@"[A-Z]+[-]?\d*[.]+\d*[.]", RegexOptions.Compiled).IsMatch(lineWithoutParenthesis))
                {
                    warningsDots.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line}");
                }

                //// лишний текст 
                //if (!new Regex(@"[)]$", RegexOptions.Compiled).IsMatch(line.TrimEnd()) && line.Contains(')'))
                //{
                //    warningsExcessText.Add($"[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line}");
                //}

                // конец программы, добавляем последний инструмент, т.к. инструмент добавляется при вызове следующего, а у последнего следующего нет
                if (lineWithoutParenthesis.Contains("M30") || lineWithoutParenthesis.Contains("M99"))
                {
                    warningEndProgram = false;
                    if (currentToolNo != 0 && currentToolComment != string.Empty) break;
                }

                // фрезерный инструмент
                if (new Regex(@"T(\d+)", RegexOptions.Compiled).IsMatch(line) && line.Contains("M6") && !line.StartsWith('('))
                {
                    millProgram = true;
                    
                    var toolLine = line.Contains('(') 
                        ? line.Split('T')[1].Replace("M6", string.Empty).Split('(')[0].Replace(" ", string.Empty) 
                        : line.Split('T')[1].Replace("M6", string.Empty).Replace(" ", string.Empty);
                    currentD = 0;
                    if (int.TryParse(toolLine, out currentToolNo))
                    {
                        try
                        {
                            currentToolComment = $"(" + line.Split("(")[1].Trim();
                            //if (!tools.Contains($"{currentTool} {currentToolComment}")) tools.Add($"{currentTool} {currentToolComment}");
                        }
                        catch (IndexOutOfRangeException)
                        {
                            currentToolComment = $"(---)";
                            //if (!tools.Contains($"{currentTool} {currentToolComment}")) tools.Add($"{currentTool} {currentToolComment}");
                        }
                    }
                }

                // токарный инструмент
                if (new Regex(@"T(\d+)", RegexOptions.Compiled).IsMatch(line) && !millProgram && !line.StartsWith('('))
                {

                    var toolLine = line.Contains('(') ? line.Split('T')[1].Split('(')[0].Replace(" ", string.Empty) : line.Split('T')[1].Replace(" ", string.Empty);
                    if (toolLine.Contains('X')) toolLine = toolLine.Split('X')[0];
                    if (toolLine.Contains('Y')) toolLine = toolLine.Split('Y')[0];
                    if (toolLine.Contains('Z')) toolLine = toolLine.Split('Z')[0];
                    if (toolLine.Contains('A')) toolLine = toolLine.Split('A')[0];
                    if (toolLine.Contains('F')) toolLine = toolLine.Split('F')[0];
                    if (toolLine.Contains('M')) toolLine = toolLine.Split('M')[0];
                    if (toolLine.Contains('G')) toolLine = toolLine.Split('G')[0];
                    currentD = 0;
                    if (int.TryParse(toolLine, out currentToolNo))
                    {
                        try
                        {
                            currentToolComment = $"(" + line.Split("(")[1].Trim();
                            //if (!tools.Contains($"{currentTool} {currentToolComment}")) tools.Add($"{currentTool} {currentToolComment}");
                        }
                        catch (IndexOutOfRangeException)
                        {
                            currentToolComment = $"(---)";
                            //if (!tools.Contains($"{currentTool} {currentToolComment}")) tools.Add($"{currentTool} {currentToolComment}");
                        }
                    }
                }

                if ((line.Contains("G41") || line.Contains("G42")) && line.Contains('D'))
                {
                    var compensationLine = line.Split("D")[1];
                    if (compensationLine.Contains('X')) compensationLine = compensationLine.Split('X')[0];
                    if (compensationLine.Contains('Y')) compensationLine = compensationLine.Split('Y')[0];
                    if (compensationLine.Contains('Z')) compensationLine = compensationLine.Split('Z')[0];
                    if (compensationLine.Contains('A')) compensationLine = compensationLine.Split('A')[0];
                    if (compensationLine.Contains('F')) compensationLine = compensationLine.Split('F')[0];
                    if (compensationLine.Contains('M')) compensationLine = compensationLine.Split('M')[0];
                    if (compensationLine.Contains('G')) compensationLine = compensationLine.Split('G')[0];
                    if (int.TryParse(compensationLine.Replace(" ", string.Empty), out currentD))
                    {
                        if (currentToolNo != currentD && currentToolNo != 0)
                        {
                            warningsD.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line} - (T{currentToolNo} D{currentD})");
                        }
                        else if (currentToolNo != currentD && currentToolNo == 0)
                        {
                            warningsD.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line} - (D{currentD} - без инструмента)");
                        }
                    }
                    //compensationsD.Add($"T{currentTool} D{int.Parse(compensationLine.Replace(" ", string.Empty))}");
                }
                if (line.Contains("G43") && line.Contains('H'))
                {
                    var compensationLine = line.Split("H")[1];
                    if (compensationLine.Contains('X')) compensationLine = compensationLine.Split('X')[0];
                    if (compensationLine.Contains('Y')) compensationLine = compensationLine.Split('Y')[0];
                    if (compensationLine.Contains('Z')) compensationLine = compensationLine.Split('Z')[0];
                    if (compensationLine.Contains('A')) compensationLine = compensationLine.Split('A')[0];
                    if (compensationLine.Contains('F')) compensationLine = compensationLine.Split('F')[0];
                    if (compensationLine.Contains('M')) compensationLine = compensationLine.Split('M')[0];
                    if (compensationLine.Contains('G')) compensationLine = compensationLine.Split('G')[0];
                    if (int.TryParse(compensationLine.Replace(" ", string.Empty), out currentH))
                    {
                        if (currentToolNo != currentH && currentToolNo != 0)
                        {
                            warningsH.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line} - (T{currentToolNo} H{currentH})");
                        }
                        else if (currentToolNo != currentH && currentToolNo == 0) 

                        {
                            warningsH.Add($"{indent}[{(lines.IndexOf(line) + 1).ToString(fString)}]: {line} - (H{currentH} - без инструмента)");
                        }
                    }
                }
            }
            caption = $"{(millProgram ? "Фрезерная" : "Токарная")} программа";
            coordinates = coordinateSystems.Count switch
            {
                1 => $"Система координат {coordinateSystems[0]}\n",
                > 1 => $"Системы координат: {string.Join(", ", coordinateSystems)}\n",
                _ => "Системы координат отсутствуют\n"
            };
            //coordinates = $"Время: {sw.ElapsedMilliseconds} мс\n" + coordinates;
        }
    }
}
