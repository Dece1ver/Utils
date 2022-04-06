using System;
using System.Windows.Forms;

namespace AlwaysDot
{
    public partial class MainWindow : Form
    {
        private KeyboardListener _listener;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            _listener = new KeyboardListener();
            _listener.OnKeyPressed += _listener_OnKeyPressed;

            _listener.HookKeyboard();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _listener.UnHookKeyboard();
        }

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
           
        }
    }
}
