using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Origine.Accessor
{
    /// <summary>
    /// 数据查询接口
    /// </summary>
    public interface IDataAccessor
    {
        /// <summary>
        /// 获取查询接口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IQueryable<GrainData<TState>> GetQueryable<TState>(string name = null);

    }
}
