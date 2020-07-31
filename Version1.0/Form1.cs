using Dapper;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
            CheckUpdate();
        }

        public async void CheckUpdate()
        {
            bool retry;

            do
            {
                retry = false;
                int b = 3;
                await Task.Run(() =>
                {
                    try
                    {
                        using (var conn = new SqlConnection("Server = tcp:zambu.database.windows.net,1433; Initial Catalog = ZAMBU_SQL; Persist Security Info = False; User ID = Mats; Password =ZambuYT#2290; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;"))
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@foo", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                            parameters.Add("@Version", "1.0");
                            parameters.Add("@Product", "Test");
                            conn.Execute("[dbo].spCheckVersion", parameters, commandType: CommandType.StoredProcedure);

                            b = parameters.Get<int>("@foo");

                        }
                    }
                    catch (Exception ex)
                    {
                        if(MessageBox.Show("Threre was an error connecting to the SQL server and check for updates.\n Do you want to retry?", "SQL connection error!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            retry = true;
                        }
                    }
                });

                if (b == 0)
                {
                    MessageBox.Show("You have the latest version");
                }
                else
                {
                    if (MessageBox.Show("This version is of the application is old.\nInitialize update?", "You have an old version!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                string newName;
                                using (var conn = new SqlConnection("Server = tcp:zambu.database.windows.net,1433; Initial Catalog = ZAMBU_SQL; Persist Security Info = False; User ID = Mats; Password =ZambuYT#2290; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;"))
                                {
                                    DynamicParameters _param = new DynamicParameters();
                                    _param.Add("@Product", "test");
                                    _param.Add("@newName", "", DbType.String, ParameterDirection.Output);

                                    conn.Execute("[dbo].spGetNewVersion @Product, @newName output", _param);

                                    newName = _param.Get<string>("newName");
                                }
                                update(newName);
                            });
                        }
                        catch (Exception)
                        {
                            if(MessageBox.Show("Threre was an error connecting to the SQL server and check for updates.\n Do you want to retry?", "SQL connection error!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                retry = true;
                            }
                        }

                    }
                }
            } while (retry);
            
        }

        private void update(string newVersion)
        {

            UpdateScreen updateScreen = new UpdateScreen();
            updateScreen.Show();

            Task.Delay(500);

            string path = Application.StartupPath;
            string filename = Path.GetFileName(Application.ExecutablePath);
            string PID = Process.GetCurrentProcess().Id.ToString();
            string newFilename = newVersion;
            bool retry;
            do
            {
                retry = false;
                try
                {
                    AzureFileDownload("Updater.exe", "updates", path);

                    Process.Start("Updater.exe", $" \"{path}\" \"{filename}\" {PID} \"{newFilename}\"");
                }
                catch (Exception)
                {
                    if (MessageBox.Show("An error occured while downloading the Updater.\n do you want to retry?", "Critical error!!!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        retry = true;
                    }
                    
                }
            } while (retry);
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
