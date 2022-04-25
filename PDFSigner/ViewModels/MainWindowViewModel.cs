using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using PDFSigner.Infrastructure.Commands;
using PDFSigner.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
namespace PDFSigner.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        Thread signFilesThread;

        private bool _SignFilesThreadFlag = false;

        public bool SignFilesThreadFlag
        {
            get => _SignFilesThreadFlag;
            set => Set(ref _SignFilesThreadFlag, value);
        }

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

        public bool SignButtonEnabled => (SelectedFilesCount > 0 && !string.IsNullOrEmpty(SignText));

        private string _SignButtonText = "Подписать";

        public string SignButtonText
        {
            get => _SignButtonText;
            set => Set(ref _SignButtonText, value);
        }

        private string _SignText;

        public string SignText
        {
            get => _SignText;
            set 
            {
                Set(ref _SignText, value);
                OnPropertyChanged(nameof(SignButtonEnabled));
            }
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

        private Visibility _ProgressBarVisibility = Visibility.Collapsed;

        public Visibility ProgressBarVisibility
        {
            get => _ProgressBarVisibility;
            set => Set(ref _ProgressBarVisibility, value);
        }

        /// <summary>
        /// Список файлов
        /// </summary>
        private List<string> _SelectedFiles;

        public List<string> SelectedFiles
        {
            get => _SelectedFiles;
            set => Set(ref _SelectedFiles, value);
        }

        public int SelectedFilesCount => SelectedFiles is null ? 0 : SelectedFiles.Count;

        /// <summary>
        /// Список подписанных
        /// </summary>
        private List<string> _SignedFiles;

        public List<string> SignedFiles
        {
            get => _SignedFiles;
            set => Set(ref _SignedFiles, value);
        }

        public int SignedFilesCount => SignedFiles is null ? 0 : SignedFiles.Count;

        #region Команды


        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        private bool CanCloseApplicationCommandExecute(object p) => true;
        #endregion


        #region SetFilesCommand
        public ICommand SetFilesCommand { get; }
        private void OnSetFilesCommandExecuted(object p)
        {
            OpenFileDialog folderDialog = new();
            folderDialog.Multiselect = true;
            folderDialog.Filter = "Документы PDF (*.pdf)|*.pdf";
            if (folderDialog.ShowDialog() == true)
            {
                SelectedFiles = folderDialog.FileNames.ToList();
                Progress = 0;
                OnPropertyChanged(nameof(SelectedFilesCount));
                OnPropertyChanged(nameof(SignButtonEnabled));
            }
            else
            {
                Status = "Выбор отменен";
            }
        }
        private bool CanSetFilesCommandExecute(object p) => true;
        #endregion

        #region SignFilesCommand
        public ICommand SignFilesCommand { get; }
        private void OnSignFilesCommandExecuted(object p)
        {
            SignFilesThreadFlag = true;
            signFilesThread = new(() => Sign(SelectedFiles));
            signFilesThread.Start();
        }
        private bool CanSignFilesCommandExecute(object p) => true;
        #endregion


        #endregion



        public MainWindowViewModel()
        {
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            SetFilesCommand = new LambdaCommand(OnSetFilesCommandExecuted, CanSetFilesCommandExecute);
            SignFilesCommand = new LambdaCommand(OnSignFilesCommandExecuted, CanSignFilesCommandExecute);
        }


        private void Sign(List<string> files)
        {
            PdfReader.unethicalreading = true;
            Status = "Подпись файлов";
            ProgressBarVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(ProgressBarVisibility));
            Progress = 0;
            foreach (var file in files)
            {
                var tempName = Path.Combine(Path.GetDirectoryName(file), "~tmp" + Path.GetFileName(file));
                File.Move(file, tempName, true);
                try
                {
                    using (var reader = new PdfReader(tempName))
                    {
                        using (var fileStream = new FileStream(
                            file,
                            FileMode.Create,
                            FileAccess.Write))
                        {
                            var document = new Document(reader.GetPageSizeWithRotation(1));
                            var writer = PdfWriter.GetInstance(document, fileStream);

                            document.Open();

                            for (var i = 1; i <= reader.NumberOfPages; i++)
                            {
                                document.NewPage();

                                var fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "CONSOLAB.TTF");
                                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                                var importedPage = writer.GetImportedPage(reader, i);

                                var contentByte = writer.DirectContent;
                                contentByte.Rectangle(60, document.PageSize.Height - 75, (double)SignText.Length * 7.2, 20);
                                contentByte.SetFontAndSize(baseFont, 12);
                                contentByte.BeginText();
                                contentByte.ShowTextAligned(PdfContentByte.ALIGN_LEFT, SignText, 65, document.PageSize.Height - 70, 0);
                                contentByte.EndText();
                                contentByte.Stroke();
                                contentByte.AddTemplate(importedPage, 0, 0);

                            }
                            document.Close();
                            writer.Close();
                        }
                    }
                    File.Delete(tempName);
                    Progress++;
                }
                
                catch (Exception ex)
                {
                    SignFilesThreadFlag = false;
                    Status = "Ошибка";
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    File.Move(tempName, file, true);
                }
            }
            Status = $"Завершено. Подписано {Progress} файлов.";

            ProgressBarVisibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(ProgressBarVisibility));
        }

    }
}
