using System;
using System.Text;
using System.Collections.Generic;
using NLog.Config;
using NLog.Mongo;

namespace Origine.Configuration.Options
{
    public class NLogNoSqlTargetOptions
    {
        public class Field
        {
            public string Name { get; set; }
            public string Layout { get; set; }
            public string BsonType { get; set; }
        }
        public class Rule
        {
            public string Logger { get; set; }
            public string MinLevel { get; set; }
            public string MaxLevel { get; set; }
        }

        public string Name { get; set; }
        public string CollectionName { get; set; }
        public List<Field> Fields { get; set; }
        public List<Rule> Rules { get; set; }
    }
}
