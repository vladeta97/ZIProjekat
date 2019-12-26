using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Properties;
using Client.ServiceReference1;
using CryptoLib;
namespace Client
{
    public partial class Form1 : Form
    {
        private Service1Client client;
        private List<string> files = new List<string>();
        string userName;

        private static Dictionary<string, int> ekstenzije = new Dictionary<string, int>
        {
            {".png",0},
            {".jpg",1},
            {".txt",2},
            {".doc",3},
            {".pdf",4},
            {".mp3",5},
            {".zip",6},
            {".mp4",7}
        };

        public Form1(string user)
        {
            InitializeComponent();
            client = new Service1Client();
           this.userName = user;
            listView1.Clear();
            files.Clear();
            files = client.AllFiles(userName).ToList();
            this.GetFiles();
            cbOFB.Enabled = false;

        }


        public void Form1_Load(object sender, EventArgs e)
        {
            client = new Service1Client();
            listView1.Clear();
            files.Clear();
            files=client.AllFiles(userName).ToList();
        }

        private void GetFiles()
        {
            files.Clear();
            listView1.Clear();
            files = client.AllFiles(userName).ToList();
            foreach (var item in files)
            {
                var ext = Path.GetExtension(item);
                listView1.Items.Add(item, ekstenzije[ext]);
            }
        }
      
        private void btnUpload_Click(object sender, EventArgs e)
        {
            string prosledi;

            if (rbDT.Checked)
                prosledi = "DT";
            else if (rbKS.Checked)
                prosledi = "KS";
            else if (rbXTEA.Checked == true && cbOFB.Checked == true)
            {
              
                prosledi = "XTEAO";
                
            }
            else
            {
                cbOFB.Enabled = true;
                prosledi = "XTEA";
               
            }
               

            using (OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Select File to Upload",

                CheckFileExists = true,
                CheckPathExists = true,

                Filter = "(*.txt)|*.txt|(*.jpg)|*.jpg|(*.png)|*.png|(*.mp4)|*.mp4|(*.mp3)|*.mp3|(*.doc)|*.doc|(*.pdf)|*.pdf|(*.zip)|*.zip",
                FilterIndex = 1,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    using (Form2 load = new Form2(ofd.FileName, client, ofd.SafeFileName, prosledi, userName))
                    {
                        try
                        {
                            load.ShowDialog();
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    }
                    GetFiles();
                }
                ofd.Reset();
            }



        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                using (Form2 lf = new Form2(listView1.SelectedItems[0].Text, client, userName))
                {
                    lf.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Selektujte fajl koji zelite da skinete");
            }
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
          
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           // Parent.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private string GetHash(byte[] file)
        {
            byte[] niz=new byte[10];
            niz= MD5.Crypt(file);
            return niz.ToString();
        }

        private void rbXTEA_CheckedChanged(object sender, EventArgs e)
        {
            if (rbXTEA.Checked == true)
            {
                cbOFB.Enabled = true;
            }
            else
            {
                cbOFB.Enabled = false;
            }
        }
    }
}
