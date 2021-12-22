using DetailsList.Infrastructure.Commands;
using DetailsList.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DetailsList.ViewModels
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


        private bool _GetFilesThreadFlag = false;

        public bool GetFilesThreadFlag
        {
            get => _GetFilesThreadFlag;
            set => Set(ref _GetFilesThreadFlag, value);
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
        /// Список файлов
        /// </summary>
        private List<string> _Details;

        public List<string> Details
        {
            get => _Details;
            set => Set(ref _Details, value);
        }

        public int? DetailsCount => Details?.Count;

        public string DetailsText => string.Join("\n", Details is null ? new List<string>() : Details);

        
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


        #region FindDetailsCommand
        public ICommand FindDetailsCommand { get; }
        private void OnFindDetailsCommandExecuted(object p)
        {
            if (!GetFilesThreadFlag)
            {
                GetFilesThreadFlag = true;
                getFilesThread = new(() => FindPrograms(TargetPath));
                getFilesThread.Start();
            }
            else
            {
                GetFilesThreadFlag = false;
            }
        }
        private bool CanFindDetailsCommandExecute(object p) => true;
        #endregion


        #region SaveDetailsToFileCommand
        public ICommand SaveDetailsToFileCommand { get; }
        private void OnSaveDetailsToFileCommandExecuted(object p)
        {
            if (DetailsCount > 0)
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, DetailsText);
                    Status = $"Список записан в файл \"{saveFileDialog.FileName}\"";
                }
            }
        }
        private bool CanSaveDetailsToFileCommandExecute(object p) => true;
        #endregion

        #endregion



        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            SetPathCommand = new LambdaCommand(OnSetPathCommandExecuted, CanSetPathCommandExecute);
            FindDetailsCommand = new LambdaCommand(OnFindDetailsCommandExecuted, CanFindDetailsCommandExecute);
            SaveDetailsToFileCommand = new LambdaCommand(OnSaveDetailsToFileCommandExecuted, CanSaveDetailsToFileCommandExecute);
        }

        private void FindPrograms(string path)
        {
            FindButtonText = "Остановить";
            Files = new();
            Details = new();
            Status = "Подсчет файлов";
            BrowseButtonEnabled = false;
            SaveButtonEnabled = false;
            OnPropertyChanged(nameof(FilesCount));
            OnPropertyChanged(nameof(DetailsCount));
            OnPropertyChanged(nameof(DetailsText));
            ProgressBarVisibility = Visibility.Collapsed;
            GetFiles(path);
            ProgressMaxValue = (double)FilesCount;
            Progress = 0;
            ProgressBarVisibility = Visibility.Visible;
            foreach (var file in Files)
            {
                Status = "Проверка файлов на наличие УП";
                if (!GetFilesThreadFlag) 
                {
                    ProgressBarVisibility = Visibility.Collapsed;
                    break;
                }
                Progress++;
                try
                {
                    var lines = File.ReadLines(file).Take(2).ToArray();
                    if (lines.Length > 0 && lines[0] == "%")
                    {
                        var detail = GetDetailName(file, TargetPath);
                        if (!Details.Contains(detail) && !string.IsNullOrEmpty(detail)
                            //&& !(
                            //new Regex(@"^АР(\d+)", RegexOptions.Compiled).IsMatch(detail) ||
                            //new Regex(@"^АРМ(\d+)", RegexOptions.Compiled).IsMatch(detail) ||
                            //new Regex(@"^АРПГА(\d+)", RegexOptions.Compiled).IsMatch(detail) ||
                            //new Regex(@"^АРКП(\d+)", RegexOptions.Compiled).IsMatch(detail) ||
                            //new Regex(@"^НМГ(\d+)", RegexOptions.Compiled).IsMatch(detail))
                            )
                        {
                            Details.Add(detail);
                            OnPropertyChanged(nameof(DetailsCount));
                            OnPropertyChanged(nameof(DetailsText));
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.ToString());
                }
            }
            Status = "Завершено";
            if(DetailsCount > 0) SaveButtonEnabled = true;
            BrowseButtonEnabled = true;
            FindButtonText = "Сформировать";
            GetFilesThreadFlag = false;
        }

        private void GetFiles(string path)
        {

            try
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    if (!GetFilesThreadFlag) break;
                    GetFiles(folder);
                }
                foreach (string file in Directory.GetFiles(path))
                {
                    if (!GetFilesThreadFlag) break;
                    Files.Add(file);
                    OnPropertyChanged(nameof(FilesCount));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.ToString());
            }
        }

        public static string GetDetailName(string file, string targetPath)
        {
            string cwd = file;
            while (cwd != targetPath)
            {
                cwd = Directory.GetParent(cwd).FullName;
                foreach (var sign in Infrastructure.DetailsInfo.numberSigns)
                {
                    if (Path.GetFileName(cwd).Contains(sign))
                    {
                        return $"{Directory.GetParent(cwd).Name} {Path.GetFileName(cwd)}";
                    }
                }
            }
            return string.Empty;
            //return $"{Directory.GetParent(file).Parent.Name} {Directory.GetParent(file).Name}";
        }

    }
}
