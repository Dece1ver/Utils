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
using static System.Net.Mime.MediaTypeNames;

namespace ChamferTrain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double[] _rads = new double[4] { 0.2, 0.4, 0.8, 1.2 };
        private int loses = 0;
        private int wins = 0;

        public int Type { get; set; }
        public int ExtDiameter { get; set; }
        public int IntDiameter { get; set; }
        public double Corner { get; set; }
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
            ScoresText.Text = $"Побед: {wins} | Поражений: {loses}";
            Type = new Random().Next(0, 3);
            int numberOfSteps = (int)((7.0 - 0.2) / 0.1);
            switch (Type) {
                case 0:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/ch1.png"));
                    ExtDiameter = new Random().Next(20, 100);
                    ExtDiamTB.Text = ExtDiameter.ToString();
                    Corner = new Random().Next(0, numberOfSteps + 1) * 0.1 + 0.2;
                    CornerTB.Text = Corner.ToString("F2");
                    break;
                case 1:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/ch2.png"));

                    break;
                case 2:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad1.png"));
                    ExtDiameter = new Random().Next(20, 100);
                    ExtDiamTB.Text = ExtDiameter.ToString();
                    Corner = new Random().Next(0, numberOfSteps + 1) * 0.1 + 0.2;
                    CornerTB.Text = Corner.ToString("F2");
                    break;
                case 3:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad2.png"));
                    break;
                default:
                    return;
            }
            
            InsertRadius = _rads[new Random().Next(0, _rads.Length)];
            RadiusTB.Text = InsertRadius.ToString("F2");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetNewData();
            switch (Type)
            {
                case 0:
                    if (double.TryParse(IntDiamTB.Text.Replace('.', ','), out double ch1res) && AlmostEq(ch1res, ExtDiameter - 2 * Corner - InsertRadius))
                    {
                        MessageBox.Show("Ок", "Норм", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Нет", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 1:
                    if (double.TryParse(IntDiamTB.Text.Replace('.', ','), out double ch2res) && AlmostEq(ch2res, IntDiameter + 2 * Corner + InsertRadius))
                    {
                        MessageBox.Show("Ок", "Норм", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Нет", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 2:
                    if (double.TryParse(IntDiamTB.Text.Replace('.', ','), out double rad1res) && AlmostEq(rad1res, ExtDiameter - 2 * (Corner + InsertRadius)))
                    {
                        MessageBox.Show("Ок", "Норм", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Нет", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 3:
                    if (double.TryParse(IntDiamTB.Text.Replace('.', ','), out double rad2res) && AlmostEq(rad2res, IntDiameter + 2 * (Corner + InsertRadius)))
                    {
                        MessageBox.Show("Ок", "Норм", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Нет", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;

                default:
                    return;
            }
            
            
        }

        bool AlmostEq(double var1, double var2, double tolerance = 0.01) => Math.Abs(var1 - var2) <= tolerance;
    }
}
