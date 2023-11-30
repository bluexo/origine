using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using Orleans.Providers.MongoDB.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Origine.Accessor
{
    [BsonIgnoreExtraElements]
    public class GrainData<TState>
    {
        [BsonId]
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("_doc")]
        public TState State { get; set; }
        [BsonElement("_etag")]
        public string Etag { get; set; }

        public string GrainKeyString => Id.ToGrainKeyString();
        public long GrainKeyLong => Id.ToGrainKeyLong();
        public Guid GrainKeyGuid => Id.ToGrainKeyGuid();
    }

    /// <summary>
    /// MongoDb Query Provider
    /// </summary>
    public class MongoDbDataAccessor : IDataAccessor
    {
        readonly IMongoDatabase _mongoDatabase;
        readonly Dictionary<string, object> cachedCollection = new Dictionary<string, object>();

        public MongoDbDataAccessor(IOptions<MongoDBOptions> options,
            IMongoClient client,
            ILogger<MongoDbDataAccessor> logger)
        {
            _mongoDatabase = client.GetDatabase(options.Value.DatabaseName);
        }

        public IQueryable<GrainData<TState>> GetQueryable<TState>(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = typeof(TState).Name;

            if (!cachedCollection.ContainsKey(name))
            {
                BsonClassMap.RegisterClassMap<TState>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    var ignoreMembers = typeof(TState).GetMembers().Where(m => m.GetCustomAttribute<QueryIgnore>() != null);
                    foreach (var member in ignoreMembers) cm.UnmapMember(member);
                });
                var collection = _mongoDatabase.GetCollection<GrainData<TState>>(name);
                cachedCollection.Add(name, collection);
            }
            var mongoCollection = (IMongoCollection<GrainData<TState>>)cachedCollection[name];

            if (mongoCollection == null)
                throw new NullReferenceException($"Cannot found table {name} from database!");

            return mongoCollection.AsQueryable();
        }
    }
}
