using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using IntegrateHelp;

namespace CusControlLibrary1
{
    /// <summary>
    /// 步骤控件
    /// </summary>
    public partial class StepControl : UserControl
    {
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
                this.stepReturn1.StepIndexs = stepIndexs;
            }
        }

        private int selectStep;
        /// <summary>
        /// 类型1值
        /// </summary>
        public int SelectStep
        {
            get { return selectStep; }
            set
            {
                selectStep = value;
                this.stepReturn1.SelectStep = value;
            }
        }

        private int index;
        /// <summary>
        /// 步骤索引
        /// </summary>
        public int Index
        {
            get { return index; }
            private set
            {
                index = value;

            }
        }

        /// <summary>
        /// 数据库映射类
        /// </summary>
        public DataSourceConfig DataSourceConfig { get; private set; }

        /// <summary>
        /// 数据表映射
        /// </summary>
        public TableConfig TableConfig { get; private set; }

        /// <summary>
        /// 字段映射
        /// </summary>
        public List<FieldConfig> FieldConfig { get; private set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        public string User { get; set; }
        public string Password { get; set; }


        public string SConntionString { get; set; }

        private string sourcePath;
        /// <summary>
        /// 数据源路径
        /// </summary>
        public string SourcePath
        {
            get { return sourcePath; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    return;
                sourcePath = value;
                XmlDocument doc = new XmlDocument();
                doc.Load(sourcePath);
                LoadXml(doc);
                this.textBox1.Text = sourcePath;
            }
        }

        public StepControl(int index)
        {
            if (index < 0)
                throw new Exception("索引不可为空！");
            InitializeComponent();
            stepReturn1.ValueChange -= new EventHandler(On_StepReturn1ValueChange);
            stepReturn1.ValueChange += new EventHandler(On_StepReturn1ValueChange);
            this.Index = index;
            button1.Text = "步骤" + index.ToString();
        }

        private void On_StepReturn1ValueChange(object sender, EventArgs e)
        {
            StepReturn stepReturn = sender as StepReturn;
            this.SelectStep = stepReturn.SelectStep;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string path = OpenConfig();
            if (string.IsNullOrWhiteSpace(path))
                return;
            this.SourcePath = path;
        }

        /// <summary>
        /// 通过连接语句与表名获取表结构信息
        /// </summary>
        /// <param name="conntionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DataTable GetTableInfo(string conntionString, string tableName)
        {

            DbHelper db = new SqlClientHelper(conntionString);
            string sqlString = @" SELECT ColumnsName = c.name ,
        Tag = isnull(CAST(ex.value AS VARCHAR(2000)),'') + '_' + t.name
 FROM   sys.columns c
        LEFT OUTER JOIN sys.extended_properties ex ON ex.major_id = c.object_id
                                                      AND ex.minor_id = c.column_id
                                                      AND ex.name = 'MS_Description'
        LEFT OUTER JOIN systypes t ON c.system_type_id = t.xtype
 WHERE  OBJECTPROPERTY(c.object_id, 'IsMsShipped') = 0
        AND OBJECT_NAME(c.object_id) = '" + tableName.Trim() + "';";
            return db.Fill(sqlString);
        }

        /// <summary>
        /// 装在配置文件
        /// </summary>
        /// <param name="doc"></param>
        private void LoadXml(XmlDocument doc)
        {
            //得到顶层节点列表
            XmlNodeList topM = doc.DocumentElement.ChildNodes;
            foreach (XmlElement element in topM)
            {
                if (element.Name.ToLower() == "datasource")
                {
                    DataSourceConfig database = new DataSourceConfig()
                    {
                        FromSource = element.Attributes["FromSource"].Value,
                        ToSource = element.Attributes["ToSource"].Value
                    };
                    this.DataSourceConfig = database;
                }
                else if (element.Name.ToLower() == "table")
                {
                    TableConfig dataTable = new TableConfig()
                    {
                        FromTable = element.Attributes["FromTable"].Value,
                        ToTable = element.Attributes["ToTable"].Value
                    };
                    this.TableConfig = dataTable;
                }
                else if (element.Name.ToLower() == "field")
                {
                    List<FieldConfig> fields = new List<FieldConfig>();
                    XmlNodeList top = element.ChildNodes;
                    foreach (XmlElement xlt in top)
                    {
                        FieldConfig field = new FieldConfig()
                        {
                            FromStyle = xlt.Attributes["FromStyle"].Value,
                            FromField = xlt.Attributes["FromField"].Value,
                            ToField = xlt.Attributes["ToField"].Value
                        };
                        fields.Add(field);
                    }
                    this.FieldConfig = fields;
                }
            }
        }

        /// <summary>
        /// 获取配置地址
        /// </summary>
        /// <returns></returns>
        private string OpenConfig()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "文档文件(*.xml)|*.xml";
            if (open.ShowDialog() != DialogResult.OK)
                return string.Empty;
            if (string.IsNullOrWhiteSpace(open.FileName))
                return string.Empty;
            return open.FileName;
        }

        private void stepReturn2_Load(object sender, EventArgs e)
        {

        }
    }
}
