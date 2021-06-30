using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Window = Utils.WinAPI.Windows.Window;

namespace WindowVisibility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Window _currentWindow;
        private bool _watch;
        List<IntPtr> _winList;
        List<string> _winNames;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchWindows();
        }

        private async void SearchWindows()
        {

            while (true)
            {
                var windows = Window.Find(w => w.Handle != IntPtr.Zero);
                _winList = new();
                _winNames = new();
                foreach (var window in windows)
                {
                    if (window.Handle != IntPtr.Zero && !string.IsNullOrEmpty(window.Text) && window.Visible)
                    {
                        _winList.Add(window.Handle);
                        _winNames.Add(window.Text);
                    }
                }
                windowNameCB.ItemsSource = _winNames;
                await Task.Delay(500);
            }
        }


        private async void setProcessButton_Click(object sender, RoutedEventArgs e)
        {
            _watch = !_watch;
            setProcessButton.Content = _watch ? "Стоп" : "Старт";
            windowNameCB.IsEnabled = _watch ? false : true;
            try
            {
                if (_currentWindow != null)
                {
                    while (_watch)
                    {
                        handleTB.Text = _currentWindow.Handle.ToString();
                        visibleTB.Text = _currentWindow.Visible.ToString();
                        iconicTB.Text = _currentWindow.Iconic.ToString();
                        locationTB.Text = $"X{_currentWindow.X} Y{_currentWindow.Y}";
                        sizeTB.Text = $"{_currentWindow.Width}x{_currentWindow.Height}";
                        await Task.Delay(100);
                    }
                }
                else
                {
                    _watch = false;
                    statusTB.Text = "Не выбрано окно.";
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.StackTrace);
                _watch = false;
                statusTB.Text = "Ошибка";
            }
            setProcessButton.Content = _watch ? "Стоп" : "Старт";
            windowNameCB.IsEnabled = _watch ? false : true;
        }

        private void windowNameCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(windowNameCB.SelectedItem != null)
                {
                    _currentWindow = new Window(_winList[_winNames.IndexOf(windowNameCB.SelectedItem.ToString())]);
                    handleTB.Text = _currentWindow.Handle.ToString();
                    visibleTB.Text = _currentWindow.Visible.ToString();
                    iconicTB.Text = _currentWindow.Iconic.ToString();
                    locationTB.Text = $"X{_currentWindow.X} Y{_currentWindow.Y}";
                    sizeTB.Text = $"{_currentWindow.Width}x{_currentWindow.Height}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.StackTrace);
                _watch = false;
                statusTB.Text = "Ошибка";
            }
            setProcessButton.Content = _watch ? "Стоп" : "Старт";
        }
    }
}
