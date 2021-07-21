using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using static MassCalc.Fr4._7.Util;

namespace MassCalc.Fr4._7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string densityString;
        private string diameterString;
        private string lengthString;
        private double mass;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }

        public string DensityString
        {
            get => densityString; set
            {
                Set(ref densityString, value);
                RaisePropertyChanged(nameof(Mass));
            }
        }
        public double Density { get => DensityString.GetDouble(); }

        public string DiameterString
        {
            get => diameterString; set
            {
                Set(ref diameterString, value);
                RaisePropertyChanged(nameof(Mass));
            }
        }
        public double Diameter { get => DiameterString.GetDouble(); }

        public string LengthString
        {
            get => lengthString; set
            {
                Set(ref lengthString, value);
                RaisePropertyChanged(nameof(Mass));
            }
        }

        public double Length { get => LengthString.GetDouble(); }

        public double Mass { get => Math.PI * Density * (Diameter * Diameter / 4) * (Length / 1000) / 1000000; set => Set(ref mass, value); }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        #endregion
    }
}
