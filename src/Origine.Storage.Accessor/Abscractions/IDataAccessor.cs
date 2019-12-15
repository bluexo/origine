using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB;
using MongoDB.Driver;

namespace Origine.Storage.Accessor
{
    public interface IDataAccessor
    {
        IQueryable<T> GetItems<T>(string collectionName);
    }
}
