using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlwaysDot
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
		static void Main()
		{
			if (!SingleInstance.Start())
			{
				SingleInstance.ShowFirstInstance();
				return;
			}

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
				MainWindow mainForm = new MainWindow();
				Application.Run(mainForm);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}

			SingleInstance.Stop();
		}
	}
}
