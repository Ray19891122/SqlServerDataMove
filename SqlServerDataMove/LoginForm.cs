using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using IntegrateHelp;

namespace SqlServerDataMove
{
    public partial class LoginForm : Form
    {
        string ip = "127.0.0.1";

        string sUser = "";

        string sPassword = "";

        string sConntionString = "server={0};database={1};uid={2};pwd={3}";

        public LoginForm(string[] pars = null)
        {
            InitializeComponent();
            if (pars != null && pars.Length > 0)
            {
                textBox3.Text = pars[0];
                textBox1.Text = pars[1];
                textBox2.Text = pars[2];
            }
            else
            {
                textBox3.Text = ".";
                textBox1.Text = "sa";
                textBox2.Text = "123";
            }
            textBox3.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.SteelBlue;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Red;
        }

        private bool LoginCheck()
        {
            sUser = textBox1.Text;
            sPassword = textBox2.Text;
            ip = textBox3.Text;
            bool bCheck = true;
            if (String.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("\n请输入sqlsqlver地址！", "提示");
                bCheck = false;
            }
            if (String.IsNullOrWhiteSpace(sUser))
            {
                MessageBox.Show("\n请输入sqlsqlver账号！", "提示");
                bCheck = false;
            }
            if (String.IsNullOrWhiteSpace(sPassword))
            {
                MessageBox.Show("\n请输入sqlsqlver密码！", "提示");
                bCheck = false;
            }
            return bCheck;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label5_Click(object sender, EventArgs e)
        {
            if (!LoginCheck())
                return;
            string conntionString = string.Format(sConntionString, ip, "master", sUser, sPassword);
            try
            {
                DbHelper db = new SqlClientHelper(conntionString);
                db.ExecuteNonQuery("select 1 ");
                this.Hide();
                Form1 form = new Form1();
                form.User = sUser;
                form.Password = sPassword;
                form.Ip = ip;
                form.SConntionString = this.sConntionString;
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label5_Click(this, null);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }
    }
}
