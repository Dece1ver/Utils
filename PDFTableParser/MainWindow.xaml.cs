using IronPdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System;
using System.IO;
using System.IO.Compression;

namespace PDFTableParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly OpenFileDialog _tableDialog = new();

        private void fileDialogButton_Click(object sender, RoutedEventArgs e)
        {
            _tableDialog.Filter = "PDF Файл (*.PDF)|*.PDF|" +
                                "All files (*.*)|*.*";
            if (_tableDialog.ShowDialog() == true)
            {
                filePathTB.Text = _tableDialog.FileName;
                PdfDocument PDF = PdfDocument.FromFile(_tableDialog.FileName);

                string content = Encoding.UTF8.GetString(PDF.BinaryData);

                if (!string.IsNullOrEmpty(content))
                {
                    infoTB.Text = content;
                }
                else
                {
                    infoTB.Text = "Текст в данном файле не обнаружен.";
                }

            }
        }



    }
}
