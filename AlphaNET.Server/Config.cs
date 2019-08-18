using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AlphaNET.Server
{
    public class Tcp
    {
        public string Ip { get; set; }
        public int Port { get; set; }
    }

    public class Db
    {
        public string ConnectionString { get; set; }
    }

    public class Config
    {
        public Tcp Tcp { get; set; }
        public Db Db { get; set; }

        public static Config CreateConfig(string json)
        {
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
