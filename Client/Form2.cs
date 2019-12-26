using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.ServiceReference1;
using System.IO;
using CryptoLib;
namespace Client
{
    public partial class Form2 : Form
    {
        private static ICrypto crypto;
        private const int chunk = 1048576;
        private byte[] algByte;
        private string userName;

        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string fileName, Service1Client client, string safeFileName, string algoritam,string user)
        {
           
            InitializeComponent();
            this.userName = user;
            this.ControlBox = false;
            if (algoritam == "KS")
            {
                crypto = new Knapsack();
                algByte = Encoding.ASCII.GetBytes("K");
            }
            else if (algoritam == "DT")
            {
                crypto = new DoubleTransposition();
                algByte = Encoding.ASCII.GetBytes("D");
            }
            else if (algoritam == "XTEA")
            {
                crypto = new XTEA(false);
                algByte = Encoding.ASCII.GetBytes("X");
            }
            else if (algoritam == "XTEAO")
            {
                crypto = new XTEA(true);
                algByte = Encoding.ASCII.GetBytes("XO");
            }
            UploadFile(fileName, client, safeFileName);

        }

        public Form2(string fileName, Service1Client client,string user)
        {
            //download constructor
            InitializeComponent();
            /*_userName = userName;
            this.ControlBox = false;
            lblCaption.Text = $@"File {Path.GetFileName(fileName)} is Uploading and encrypting!{Environment.NewLine} Please wait";
            this.Text = @"Downloading...";
            progressBar1.Maximum = 20;
            
            progressBar1.Style = ProgressBarStyle.Marquee;
           */
            this.userName = user;
            timer1.Interval = 250;
            timer1.Start();
            DownloadFile(fileName, client);

        }

        private async void UploadFile(string FileName, Service1Client client, string SafeFileName)
        {
            try
            {
               
                var fileinfo = new FileInfo(FileName);

                var file = File.ReadAllBytes(fileinfo.FullName);
               
                var enc = crypto.Crypt(file);
                var encWithAlgorithm = Append(enc, algByte);
                for (int i = 0, j = 0; i < encWithAlgorithm.LongLength; i += chunk, j++)
                {
                    var razlika = encWithAlgorithm.Length - i;
                    var Chunk = new byte[razlika < chunk ? razlika : chunk];
                    Buffer.BlockCopy(encWithAlgorithm, i, Chunk, 0, razlika < chunk ? (int)razlika : chunk);
                    client.UploadFile(fileinfo.Name, Chunk, j, encWithAlgorithm.LongLength, i + chunk >= encWithAlgorithm.Length,userName);
                }
               // client.Upload(SafeFileName, encWithAlgorithm, userName);
                //var hash = await Task.Run(() => GetHash(file));
                var hash = GetHash(file);
                client.SetFileHash(userName,fileinfo.Name, hash);
                timer1.Stop();
                //MessageBox.Show(GetHash(file));
                //DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        public static byte[] Append(byte[] current, byte[] after)
        {
            var bytes = new byte[current.Length + after.Length];
            current.CopyTo(bytes, 0);
            after.CopyTo(bytes, current.Length);
            return bytes;
        }

        private string GetHash(byte[] file)
        {
            byte[] niz = MD5.Crypt(file);
            string res = BitConverter.ToString(niz);
           
            return res;
        }

        private async void DownloadFile(string fileName, Service1Client client)
        {
            var fileinfo = client.FileInfo(fileName,userName);
            var path = "E://Download//";
          
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var nChunks = fileinfo.Length / chunk + 1;
            var down = new byte[fileinfo.Length];
            for (int i = 0; i < nChunks; i++)
            {
                var Downdata = client.Download(fileinfo.Name, i,userName);
                Buffer.BlockCopy(Downdata, 0, down, i * chunk, Downdata.Length);
            }

           
            var dataWithoutAlgorithamByte = new byte[down.Length - 1];
            algByte = new Byte[] { down[down.Length - 1] };
            if (Encoding.ASCII.GetString(algByte) == "X")
            {
                crypto = new XTEA(false);
            }
           else if (Encoding.ASCII.GetString(algByte) == "XO")
            {
                crypto = new XTEA(true);
            }
            else if (Encoding.ASCII.GetString(algByte) == "K")
            {
                crypto = new Knapsack();
            }
            else if (Encoding.ASCII.GetString(algByte) == "D")
            {
                crypto =new DoubleTransposition();
            }

            Buffer.BlockCopy(down, 0, dataWithoutAlgorithamByte, 0, down.Length - 1);
            var data = crypto.Decrypt(dataWithoutAlgorithamByte);
            File.WriteAllBytes($"{path}{fileName}", data);
             

             var hash = await Task.Run(() => GetHash(data));
             //var hash = GetHash(data);
             var bazahash = client.GetFileHash(userName, fileName);
            if (hash == bazahash) {

            }

            else
            {
                label1.Text = "Greska prilikom skidanja fajla";
                    
            }
             //timer1.Stop();
            DialogResult = DialogResult.OK;
          // this.Close();
            //this.Hide();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /*private string GetHash(byte[] file)
{
   //return MD5.Crypt(file);
}*/

    }
}
