using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlServerDataMove
{
    public partial class Form2 : Form
    {
        public string Context { get; set; }

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Context))
            {
                textBox1.Text = Context;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Context = textBox1.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
