using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerDataMove
{
    public class KeyClass
    {
        /// <summary>
        /// 步骤
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// 原字段
        /// </summary>

        public string OldField { get; set; }

        /// <summary>
        /// 原字段值
        /// </summary>

        public object OldValue { get; set; }

        /// <summary>
        /// 新字段
        /// </summary>

        public string NewField { get; set; }

        /// <summary>
        /// 新字段值
        /// </summary>
        public object NewValue { get; set; }
    }
}
