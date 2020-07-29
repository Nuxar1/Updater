using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Version1._0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath;
            string filename = Path.GetFileName(Application.ExecutablePath);
            string PID = Process.GetCurrentProcess().Id.ToString();
            string newFilename = "Version1.1.exe";


            Process.Start("C:\\Users\\Mats\\source\\repos\\Updater\\Updater1.1\\bin\\Debug\\Updater1.1.exe", $" \"{path}\" \"{filename}\" {PID} \"{newFilename}\"");
        }
    }
}
