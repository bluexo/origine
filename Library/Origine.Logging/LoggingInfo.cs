using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NLog.Extensions.Logging;

namespace Origine
{
    public struct LoggingInfo
    {
        public string Id { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public LogLevel Level { get; set; }
    }
}
