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
    public partial class DataTrack : UserControl
    {
        /// <summary>
        /// 操作记录数据源
        /// </summary>
        public DataTable DT_OperationRecord
        {
            //get { }
            set
            {
                this.dataGridView1.DataSource = value;
            }
        }

        /// <summary>
        /// 操作数据源
        /// </summary>
        public DataTable DT_Data
        {
            //get;
            set
            {
                this.dataGridView2.DataSource = value;
                // this.Height = GetDataGridViewHeight(dataGridView2);
            }
        }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBaseName { get; set; }

        /// <summary>
        /// 数据表名称
        /// </summary>
        public string DataTableName { get; set; }

        /// <summary>
        /// 选中行
        /// </summary>
        public DataRow Row_SelectRow { get; private set; }

        public delegate void SelectRowHander(object sender, DataRow row);
        /// <summary>
        /// 选中行事件
        /// </summary>
        public event SelectRowHander SelectRowEvent;

        public DataTrack()
        {
            InitializeComponent();
            //ShowCheck(dataGridView2);
        }

        /// <summary>
        /// 添加复选框
        /// </summary>
        /// <param name="gv"></param>
        private void ShowCheck(DataGridView gv)
        {
            //为dgv增加复选框列
            DataGridViewCheckBoxColumn checkbox = new DataGridViewCheckBoxColumn();
            //列显示名称
            checkbox.HeaderText = "选择";
            checkbox.Name = "IsChecked";
            checkbox.TrueValue = true;
            checkbox.FalseValue = false;
            checkbox.DataPropertyName = "IsChecked";
            //列宽
            checkbox.Width = 50;
            //列大小不改变
            checkbox.Resizable = DataGridViewTriState.False;
            //checkbox.Selected
            //添加的checkbox在dgv第一列
            gv.Columns.Insert(0, checkbox);
        }

        int GetDataGridViewHeight(DataGridView dataGridView)
        {
            var sum = (dataGridView.ColumnHeadersVisible ? dataGridView.ColumnHeadersHeight : 0) +
                      dataGridView.Rows.OfType<DataGridViewRow>().Where(r => r.Visible).Sum(r => r.Height);

            return sum;
        }

        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            this.Row_SelectRow = (this.dataGridView2.CurrentRow.DataBoundItem as DataRowView).Row;
            if (this.SelectRowEvent != null)
            {
                SelectRowEvent(this, this.Row_SelectRow);
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
