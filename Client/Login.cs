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
using CryptoLib;
namespace Client
{
    public partial class Login : Form
    {
        private Service1Client client;
        public Login()
        {
            InitializeComponent();
            btnRegister.Enabled = false;
            tbRegister.Enabled = false;
            client = new Service1Client();
            tbPassword.UseSystemPasswordChar = true;
            tbPassword.PasswordChar = '*';
            tbRegister.UseSystemPasswordChar = true;
            tbRegister.PasswordChar = '*';
        }

        private void cbRegister_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRegister.Checked == true)
            {
                btnRegister.Enabled = true;
                tbRegister.Enabled = true;
                btnLogIn.Enabled = false;
            }
            else
            {
                btnRegister.Enabled = false;
                tbRegister.Enabled = false;
                btnLogIn.Enabled = true;
            }
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if (client.CheckUser(tbUserName.Text) == true)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(tbPassword.Text);
                var pass = MD5.Crypt(bytes);
                string res = BitConverter.ToString(pass);

                if (client.Login(tbUserName.Text, res))
                {
                    Form1 fd = new Form1(tbUserName.Text);
                    
                    fd.Show();
                    this.Hide();
                  

                }
                else
                {
                    MessageBox.Show("Logovanje nije uspelo");
                }
            }
            else
            {
                MessageBox.Show("Nalog ne postoji");
            }
            
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (tbPassword.Text == tbRegister.Text)
            {
                if (client.CheckUser(tbUserName.Text) == false)
                {
                    //var pass = MD5.Crypt(tbPassword.Text);
                    byte[] bytes = Encoding.ASCII.GetBytes(tbPassword.Text);
                    var pass = MD5.Crypt(bytes);
                    string res = BitConverter.ToString(pass);
                   // MessageBox.Show(res);
                    bool b = client.Register(tbUserName.Text, res);
                    if (b==true)
                    {
                        MessageBox.Show("Nalog je kreiran");
                        tbRegister.Clear();
                        cbRegister.Checked = false;
                    }
                    else
                    {
                        MessageBox.Show("Nalog nije kreiran");
                    }
                }
                else
                {
                    MessageBox.Show("Korinicko ime je zauzeto");
                }
            }
            else
            {
                MessageBox.Show("Pogresna potvrad sifre");
            }

        }
    }
}
