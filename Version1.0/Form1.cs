using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Updater1._1;

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
            UpdateScreen updateScreen = new UpdateScreen();
            updateScreen.Show();

            Task.Delay(500);

            string path = Application.StartupPath;
            string filename = Path.GetFileName(Application.ExecutablePath);
            string PID = Process.GetCurrentProcess().Id.ToString();
            string newFilename = "Version1.1.exe";
            AzureFileDownload("Updater.exe", "updates", path);

            Process.Start("Updater.exe", $" \"{path}\" \"{filename}\" {PID} \"{newFilename}\"");
        }


        public static void AzureFileDownload(string fileName, string containerName, string path)
        {
            string mystrconnectionString = "DefaultEndpointsProtocol=https;AccountName=zambu;AccountKey=NeQPY59AATU/n/v2OOVeT7aG/NPu4cDUcnO1zV76NLR/7zhMdvOihAjG4oFQ92nNLqMXoxWYfGrjPSqhGjHxyg==;EndpointSuffix=core.windows.net";//<------------------ enter your key here!

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(mystrconnectionString);
            CloudBlobClient myBlob = mycloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer mycontainer = myBlob.GetContainerReference(containerName);
            CloudBlockBlob myBlockBlob = mycontainer.GetBlockBlobReference(fileName);

            // provide the location of the file need to be downloaded          
            Stream fileupd = File.OpenWrite(path + "\\"  + "Updater.exe");
            myBlockBlob.DownloadToStream(fileupd);

            fileupd.Dispose();
        }

    }
}
