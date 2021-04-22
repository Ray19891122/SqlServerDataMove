using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CusControlLibrary1
{
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
