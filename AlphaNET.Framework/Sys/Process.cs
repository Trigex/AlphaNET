using AlphaNET.Framework.JS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AlphaNET.Framework.Sys
{
    public class Process
    {
        public int Pid { get; set; }
        private Thread thread;
        private JSInterpreter interpreter;

        public Process(int pid, string script, JSInterpreter interpreter)
        {
            Pid = pid;
        }
    }
}
