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
        private double[] _corners = new double[12] { 0.2, 0.3, 0.5, 0.6, 1, 1.5, 1.6, 2, 2.5, 3, 4, 5 };
        private double[] _addCorners = new double[6] { 0.2, 0.3, 0.5, 1, 2, 3 };
        private int loses = 0;
        private int wins = 0;

        public int Type { get; set; }
        public double ExtDiameter { get; set; }
        public double AddCorner { get; set; }
        public double Corner { get; set; } = 0;
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
            ResultTB.Text = string.Empty;
            AddResultTB.Text = string.Empty;
            ScoresText.Text = $"Побед: {wins} | Поражений: {loses}";
            Type = new Random().Next(0, 4);
            switch (Type) {
                case 0:
                    HideAdditional();
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/ch1.png"));
                    TypeTB.Text = "C = ";
                    ResTb.Text = "X = ";
                    break;
                case 1:
                    HideAdditional();
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/ch2.png"));
                    TypeTB.Text = "C = ";
                    ResTb.Text = "Z = ";
                    break;
                case 2:
                    HideAdditional();
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad1.png"));
                    TypeTB.Text = "r = ";
                    ResTb.Text = "X = ";
                    break;
                case 3:
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/rad2.png"));
                    TypeTB.Text = "r = ";
                    ResTb.Text = "Z = ";
                    break;
                case 4:
                    ShowAdditional();
                    TaskImage.Source = new BitmapImage(new Uri("pack://application:,,,/ChamferTrain;component/res/ch3.png"));
                    TypeTB.Text = "C = ";
                    ResTb.Text = "X = ";
                    break;
                default:
                    return;
            }
            ExtDiameter = new Random().Next(20, 100);
            ExtDiamTB.Text = ExtDiameter.ToString();
            Corner = _corners[new Random().Next(0, _corners.Length)];
            CornerTB.Text = Corner.ToString("F1").Replace(',','.');
            InsertRadius = _rads[new Random().Next(0, _rads.Length)];
            RadiusTB.Text = InsertRadius.ToString("F1").Replace(',','.');
            AddCorner = _addCorners[new Random().Next(0, _addCorners.Length)];
            AddCornerTB.Text = AddCorner.ToString("F1").Replace(",",".");
            ResultTB.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (Type)
            {
                case 0:
                    if (double.TryParse(ResultTB.Text.Replace('.', ','), out double ch1res) && AlmostEq(ch1res, ExtDiameter - 2 * Corner - InsertRadius))
                    {
                        MessageBox.Show("Успех", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Неправильно", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 1:
                    if (double.TryParse(ResultTB.Text.Replace('.', ','), out double ch2res) && AlmostEq(Math.Abs(ch2res), Corner + InsertRadius / 2))
                    {
                        _ = ch2res > 0 
                            ? MessageBox.Show("Первоклассно, но не забывай про минус.", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information) 
                            : MessageBox.Show("Первоклассно", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Неправильно", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 2:
                    if (double.TryParse(ResultTB.Text.Replace('.', ','), out double rad1res) && AlmostEq(rad1res, ExtDiameter - 2 * (Corner + InsertRadius)))
                    {
                        MessageBox.Show("Великолепно", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Неправильно", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 3:
                    if (double.TryParse(ResultTB.Text.Replace('.', ','), out double rad2res) && AlmostEq(Math.Abs(rad2res), Corner + InsertRadius))
                    {
                        _ = rad2res > 0 
                            ? MessageBox.Show("Норм, но не забывай про минус.", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information) 
                            : MessageBox.Show("Норм", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Нет", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                case 4:
                    if (double.TryParse(ResultTB.Text.Replace('.', ','), out double ch3res1) && AlmostEq(ch3res1, ExtDiameter - 2 * Corner - InsertRadius)
                        && double.TryParse(AddResultTB.Text.Replace('.', ','), out double ch3res2) && AlmostEq(ch3res2, ch3res1 - 2 * (AddCorner + InsertRadius)))
                    {
                        MessageBox.Show("Успех", "Правильно", MessageBoxButton.OK, MessageBoxImage.Information);
                        wins++;
                        SetNewData();
                    }
                    else
                    {
                        loses++;
                        MessageBox.Show("Нет", "Неправильно", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
                default:
                    return;
            }
            ScoresText.Text = $"Побед: {wins} | Поражений: {loses} | Успешность: {(double)wins / (double)(wins + loses) * 100:N0}%";
            
        }

        static bool AlmostEq(double var1, double var2, double tolerance = 0.01) => Math.Abs(var1 - var2) <= tolerance;

        void HideAdditional()
        {
            AddTypeTB.Visibility = Visibility.Collapsed;
            AddCornerTB.Visibility = Visibility.Collapsed;
            AddResTb.Visibility = Visibility.Collapsed;
            AddResultTB.Visibility = Visibility.Collapsed;
        }

        void ShowAdditional()
        {
            AddTypeTB.Visibility = Visibility.Visible;
            AddCornerTB.Visibility = Visibility.Visible;
            AddResTb.Visibility = Visibility.Visible;
            AddResultTB.Visibility = Visibility.Visible;
        }
    }
}
