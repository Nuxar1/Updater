using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Updater1._1
{
    class Program
    {
        static string path, filename, ProcessID, newFilename;
        static Form1 Form1;

        static void Main(string[] args)
        {
            Thread thread = new Thread(() => update(args));
            thread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            if (args.Length == 4)
            {
                Form1 = new Form1();
                Application.Run(Form1);
            }
            
        }
        public static void update(string[] args)
        {
            if (args.Length != 4)
            {
                while (MessageBox.Show("Dont start this .exe!!!\n This is just for updating and will be executed automaticly!") != DialogResult.OK) ;
                Application.Exit();
                return;
            }
            path = args[0];
            filename = args[1];
            ProcessID = args[2];
            newFilename = args[3];


            Process.GetProcessById(Convert.ToInt32(ProcessID)).Kill();

            AzureFileDownload(newFilename, "updates");

            File.Delete(path + "\\" + filename);
            Console.WriteLine("deleted!");

            File.Move(path + "\\new.exe", path + "\\" + newFilename);
            Console.WriteLine("moved");

            Process.Start(path + "\\" + newFilename);
            Thread.Sleep(2000);
            Application.Exit();
        }
        public static void AzureFileDownload(string fileName, string containerName)
        {
            string mystrconnectionString = "<key>";//<---------------- enter your key here!

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(mystrconnectionString);
            CloudBlobClient myBlob = mycloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer mycontainer = myBlob.GetContainerReference(containerName);
            CloudBlockBlob myBlockBlob = mycontainer.GetBlockBlobReference(fileName);

            // provide the location of the file need to be downloaded          
            Stream fileupd = File.OpenWrite(path + "\\new.exe");
            myBlockBlob.DownloadToStream(fileupd);

            fileupd.Dispose();
        }

    }

}
