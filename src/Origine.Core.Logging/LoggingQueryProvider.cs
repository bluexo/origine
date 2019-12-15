using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Origine
{
    public interface LoggingQueryProvider
    {
        Task<List<LoggingInfo>> GetLogging();
        Task RemoveAsync(string id);
        Task ClearAsync(DateTime endTime);
    }

    public class DefaultLoggingQueryProvider : LoggingQueryProvider
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;

        public DefaultLoggingQueryProvider(string connectionString,
            string databaseName,
            string collectionName)
        {
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase(databaseName);
            _collectionName = collectionName;
        }

        public async Task ClearAsync(DateTime endTime)
        {
            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            var builder = new FilterDefinitionBuilder<BsonDocument>();
            var filter = builder.Lt(b => b[nameof(LoggingInfo.Date)], BsonValue.Create(endTime));
            var result = await collection.DeleteManyAsync(filter);
        }

        public async Task<List<LoggingInfo>> GetLogging()
        {
            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            var list = await collection
                .Find(new BsonDocument())
                .SortByDescending(b => b[nameof(LoggingInfo.Date)])
                .Limit(sbyte.MaxValue)
                .ToListAsync();

            return list.ConvertAll(Convert);

            static LoggingInfo Convert(BsonDocument document)
            {
                var info = new LoggingInfo
                {
                    Id = document.GetValue("_id").AsObjectId.ToString(),
                    Date = document.GetValue(nameof(LoggingInfo.Date), DateTime.MinValue).ToUniversalTime(),
                    Level = (LogLevel)document.GetValue(nameof(LoggingInfo.Level)).AsInt32,
                    Logger = document.GetValue(nameof(LoggingInfo.Logger), string.Empty).AsString,
                };
                var message = document.GetValue(nameof(LoggingInfo.Message)).AsString;
                info.Message = document.TryGetValue(nameof(Exception), out BsonValue exception)
                    ? $"{message}\n Exception:[{exception}]"
                    : message;
                return info;
            }
        }

        public Task RemoveAsync(string id)
        {
            var collection = _database.GetCollection<BsonDocument>(_collectionName);
            var doc = new BsonDocument { { "_id", new ObjectId(id) } };
            return collection.DeleteOneAsync(doc);
        }
    }
}
