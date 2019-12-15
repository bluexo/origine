using System;
using System.Collections.Generic;
using System.Text;

namespace Origine.Models.AI
{
    /// <summary>
    /// 认知片段
    /// </summary>
    public class CognitionSegment
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 特征值
        /// </summary>
        public Dictionary<string, string> Features { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 大概时间
        /// </summary>
        public DateTime? ProbablyTime { get; set; }
    }
}
