using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Origine.Models.AI;

namespace Origine.Interfaces.AI
{
    public interface IAgent : IGrainWithGuidKey
    {
        /// <summary>
        /// 学习 
        /// </summary>
        /// <returns></returns>
        Task Learn(IList<CognitionSegment> cognitionSegment);

        /// <summary>
        /// 分析一个目标, 并记录特征
        /// </summary>
        /// <returns></returns>
        Task Analyze<T>(T target);

        /// <summary>
        /// 辨别一个目标, 并给出结果
        /// </summary>
        /// <returns></returns>
        Task<bool> Distinguish<T>(T agent);

        /// <summary>
        /// 被询问,可以根据双方关系以及自己的状态决定是否给出答案
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<IList<CognitionSegment>> InquireFrom(IAgent agent);
    }
}
