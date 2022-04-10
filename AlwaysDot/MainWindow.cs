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
        
        private void MainWindow_Load(object sender, EventArgs e)
        {
            _listener = new KeyboardListener();
            _listener.HookKeyboard();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e) => _listener.UnHookKeyboard();

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) => MessageBox.Show(
                "При включенной RU раскладке клавиша Del на цифровой клавиатуре вводит точку вместо запятой.\n\ndece1ver © 2022",
                "О программе Always Dot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        private void closeMenuItem_Click(object sender, EventArgs e) => Close();

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                ShowWindow();
            }
            base.WndProc(ref message);
        }

        public void ShowWindow()
        {
            WinApi.ShowToFront(this.Handle);
        }

    }
}
