using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using CusControlLibrary1;
using IntegrateHelp;
using System.Data.SqlClient;
using System.Data.Common;
using GeneralTools.SystemCommon;

namespace SqlServerDataMove
{
    public partial class Form1 : Form
    {
        public string Ip { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public string SConntionString { get; set; }

        private int index = 0;
        private List<int> lst_index = new List<int>();

        private Dictionary<int, List<KeyClass>> keyDictionary = null;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 保存检查
        /// </summary>
        /// <returns></returns>
        private bool DoSaveCheck()
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("来源数据库不可为空！");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("目标数据库不可为空！");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("来源表不可为空！");
                return false;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("目标表不可为空！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 保存当前配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!DoSaveCheck())
                return;
            if (flowLayoutPanel1.Controls.Count <= 0)
            {
                MessageBox.Show("没有任何字段!");
                return;
            }

            //创建XmlDocument对象
            XmlDocument xmlDoc = new XmlDocument();
            //XML的声明<?xml version="1.0" encoding="gb2312"?>
            XmlDeclaration xmlSM = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //追加xmldecl位置
            xmlDoc.AppendChild(xmlSM);
            //添加一个名为Gen的根节点
            XmlElement xml = xmlDoc.CreateElement("", "DataMove", "");
            //追加Gen的根节点位置
            xmlDoc.AppendChild(xml);

            //添加一个名为<DataSource>的节点,并设置数据库对应属性
            XmlNode gen = xmlDoc.SelectSingleNode("DataMove");
            XmlElement dataSource = xmlDoc.CreateElement("DataSource");
            //为<Zi>节点的属性
            dataSource.SetAttribute("FromSource", textBox4.Text.Trim());
            dataSource.SetAttribute("ToSource", textBox3.Text.Trim());
            gen.AppendChild(dataSource);//添加到<Gen>节点中

            //添加一个名为<table>的节点,并设置表对应属性
            XmlElement table = xmlDoc.CreateElement("Table");
            //为<Zi>节点的属性
            table.SetAttribute("FromTable", textBox1.Text.Trim());
            table.SetAttribute("ToTable", textBox2.Text.Trim());
            gen.AppendChild(table);//添加到<Gen>节点中

            //添加一个名为<Field>的节点，不具有属性,只有元素(类)
            XmlElement field = xmlDoc.CreateElement("Field");
            gen.AppendChild(field);//添加到<Gen>节点中

            int index = 0;
            foreach (Control row in flowLayoutPanel1.Controls)
            {
                if (!(row is UserControl1))
                    continue;
                UserControl1 usCon = row as UserControl1;
                if (string.IsNullOrWhiteSpace(usCon.FromText) && string.IsNullOrWhiteSpace(usCon.ToText))
                    continue;
                if (!string.IsNullOrWhiteSpace(usCon.FromText) && string.IsNullOrWhiteSpace(usCon.ToText))
                {
                    MessageBox.Show("来源字段【" + usCon.FromText + "】未指定目标!");
                    return;
                }
                //if (string.IsNullOrWhiteSpace(usCon.FromText) && !string.IsNullOrWhiteSpace(usCon.ToText))
                //{
                //   MessageBox.Show("目标字段【" + usCon.ToText + "】未指定来源!");
                //}
                if (string.IsNullOrWhiteSpace(usCon.FromStyle))
                {
                    MessageBox.Show("目标字段【" + usCon.ToText + "】未指定来源类型!");
                    return;
                }
                index++;
                if (string.IsNullOrWhiteSpace(usCon.FromText))
                    usCon.FromText = string.Empty;
                AddFieldXml(xmlDoc, field, "Field" + index, usCon.FromStyle.Trim(), usCon.FromText.Trim(), usCon.ToText.Trim());
            }
            if (index == 0)
            {
                MessageBox.Show("没有任何需要保存的字段对应!");
                return;
            }

            DoSave(xmlDoc);
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="xmlDoc"></param>
        private void DoSave(XmlDocument xmlDoc)
        {
            //string localFilePath, fileNameExt, newFileName, FilePath;
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型
            sfd.Filter = "*.xml|配置文件（*.xml";

            //设置默认文件类型显示顺序
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录
            sfd.RestoreDirectory = true;

            //点了保存按钮进入
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径

                //获取文件路径，不带文件名
                //FilePath = localFilePath.Substring(0, localFilePath.LastIndexOf("\\"));

                //给文件名前加上时间
                //newFileName = DateTime.Now.ToString("yyyyMMdd") + fileNameExt;

                //在文件名里加字符
                //saveFileDialog1.FileName.Insert(1,"dameng");

                //System.IO.FileStream fs = (System.IO.FileStream)sfd.OpenFile();//输出文件

                ////fs输出带文字或图片的文件，就看需求了
                try
                {
                    xmlDoc.Save(localFilePath);
                    MessageBox.Show("配置保存成功！");
                }
                catch (Exception)
                {
                    MessageBox.Show("配置保存失败！");
                }

            }
        }

        /// <summary>
        /// 增加字段对应节点,并设置字段对应属性
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="table"></param>
        /// <param name="elementName"></param>
        /// <param name="fromfieldName"></param>
        /// <param name="toFieldName"></param>
        private void AddFieldXml(XmlDocument xmlDoc, XmlElement table, string elementName, string fromStyle, string fromfieldName, string toFieldName)
        {
            //为table节点增加新节点,并设置属性 <Field name="博客园" age="26">
            XmlElement field = xmlDoc.CreateElement(elementName);
            //为<Zi>节点的属性
            field.SetAttribute("FromStyle", fromStyle);
            field.SetAttribute("FromField", fromfieldName);
            field.SetAttribute("ToField", toFieldName);
            table.AppendChild(field);

            //节点增加串联节点  <title>C#从入门到放弃</title>
            //XmlElement x1 = xmlDoc.CreateElement("title");
            ////InnerText:获取或设置节点及其所有子节点的串连值
            //x1.InnerText = "C#从入门到放弃";
            //zi.AppendChild(x1);//添加到<Zi>节点中
            //XmlElement x2 = xmlDoc.CreateElement("unit");
            //x2.InnerText = "第一讲，如何放弃";
            //zi.AppendChild(x2);
            //XmlElement x3 = xmlDoc.CreateElement("fm");
            //x3.InnerText = "123.06兆赫";
            //zi.AppendChild(x3);
            //重点End
        }

        /// <summary>
        /// 新增字段对应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            UserControl1 fieldControl = new UserControl1();
            flowLayoutPanel1.Controls.Add(fieldControl);
        }

        /// <summary>
        /// 按目标表自动生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("目标数据库不可为空！");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("目标表不可为空！");
                return;
            }
            string conntionString = string.Format(SConntionString, Ip, textBox3.Text, User, Password);
            DataTable dt = GetTableInfo(conntionString, textBox2.Text.Trim());
            AddFieldControl(dt);
        }

        /// <summary>
        /// 打开xml配置
        /// </summary>
        /// <returns></returns>
        private XmlDocument OpenConfig()
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "文档文件(*.xml)|*.xml";
            if (open.ShowDialog() != DialogResult.OK)
                return null;
            if (string.IsNullOrWhiteSpace(open.FileName))
                return null;

            XmlDocument doc = new XmlDocument();
            doc.Load(open.FileName);
            return doc;
        }

        /// <summary>
        /// 导入配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            XmlDocument doc = OpenConfig();
            if (doc == null)
                return;

            XmlNode xn = doc.SelectSingleNode("DataMove");//获取根节点
            if (xn == null)
            {
                MessageBox.Show("配置文件类型不匹配,请确认！");
                return;
            }
            XmlNodeList xnl = xn.ChildNodes;//获取根节点的所有子节点

            DataSourceConfig dataSource = new DataSourceConfig();
            TableConfig table = new TableConfig();
            List<FieldConfig> fields = new List<FieldConfig>();

            foreach (XmlNode xn1 in xnl)
            {
                // 将节点转换为元素，便于得到节点的属性值
                XmlElement xe = (XmlElement)xn1;
                if (xe.Name == "DataSource")
                {
                    dataSource.FromSource = xe.GetAttribute("FromSource").ToString();
                    dataSource.ToSource = xe.GetAttribute("ToSource").ToString();
                }

                if (xe.Name == "Table")
                {
                    table.FromTable = xe.GetAttribute("FromTable").ToString();
                    table.ToTable = xe.GetAttribute("ToTable").ToString();
                }

                if (xe.Name == "Field")
                {
                    XmlNodeList xnList = xe.ChildNodes;

                    foreach (XmlNode xn1l in xnList)
                    {
                        XmlElement xel = (XmlElement)xn1l;
                        FieldConfig field = new FieldConfig();
                        field.FromStyle = xel.GetAttribute("FromStyle").ToString();
                        field.FromField = xel.GetAttribute("FromField").ToString();
                        field.ToField = xel.GetAttribute("ToField").ToString();
                        fields.Add(field);
                    }
                }
            }
            textBox4.Text = dataSource.FromSource;
            textBox3.Text = dataSource.ToSource;

            textBox1.Text = table.FromTable;
            textBox2.Text = table.ToTable;

            AddFieldControl(fields);
        }

        /// <summary>
        /// 按照配置文件增加字段对应控件
        /// </summary>
        /// <param name="fields"></param>
        private void AddFieldControl(List<FieldConfig> fields)
        {
            this.flowLayoutPanel1.Controls.Clear();
            foreach (FieldConfig field in fields)
            {
                UserControl1 fieldControl = new UserControl1();
                fieldControl.FromStyle = field.FromStyle;
                fieldControl.FromText = field.FromField;
                fieldControl.ToText = field.ToField;
                flowLayoutPanel1.Controls.Add(fieldControl);
            }
        }

        /// <summary>
        /// 按照表字段增加字段对应控件
        /// </summary>
        /// <param name="dt"></param>
        private void AddFieldControl(DataTable dt)
        {
            if (dt.Rows.Count <= 0)
                return;
            this.flowLayoutPanel1.Controls.Clear();
            foreach (DataRow row in dt.Rows)
            {
                UserControl1 fieldControl = new UserControl1();
                fieldControl.ToText = row["ColumnsName"].ToString();
                fieldControl.ToMemo = row["Tag"].ToString();
                flowLayoutPanel1.Controls.Add(fieldControl);
            }
        }

        /// <summary>
        /// 按照追踪表的值设置字段对应(方案:来源表与目标表字段值相同,并且值不为空,不为0)
        /// </summary>
        /// <param name="fromRow">追踪来源表数据行</param>
        /// <param name="toTable">目标表结构信息表</param>
        /// <param name="ToRow">追踪目标表数据行</param>
        private void AddFieldControl(DataRow fromRow, DataTable toTable, DataRow ToRow)
        {
            if (fromRow == null || toTable.Rows.Count <= 0 || ToRow == null)
                return;
            this.flowLayoutPanel1.Controls.Clear();
            foreach (DataRow row in toTable.Rows)
            {
                UserControl1 fieldControl = new UserControl1();
                fieldControl.ToText = row["ColumnsName"].ToString();
                fieldControl.ToMemo = row["Tag"].ToString();
                if (ToRow.Table.Columns.Contains(row["ColumnsName"].ToString()))
                {
                    string toValue = ToRow[row["ColumnsName"].ToString()].ToString();
                    foreach (DataColumn col in fromRow.Table.Columns)
                    {
                        string fromValue = fromRow[col.ColumnName].ToString();
                        if (col.ColumnName != "ID" && !string.IsNullOrWhiteSpace(fromValue) && fromValue != "0" && fromValue == toValue)
                        {
                            fieldControl.FromStyle = "From字段";
                            fieldControl.FromText = col.ColumnName;
                            break;
                        }
                    }
                }
                flowLayoutPanel1.Controls.Add(fieldControl);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            HookTool.Hook_Clear();
            Application.Exit();
        }

        /// <summary>
        /// sql追踪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            string conntionString = string.Format(SConntionString, Ip, "master", User, Password);
            DbHelper db = new SqlClientHelper(conntionString);
            DataTrackForm data = new DataTrackForm();
            data.SConntionString = this.SConntionString;
            data.IP = this.Ip;
            data.User = this.User;
            data.PassWord = this.Password;
            data.DB = db;
            data.StartTime = DateTime.Now;
            data.ShowDialog();
            if (data.DialogResult != DialogResult.OK)
                return;
            this.textBox4.Text = data.FromDataBaseName;
            this.textBox3.Text = data.ToDataBaseName;
            this.textBox1.Text = data.FromDataTableName;
            this.textBox2.Text = data.ToDataTableName;
            conntionString = string.Format(SConntionString, Ip, textBox3.Text, User, Password);
            DataTable dt = GetTableInfo(conntionString, textBox2.Text.Trim());
            AddFieldControl(data.FromRow, dt, data.ToRow);
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
            string sqlString = @" SELECT  ColumnsName = a.name ,
        Tag = CAST(ISNULL(g.[value], ' ') AS VARCHAR(200)) + '_' + b.name
FROM syscolumns a
     LEFT JOIN systypes b ON a.xtype = b.xusertype
        INNER JOIN sysobjects d ON a.id = d.id
                                   AND d.xtype = 'U'
                                   AND d.name <> 'dtproperties'
        LEFT JOIN syscomments e ON a.cdefault = e.id
        LEFT JOIN sys.extended_properties g ON a.id = g.major_id
                                               AND a.colid = g.minor_id
        LEFT JOIN sys.extended_properties f ON d.id = f.class
                                               AND f.minor_id = 0
WHERE b.name IS NOT NULL
        AND d.name = '" + tableName + @"'
ORDER BY a.id ,
        a.colorder;";
            return db.Fill(sqlString);
        }

        /// <summary>
        /// 添加步骤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            index++;
            StepControl step = new StepControl(index);
            step.IP = this.Ip;
            step.User = this.User;
            step.Password = this.Password;
            step.SConntionString = this.SConntionString;
            step.StepIndexs = lst_index;
            lst_index.Add(index);
            step.Dock = DockStyle.Top;
            this.flowLayoutPanel2.Controls.Add(step);
        }

        /// <summary>
        /// 执行步骤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button6_Click(object sender, EventArgs e)
        {
            keyDictionary = new Dictionary<int, List<KeyClass>>();
            textBox5.Text = string.Empty;
            await Task.Run(new Action(() =>
            {
                List<StepControl> lst_step = new List<StepControl>();
                foreach (Control con in flowLayoutPanel2.Controls)
                {
                    if (!(con is StepControl))
                        continue;
                    StepControl step = con as StepControl;
                    lst_step.Add(step);
                }
                lst_step = lst_step.OrderBy(x => x.Index).ToList();
                string contionString = string.Format(this.SConntionString, this.Ip, "master", this.User, this.Password);
                DbHelper db = new SqlClientHelper(contionString);
                var var = lst_step.FirstOrDefault(x => x.DataSourceConfig == null);
                if (var != null)
                {
                    MessageBox.Show("步骤配置异常！");
                    return;
                }
                string mess = string.Empty;
                string insertString = string.Empty;
                List<string> sqlMess = new List<string>();
                //分解步骤
                foreach (StepControl step in lst_step)
                {
                    try
                    {
                        mess += "\r\n\r\n开始执行步骤" + step.Index + "\r\n\r\n";
                        List<KeyClass> upStepReturn = new List<KeyClass>();
                        if (step.SelectStep > 0)//映射步骤
                        {
                            upStepReturn = keyDictionary.FirstOrDefault(x => x.Key == Convert.ToInt16(step.SelectStep)).Value;
                            if (upStepReturn == null)
                                throw new Exception("需要映射的步骤未找到绑定的步骤!");
                        }
                        string selectString = string.Format(" select * from {0}.dbo.{1} ", step.DataSourceConfig.FromSource, step.TableConfig.FromTable);
                        DataTable dt = db.Fill(selectString);
                        mess += "步骤" + step.Index + " 来源表:" + step.DataSourceConfig.FromSource + "." + step.TableConfig.FromTable + " 共计:" + dt.Rows.Count + " 行数据";
                        //分解步骤数据行
                        int haneld = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            KeyClass key = new KeyClass();
                            List<string> lst_fields = new List<string>();
                            List<object> lst_values = new List<object>();
                            Run(db, step, key, row, upStepReturn, lst_fields, lst_values);
                            insertString = string.Format(" insert into {0}.dbo.{1}({2})values({3}) SELECT @@IDENTITY ", step.DataSourceConfig.ToSource, step.TableConfig.ToTable, string.Join(",", lst_fields), string.Join(",", lst_values));
                            object flag = null;
                            if (checkBox2.Checked)
                            {
                                flag = db.ExecuteScalar<object>(insertString);
                                if (!string.IsNullOrWhiteSpace(key.OldField))
                                {
                                    key.NewValue = flag;
                                    if (!keyDictionary.Keys.Contains(step.Index))
                                        keyDictionary.Add(step.Index, new List<KeyClass>());
                                    keyDictionary[step.Index].Add(key);
                                }
                            }
                            else
                            {
                                sqlMess.Add(insertString);
                            }
                            haneld++;
                            textBox5.Invoke(
                                new Action<string>
                                ((a) =>
                                {
                                    textBox5.Text = a + "\r\n" + haneld;
                                })
                                , mess);
                        }
                        if (checkBox1.Checked)
                        {
                            Form form = new Form();
                            form.Width = 500;
                            form.Height = 500;
                            form.StartPosition = FormStartPosition.CenterScreen;
                            TextBox tbx = new TextBox();
                            tbx.Multiline = true;
                            tbx.Dock = DockStyle.Fill;
                            tbx.Text = string.Join("\r\n", sqlMess);
                            form.Controls.Add(tbx);
                            form.ShowDialog();
                        }
                        else
                        {
                            DialogResult diac = MessageBox.Show("步骤" + step.Index + " 执行成功!是否继续后续步骤?", "提示", MessageBoxButtons.YesNo);
                            if (diac != DialogResult.Yes)
                                return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("执行失败！\r\n" + ex.ToString() + "\r\nSQL语句已经输出到消息");
                        textBox5.Invoke(new Action(() => { textBox5.Text += "\r\n\r\n错误语句:" + insertString; }));
                        return;
                    }
                }
            }));
        }

        private void Run(DbHelper db, StepControl step, KeyClass key, DataRow row, List<KeyClass> upStepReturn, List<string> lst_fields, List<object> lst_values)
        {
            foreach (var item in step.FieldConfig)
            {
                switch (item.FromStyle)
                {
                    case "自增":
                        key.Step = step.Index;
                        key.OldField = item.FromField;
                        key.NewField = item.ToField;
                        key.OldValue = row[item.FromField];
                        break;
                    case "映射步骤":
                        KeyClass oldKey = upStepReturn.FirstOrDefault(x => x.OldValue.ToString() == row[item.FromField].ToString());
                        lst_fields.Add(item.ToField);
                        if (oldKey != null)
                            lst_values.Add(oldKey.NewValue);
                        else
                            throw new Exception("映射的步骤未找到绑定步骤的返回匹配,不能建立强关系!");//未找到原主表ID,与新表ID不能建立绑定关系
                        break;
                    case "FromSql": //sql条件示例: select name from aoyin.dbo.sys_bdstaff where ID=[原表ID] and name =[原表name] 表名前面一定要带数据库名,条件文本也不用带''
                        lst_fields.Add(item.ToField);
                        string sqlString = string.Empty;
                        if (item.FromField.Contains("[") && item.FromField.Contains("]"))
                        {
                            List<string> lst_where = GetAllSubstring(item.FromField, "[", "]").Distinct().ToList();
                            Dictionary<string, object> dic_where = lst_where
                                                                  .Select(x => x)
                                                                  .ToDictionary(x => x, x => row[x]);
                            foreach (string field in dic_where.Keys)
                            {
                                sqlString = item.FromField.Replace("[" + field + "]", "'" + dic_where[field].ToString() + "'");
                            }
                        }
                        object obj = db.ExecuteScalar<object>(sqlString);
                        if (obj != null)
                            lst_values.Add("'" + obj.ToString() + "'");
                        else
                            lst_values.Add("'0'");
                        break;
                    case "From固定值":
                        if (string.IsNullOrWhiteSpace(item.FromField))
                            continue;
                        lst_fields.Add(item.ToField);
                        lst_values.Add(item.FromField);
                        break;
                    case "From字段":
                        if (string.IsNullOrWhiteSpace(item.FromField))
                            continue;
                        lst_fields.Add(item.ToField);
                        lst_values.Add("'" + row[item.FromField].ToString() + "'");
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 截取两个指定字符串中间的字符串列表(开始和结束两个字符串不能相同！)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="startStr"></param>
        /// <param name="endStr"></param>
        /// <returns></returns>
        public static List<string> GetAllSubstring(string content, string startStr, string endStr)
        {
            List<string> resultList = new List<string>();

            int len = content.Length;
            int startLen = startStr.Length;
            int endLen = endStr.Length;
            for (var i = 0; i < len; i++)
            {
                string a = startStr.Substring(0, 1);
                if (content[i].ToString() == a)
                {
                    int startIndex = (i + startLen - 1);
                    if (startIndex < len)
                    {
                        a = content.Substring(i, startLen);
                        if (a.Equals(startStr))
                        {
                            // 循环找出结尾匹配
                            for (int endIndex = startIndex; endIndex < len; endIndex++)
                            {
                                var str = "";
                                for (int j = 0; j < endLen; j++)
                                {
                                    str += content[endIndex].ToString();
                                }
                                if (str == endStr && endStr != startStr)
                                {
                                    // 得到长度
                                    int splLen = endIndex - startIndex;
                                    string result = content.Substring(startIndex + 1, splLen - 1);
                                    resultList.Add(result);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return resultList;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = !checkBox1.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            HookTool.Hook_Clear();
            HookTool.Hook_Start();
            HookTool.KeyPress -= new HookTool.KeyPressHandler(KeyPress);
            HookTool.KeyPress += new HookTool.KeyPressHandler(KeyPress);
        }

        Form2 form = null;
        private string str = string.Empty;
        private new void KeyPress(Keys keys)
        {
            if (keys != Keys.F)
                return;
            if (form == null)
            {
                form = new Form2();
            }
            if (form.Visible)
                return;
            form.Context = str;
            form.ShowDialog();
            if (form.DialogResult != DialogResult.OK)
                return;
            str = form.Context;
            foreach (UserControl1 con in flowLayoutPanel1.Controls)
            {
                if (!string.IsNullOrWhiteSpace(form.Context))
                {
                    if (con.ToText == str || con.FromText == str)
                        con.Visible = true;
                    else
                        con.Visible = false;
                }
                else
                    con.Visible = true;
            }
        }

    }

    /// <summary>
    /// 数据库对应配置
    /// </summary>
    public class DataSourceConfig
    {
        /// <summary>
        /// 来源数据库
        /// </summary>
        public string FromSource { get; set; }

        /// <summary>
        /// 目标数据库
        /// </summary>
        public string ToSource { get; set; }
    }

    /// <summary>
    /// 数据库对应配置
    /// </summary>
    public class TableConfig
    {
        /// <summary>
        /// 来源表名
        /// </summary>
        public string FromTable { get; set; }

        /// <summary>
        /// 目标表名
        /// </summary>
        public string ToTable { get; set; }
    }

    /// <summary>
    /// 数据库对应配置
    /// </summary>
    public class FieldConfig
    {
        /// <summary>
        /// 目标来源类型(From字段,From固定值,FromSql)
        /// </summary>
        public string FromStyle { get; set; }

        /// <summary>
        /// 来源表字段
        /// </summary>
        public string FromField { get; set; }

        /// <summary>
        /// 目标表字段
        /// </summary>
        public string ToField { get; set; }
    }
}
