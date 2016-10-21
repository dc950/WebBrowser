using System;
using System.Windows.Forms;

namespace WebBrowser
{
    class MainClass
    {
        [STAThread]
        public static void Main() {
            var mainWindow = new MainWindow();
            Application.Run(mainWindow);
        }
    }
}
