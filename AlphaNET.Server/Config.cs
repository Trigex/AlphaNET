using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AlphaNET.Server
{
    public class Tcp
    {
        public string ip { get; set; }
        public int port { get; set; }
    }

    public class Db
    {
        public string connectionString { get; set; }
    }

    public class Config
    {
        public Tcp tcp { get; set; }
        public Db db { get; set; }

        public static Config CreateConfig(string json)
        {
            return JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
