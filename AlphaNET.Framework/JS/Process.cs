using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AlphaNET.Framework.JS
{
    public class Process
    {
        public Thread _thread { get; set; }
        public string Script { get; set; }
        public string[] Args { get; set; }

        public Process(string script, string[] args)
        {
            Script = script;
            Args = args;
        }
    }
}
