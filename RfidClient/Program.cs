using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace RfidClient
{
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			File.WriteAllBytes("LoadMemLibrary.dll", Properties.Resources.LoadMemLibrary);
			File.WriteAllBytes("UHFReader18.dll", Properties.Resources.UHFReader18);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
