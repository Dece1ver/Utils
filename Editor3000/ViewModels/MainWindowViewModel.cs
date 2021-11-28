using Editor3000.Infrastructure.Commands;
using Editor3000.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Editor3000.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
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


        private string _TargetContent;

        public string TargetContent
        {
            get => _TargetContent;
            set => Set(ref _TargetContent, value);
        }

        private int _Encoding;

        public int Encoding
        {
            get => _Encoding;
            set => Set(ref _Encoding, value);
        }


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
            OpenFileDialog fileDialog = new();
            if (fileDialog.ShowDialog() == true)
            {
                TargetPath = fileDialog.FileName;
                var content = File.ReadLines(TargetPath);
                Status = $" | {content.Count()} Строк";
                TargetContent = string.Join(Environment.NewLine, File.ReadLines(TargetPath, System.Text.Encoding.GetEncoding(0)));
            }
            else
            {
                Status = "Выбор отменен";
            }
        }
        private bool CanSetPathCommandExecute(object p) => true;
        #endregion


        #endregion



        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            SetPathCommand = new LambdaCommand(OnSetPathCommandExecuted, CanSetPathCommandExecute);
        }



    }
}
