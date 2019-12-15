using System;
using System.Linq;

using MongoDB;
using MongoDB.Driver;

namespace Origine.Storage.Accessor
{
    /// <summary>
    /// MongoDb Query Provider
    /// </summary>
    public class MongoDbDataAccessor : IDataAccessor
    {
        readonly IMongoDatabase _mongoDatabase;

        public MongoDbDataAccessor(IMongoDatabase database)
        {
            _mongoDatabase = database;
        }

        public IQueryable<TState> GetItems<TState>(string name)
        {
            var collection = _mongoDatabase.GetCollection<TState>(name ?? typeof(TState).Name);

            return collection.AsQueryable();
        }
    }
}
