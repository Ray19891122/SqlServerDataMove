using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CusControlLibrary1
{
    public partial class LongStringForm : Form
    {
        public new string Text { get; set; }

        public LongStringForm()
        {
            InitializeComponent();
        }

        private void LongStringForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = this.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Text = textBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
