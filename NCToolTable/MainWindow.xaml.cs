using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace NCToolTable
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new();

            if (fileDialog.ShowDialog().Value)
            {
                var tools = new List<string>();
                var filePath = fileDialog.FileName;
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (line.Contains("T") && line.Contains("("))
                    {
                        tools.Add("(" + line.Split("(")[1].Trim());
                    }
                }
                if (tools.Count > 0)
                {
                    Clipboard.SetText(string.Join(Environment.NewLine, tools));
                }
            }
            else
            {
                MessageBox.Show("Выбор файла отменен.");
            }
            Close();
        }
    }
}
