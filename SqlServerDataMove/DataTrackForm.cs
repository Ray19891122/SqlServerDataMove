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
using IntegrateHelp;
using CusControlLibrary1;
using System.IO;

namespace SqlServerDataMove
{
    public partial class DataTrackForm : Form
    {
        /// <summary>
        /// 跟踪开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 插入记录表
        /// </summary>
        public DataTable dt_table { get; set; }

        /// <summary>
        /// 数据库操作对象
        /// </summary>
        public DbHelper DB { get; set; }

        /// <summary>
        /// 来源表数据库
        /// </summary>
        public string FromDataBaseName { get; private set; }

        /// <summary>
        /// 来源表数据表
        /// </summary>
        public string FromDataTableName { get; private set; }

        /// <summary>
        /// 来源表数据行
        /// </summary>
        public DataRow FromRow { get; private set; }

        /// <summary>
        /// 目标表数据库
        /// </summary>
        public string ToDataBaseName { get; private set; }

        /// <summary>
        /// 目标表数据表
        /// </summary>
        public string ToDataTableName { get; private set; }

        /// <summary>
        /// 目标表数据行
        /// </summary>
        public DataRow ToRow { get; private set; }


        private string sstyple = string.Empty;

        public string SConntionString { get; set; }

        public string IP { get; set; }

        public string User { get; set; }

        public string PassWord { get; set; }

        public DataTrackForm()
        {
            InitializeComponent();
            radioButton1.Checked = true;
        }

        private struct dataInfo
        {
            public string Name;
            public string Score;
        }

        private void DataTrackForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据库插入记录追踪
        /// </summary>
        /// <param name="db"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private DataTable GetTrackInfo(DbHelper db, string startTime, string endTime)
        {
            string sqlString = @"CREATE TABLE #TEMP
        (
          ID INT IDENTITY(1, 1) ,
          [数据库名称] VARCHAR(50) ,
          [表名] VARCHAR(200) ,
          [语句内容] VARCHAR(MAX) ,
          [执行计数] INT ,
          [上次执行时间] DATETIME
        );
    INSERT  INTO #TEMP
            ( 数据库名称 ,
              表名 ,
              语句内容 ,
              执行计数 ,
              上次执行时间
            )
            SELECT  A.数据库名称 ,
                    RTRIM(LTRIM(REPLACE(REPLACE(CASE WHEN CHARINDEX('.', A.表名) > 0
                                                     THEN REVERSE(SUBSTRING(REVERSE(A.表名),
                                                              1,
                                                              CHARINDEX('.',
                                                              REVERSE(A.表名))
                                                              - 1))
                                                     ELSE A.表名
                                                END, '[', ''), ']', ''))) AS 表名 ,
                    A.语句内容 ,
                    A.执行计数 ,
                    A.上次执行时间
            FROM    ( SELECT    A.数据库名称 ,
                                SUBSTRING(REPLACE(REPLACE(REPLACE(LTRIM(REPLACE(A.语句内容,
                                                              'INSERT INTO',
                                                              '')), '@', '  '),
                                                          '(', ' '), '#', ' '),
                                          0,
                                          CHARINDEX(' ',
                                                    REPLACE(REPLACE(REPLACE(LTRIM(REPLACE(A.语句内容,
                                                              'INSERT INTO',
                                                              '')), '@', '  '),
                                                              '(', ' '), '#',
                                                            ' '))) AS 表名 ,
                                A.语句内容 ,
                                A.执行计数 ,
                                A.上次执行时间
                      FROM      ( SELECT    DB_NAME(qp.dbid) AS [数据库名称] ,
                                            SUBSTRING(qt.text,
                                                      ( qs.statement_start_offset
                                                        / 2 ) + 1,
                                                      ( ( CASE qs.statement_end_offset
                                                            WHEN -1
                                                            THEN DATALENGTH(qt.text)
                                                            ELSE qs.statement_end_offset
                                                          END
                                                          - qs.statement_start_offset )
                                                        / 2 ) + 1) AS [语句内容] ,
                                            qs.execution_count AS [执行计数] ,
                                            qs.last_execution_time [上次执行时间]
                                  FROM      sys.dm_exec_query_stats qs
                                            CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
                                            CROSS APPLY sys.dm_exec_query_plan(qs.plan_handle) qp
                                  WHERE     qt.encrypted = 0
                                            AND last_execution_time >= '" + startTime + @"'
                                            AND last_execution_time <= '" + endTime + @"'
                                ) A
                      WHERE     A.语句内容 LIKE 'INSERT INTO%'
                    ) A
            WHERE   A.表名 IS NOT NULL
                    AND A.表名 != ''
            ORDER BY A.上次执行时间 DESC;
    CREATE TABLE #TEMP2
        (
          ID INT IDENTITY(1, 1) ,
          BaseName VARCHAR(50) ,
          TableName VARCHAR(200)
        );
    CREATE TABLE #TEMP3
        (
          ID INT IDENTITY(1, 1) ,
          Context VARCHAR(2000)
        );
    INSERT  INTO #TEMP3
            ( Context
            )
            SELECT  'SELECT ''' + name + ''',Name FROM ' + name
                    + '.. SysObjects Where XType=''U''' AS Context
            FROM    master..sysdatabases
            WHERE   name NOT IN ( 'master', 'model', 'msdb', 'tempdb',
                                  'northwind', 'pubs', 'ReportServer',
                                  'ReportServerTempDB' );
    --SELECT  *
    --FROM    #TEMP3;
    DECLARE @index INT = 1 ,
        @count INT = ( SELECT   COUNT(*)
                       FROM     #TEMP3
                     ) ,
        @Text VARCHAR(2000);
    WHILE @index < @count + 1
        BEGIN
            SET @Text = ( SELECT    Context
                          FROM      #TEMP3
                          WHERE     ID = @index
                        );
            INSERT  INTO #TEMP2
                    EXEC ( @Text
                        );
            SET @index = @index + 1;
        END;
    SELECT  CASE WHEN A.数据库名称 IS NULL THEN B.BaseName
                 ELSE A.数据库名称
            END AS 数据库名称 ,
            A.表名 ,
            A.语句内容 ,
            A.执行计数 ,
            A.上次执行时间
    FROM    #TEMP A
            LEFT JOIN #TEMP2 B ON A.表名 = B.TableName;";
            return db.Fill(sqlString);
        }

        private void On_SelectRow(object sender, DataRow row)
        {
            DataTrack con = sender as DataTrack;
            if (this.sstyple == "选择来源")
            {
                this.FromDataBaseName = con.DataBaseName;
                this.FromDataTableName = con.DataTableName;
                this.FromRow = row;
                textBox1.Text = "来源库:" + FromDataBaseName + " 来源表:" + FromDataTableName;
            }
            else if (this.sstyple == "选择目标")
            {
                this.ToDataBaseName = con.DataBaseName;
                this.ToDataTableName = con.DataTableName;
                this.ToRow = row;
                textBox2.Text = "来源库:" + ToDataBaseName + " 来源表:" + ToDataTableName;
            }
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control con in this.flowLayoutPanel1.Controls)
            {
                if (con is DataTrack)
                {
                    con.Width = flowLayoutPanel1.Width;
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton2.Checked = false;
                sstyple = radioButton1.Text;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                radioButton1.Checked = false;
                sstyple = radioButton2.Text;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.FromDataBaseName))
            {
                MessageBox.Show("请选择来源");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.FromDataTableName))
            {
                MessageBox.Show("请选择来源");
                return;
            }
            if (FromRow == null)
            {
                MessageBox.Show("请选择来源");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.ToDataBaseName))
            {
                MessageBox.Show("请选择目标");
                return;
            }
            if (string.IsNullOrWhiteSpace(this.ToDataTableName))
            {
                MessageBox.Show("请选择目标");
                return;
            }
            if (ToRow == null)
            {
                MessageBox.Show("请选择目标");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = GetTrackInfo(this.DB, this.StartTime.ToString(), DateTime.Now.AddDays(1).ToString());
            if (dt.Rows.Count <= 0)
                return;
            this.dt_table = dt;
            var query = (from p in dt_table.AsEnumerable()
                         group p by new { name = p.Field<string>("数据库名称"), score = p.Field<string>("表名") } into m
                         select new dataInfo { Name = m.Key.name, Score = m.Key.score }).ToList();
            if (!string.IsNullOrWhiteSpace(this.FromDataBaseName) && !string.IsNullOrWhiteSpace(this.FromDataTableName) && !string.IsNullOrWhiteSpace(this.ToDataBaseName) && !string.IsNullOrWhiteSpace(this.ToDataTableName))
            {
                query = new List<dataInfo>
                {
                    new dataInfo {Name = this.FromDataBaseName,Score = this.FromDataTableName },
                    new dataInfo { Name = this.ToDataBaseName,Score = this.ToDataTableName}

                };
            }
            if (File.Exists("D:\\123.txt"))
            {
                File.Delete("D:\\123.txt");
            }
            foreach (var item in query)
            {
                List<DataRow> lst_rows = dt_table.Rows.Cast<DataRow>()
                    .Where(x => x["数据库名称"].ToString() == item.Name && x["表名"].ToString() == item.Score)
                    .Select(x => x)
                    .ToList();

                //DataTable dt = DB.Fill(string.Format(" SELECT  name  FROM sys.columns where OBJECT_NAME(OBJECT_ID)={0}.dbo.{1}", item.Name, item.Score));
                DataTable dt2 = GetTableInfo(item.Name, item.Score);
                List<string> lst_timeFields = new List<string>();
                foreach (DataRow row in dt2.Rows)
                {
                    string type = row["type"].ToString();
                    if (type != "datetime")
                        continue;
                    lst_timeFields.Add("(" + row["ColumnsName"].ToString() + " >= '" + this.StartTime + "' and " + row["ColumnsName"].ToString() + " is not null ) ");
                }
                string sqlWhere = string.Join(" or ", lst_timeFields);
                string sqlString = string.Format(" select TOP 5 * from {0}.dbo.{1} where 1 = 1 and " + sqlWhere, item.Name, item.Score);
                File.AppendAllText("D:\\123.txt", "\r\n" + sqlString);
                DataTable dt1 = DB.Fill(sqlString);
                //DataTable dt1 = DB.Fill(string.Format(" select * from {0}.dbo.{1} where 1=1 " ,item.Name, item.Score));
                DataTrack dataTrack = new DataTrack();
                dataTrack.DT_OperationRecord = lst_rows.CopyToDataTable();
                dataTrack.DT_Data = dt1;
                dataTrack.Width = flowLayoutPanel1.Width;
                dataTrack.DataBaseName = item.Name;
                dataTrack.DataTableName = item.Score;
                dataTrack.SelectRowEvent -= new DataTrack.SelectRowHander(On_SelectRow);
                dataTrack.SelectRowEvent += new DataTrack.SelectRowHander(On_SelectRow);
                flowLayoutPanel1.Controls.Add(dataTrack);
            }
        }

        /// <summary>
        /// 通过连接语句与表名获取表结构信息
        /// </summary>
        /// <param name="conntionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private DataTable GetTableInfo(string dataBase, string tableName)
        {
            DbHelper db = new SqlClientHelper(string.Format(this.SConntionString, this.IP, dataBase, this.User, this.PassWord));
            string sqlString = @" SELECT  ColumnsName = a.name ,
        type = b.name
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
    }
}
