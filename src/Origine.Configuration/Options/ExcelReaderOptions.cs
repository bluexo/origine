using System;
using System.Collections.Generic;
using System.Text;

namespace Origine
{
    public class ExcelReaderOptions
    {
        /// <summary>
        /// Excel 配置文件路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Excel 字段名所在行
        /// </summary>
        public int FieldNameRow { get; set; }
    }
}
