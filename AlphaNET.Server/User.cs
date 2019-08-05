using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Server
{
    public class User
    {
        [BsonId]
        public string ID { get; set; } // the unique ID should be a user's real IP address
        [BsonElement("virtualIPAddress")]
        public string VirtualIPAddress { get; set; }
    }
}
