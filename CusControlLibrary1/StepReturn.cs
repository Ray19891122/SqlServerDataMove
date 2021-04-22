using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CusControlLibrary1
{
    public partial class StepReturn : UserControl
    {
        private int selectStep;
        /// <summary>
        /// 选中对象(步骤索引)
        /// </summary>
        public int SelectStep
        {
            get { return selectStep; }
            set
            {
                selectStep = value;
                //this.comboBox2.Text = value; 
            }
        }

        private List<int> stepIndexs;
        /// <summary>
        /// 前步骤索引集合
        /// </summary>
        public List<int> StepIndexs
        {
            get { return stepIndexs; }
            set
            {
                stepIndexs = value;
                if (stepIndexs != null)
                    comboBox2.Items.AddRange(stepIndexs.Select(x => x.ToString()).ToArray());
            }
        }

        /// <summary>
        /// 值改变事件
        /// </summary>
        public event EventHandler ValueChange;

        public StepReturn()
        {
            InitializeComponent();
            this.label1.Text = "映射步骤";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectStep = Convert.ToInt16(comboBox2.Text);
            if (ValueChange != null)
            {
                ValueChange(this, new EventArgs());
            }
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
            return;
        }
    }
}
