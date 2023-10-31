using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata;
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

namespace ChamferTrain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double[] _rads = new double[4] { 0.2, 0.4, 0.8, 1.2 };
        private double[] _corners = new double[12] { 0.2, 0.3, 0.5, 0.6, 1, 1.5, 1.6, 2, 2.5, 3, 4, 5 };
        private double[] _addCorners = new double[6] { 0.2, 0.3, 0.5, 1, 2, 3 };
        private int loses = 0;
        private int wins = 0;

        public enum TaskType
        {
            ChamferExternalTraditional,
            ChamferExternalReverse,
            ChamferInternalTraditional,
            ChamferInternalTraditionalС,
            ChamferInternalReverse,
            ChamferInternalReverseС,
            RadiusExternalTraditionalG1,
            RadiusExternalReverseG1,
            RadiusExternalTraditionalG3,
            RadiusExternalReverseG2,
            RadiusInternalTraditionalG1,
            RadiusInternalTraditionalG2,
            RadiusInternalReverseG1,
            RadiusInternalReverseG3,
        }
        public TaskType Type { get; set; }
        public double Diameter { get; set; }
        public double AddCorner { get; set; }
        public double Blunt { get; set; } = 0;
        public double InsertRadius { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetNewData();
        }

        private void SetNewData()
        {
            //ResultTB.Text = string.Empty;
            //AddResultTB.Text = string.Empty;
            ScoresText.Text = $"Побед: {wins} | Поражений: {loses}";
            var taskTypes = Enum.GetValues(typeof(TaskType));
            Type = (TaskType)taskTypes.GetValue(new Random().Next(taskTypes.Length))!;
            switch (Type)
            {
                case TaskType.ChamferExternalTraditional:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/cha_ext_tra.png"));
                    BluntLabelTextBox.Text = "C";
                    ExternalChamferXTextBox.Focus();
                    break;
                case TaskType.ChamferExternalReverse:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/cha_ext_rev.png"));
                    BluntLabelTextBox.Text = "C";
                    ExternalChamferReverseZTextBox.Focus();
                    break;
                case TaskType.ChamferInternalTraditional:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/cha_int_tra.png"));
                    BluntLabelTextBox.Text = "C";
                    break;
                case TaskType.ChamferInternalReverse:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/cha_int_rev.png"));
                    BluntLabelTextBox.Text = "C";
                    break;
                case TaskType.RadiusExternalTraditionalG1 or TaskType.RadiusExternalTraditionalG3:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad_ext_tra.png"));
                    BluntLabelTextBox.Text = "R";
                    ExternalRadiusG1XTextBox.Focus();
                    break;
                case TaskType.RadiusExternalReverseG1 or TaskType.RadiusExternalReverseG2:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad_ext_rev.png"));
                    BluntLabelTextBox.Text = "R";
                    break;
                case TaskType.RadiusInternalTraditional:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad_int_tra.png"));
                    BluntLabelTextBox.Text = "R";
                    break;
                case TaskType.RadiusInternalReverse:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad_int_rev.png"));
                    BluntLabelTextBox.Text = "R";
                    break;
            }
            Diameter = new Random().Next(20, 100);
            DiameterTextBlock.Text = $"{Diameter}";
            Blunt = _corners[new Random().Next(0, _corners.Length)];
            BluntTextBlock.Text = $"{Blunt:F1}".Replace(',','.');
            InsertRadius = _rads[new Random().Next(0, _rads.Length)];
            InsertRadiusTextBlock.Text = $"{InsertRadius:F1}".Replace(',','.');
            
            //RadiusTB.Text = InsertRadius.ToString("F1").Replace(',','.');
            //AddCorner = _addCorners[new Random().Next(0, _addCorners.Length)];
            //AddCornerTB.Text = AddCorner.ToString("F1").Replace(",",".");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Type.ToString());
            switch (Type)
            {
                
            }
            ScoresText.Text = $"Побед: {wins} | Поражений: {loses} | Успешность: {(double)wins / (double)(wins + loses) * 100:N0}%";

        }

        static bool AlmostEq(double var1, double var2, double tolerance = 0.01) => Math.Abs(var1 - var2) <= tolerance;

    }
}
