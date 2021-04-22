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
    /// <summary>
    /// 210413 代红明 字段对应控件
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private string fromText;
        /// <summary>
        /// from字段名
        /// </summary>
        public string FromText
        {
            get { return fromText; }
            set
            {
                fromText = value;
                this.textBox1.Text = value;
            }
        }


        private string toText;
        /// <summary>
        /// to字段名
        /// </summary>
        public string ToText
        {
            get { return toText; }
            set
            {
                toText = value;
                this.textBox2.Text = value;
            }
        }

        private string fromStyle = "From字段";
        /// <summary>
        /// 来源类型
        /// </summary>
        public string FromStyle
        {
            get { return fromStyle; }
            set
            {
                fromStyle = value;
                comboBox1.Text = value.ToString();
            }
        }

        private string toMemo;
        /// <summary>
        /// 目标字段注释(只有从目标表生成,才有效)
        /// </summary>
        public string ToMemo
        {
            get { return toMemo; }
            set
            {
                toMemo = value;
                textBox2.Tag = value;
            }
        }

        public UserControl1()
        {
            InitializeComponent();
            comboBox1.Items.Add("From字段");
            comboBox1.Items.Add("From固定值");
            comboBox1.Items.Add("FromSql");
            comboBox1.Items.Add("自增");//不写
            comboBox1.Items.Add("映射步骤");//来源于主表ID(先写主表步骤)
            comboBox1.Text = FromStyle;

            MenuItem item1 = new MenuItem("删除");
            MenuItem item2 = new MenuItem("取消");
            item1.Click -= new EventHandler(menuClick);
            item1.Click += new EventHandler(menuClick);
            item2.Click -= new EventHandler(menuClick);
            item2.Click += new EventHandler(menuClick);
            ContextMenu menu = new ContextMenu(new MenuItem[2] { item1, item2 });

            comboBox1.ContextMenu = menu;
            textBox1.ContextMenu = menu;
            label2.ContextMenu = menu;
            textBox2.ContextMenu = menu;

        }

        /// <summary>
        /// 右键菜单点击
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        private void menuClick(object obj, EventArgs e)
        {
            MenuItem item = obj as MenuItem;
            if (item.Text != "删除")
                return;
            ContextMenu menu = item.GetContextMenu();
            Control con = menu.SourceControl;
            UserControl parent1 = con.Parent as UserControl;
            Control panel = parent1.Parent;
            panel.Controls.Remove(this);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.FromText = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.ToText = textBox2.Text;
        }

        /// <summary>
        /// 设置下拉框颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "From字段":
                    textBox1.Click -= new System.EventHandler(this.textBox1_Click);
                    comboBox1.BackColor = Color.FromArgb(128, 128, 255);
                    break;
                case "From固定值"://文本类型一定要加''
                    textBox1.Click -= new System.EventHandler(this.textBox1_Click);
                    comboBox1.BackColor = Color.FromArgb(192, 255, 255);
                    break;
                case "FromSql"://sql语句一定要加数据库名.表名
                    textBox1.Click -= new System.EventHandler(this.textBox1_Click);
                    textBox1.Click += new System.EventHandler(this.textBox1_Click);
                    comboBox1.BackColor = Color.FromArgb(192, 255, 192);
                    break;
                case "自增"://对应原来表的自增
                    textBox1.Click -= new System.EventHandler(this.textBox1_Click);
                    comboBox1.BackColor = Color.FromArgb(255, 255, 192);
                    break;
                case "映射步骤"://用原来表外键,对应新表返回值
                    textBox1.Click -= new System.EventHandler(this.textBox1_Click);
                    comboBox1.BackColor = Color.FromArgb(255, 128, 128);
                    break;
                default:
                    break;
            }
            FromStyle = comboBox1.Text;
        }

        /// <summary>
        /// 禁止下拉框输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;//取消输入事件
        }

        /// <summary>
        /// 来源字段单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_Click(object sender, EventArgs e)
        {
            LongStringForm form = new LongStringForm();
            form.Text = (sender as Control).Text;
            form.ShowDialog();
            (sender as Control).Text = form.Text;
        }

        /// <summary>
        /// 显示目标字段注释
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_MouseEnter(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box.Tag == null)
                return;
            Label memo = new Label()
            {
                AutoSize = false,
                Width = textBox2.Width,
                Height = this.Height,
                BorderStyle = BorderStyle.FixedSingle,
                Font = textBox2.Font,
                Visible = false,
                Dock = DockStyle.Right,
                BackColor = Color.Silver,
                ForeColor = Color.Black
            };
            memo.Text = box.Tag.ToString();
            memo.ContextMenu = box.ContextMenu;
            box.Visible = false;
            box.ContextMenu = null;
            memo.Visible = true;
            memo.Tag = box;
            memo.MouseLeave -= new EventHandler(memo_MouseLeave);
            memo.MouseLeave += new EventHandler(memo_MouseLeave);
            this.Controls.Add(memo);
        }

        /// <summary>
        /// 隐藏目标字段注释
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void memo_MouseLeave(object sender, EventArgs e)
        {
            Label lab = sender as Label;
            TextBox box = (lab.Tag) as TextBox;
            lab.Visible = false;
            box.Visible = true;
            box.ContextMenu = lab.ContextMenu;
            lab.ContextMenu = null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
