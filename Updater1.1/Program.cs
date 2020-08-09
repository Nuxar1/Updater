using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Updater1._1
{
    class Program
    {
        static string path, filename, ProcessID, newFilename;

        static void Main(string[] args)
        {
            Thread thread = new Thread(() => update(args));
            thread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            
        }   
        public static void update(string[] args)
        {
            if (args.Length != 4)
            {
                while (MessageBox.Show("Dont start this .exe!!!\n This is just for updating and will be executed automaticly!") != DialogResult.OK) ;
                DeleteMyself();
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

            File.Move(path + "\\TempDownload.exe", path + "\\" + newFilename);
            Console.WriteLine("moved");

            Process.Start(path + "\\" + newFilename);
            DeleteMyself();
            Application.Exit();
            return;
        }
        public static void AzureFileDownload(string fileName, string containerName)
        {
            string mystrconnectionString = "DefaultEndpointsProtocol=https;AccountName=zambu;AccountKey=NeQPY59AATU/n/v2OOVeT7aG/NPu4cDUcnO1zV76NLR/7zhMdvOihAjG4oFQ92nNLqMXoxWYfGrjPSqhGjHxyg==;EndpointSuffix=core.windows.net";//<------------------ enter your key here!

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(mystrconnectionString);
            CloudBlobClient myBlob = mycloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer mycontainer = myBlob.GetContainerReference(containerName);
            CloudBlockBlob myBlockBlob = mycontainer.GetBlockBlobReference(fileName);

            // provide the location of the file need to be downloaded          
            Stream fileupd = File.OpenWrite(path + "\\TempDownload.exe");
            myBlockBlob.DownloadToStream(fileupd);

            fileupd.Dispose();
        }

        public static void DeleteMyself()
        {
            string batchCommands = string.Empty;
            string exeFilename = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");

            batchCommands +=  "@ECHO OFF\n"; // do not show any output

            batchCommands += "ping 127.0.0.1 -n 1 > nul\n"; //proximately 4 seconds (so that the process is already terminated)
            batchCommands += "echo j | del /F ";
            batchCommands += exeFilename + "\n";
            batchCommands += "echo j | del deleteMyProgram.bat";
            File.WriteAllText("deleteMyProgram.bat", batchCommands);

            StartBatSilent("deleteMyProgram.bat");
        }

        private static void StartBatSilent(string  path)
        {
            Process myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = path;
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
        }
    }

}
