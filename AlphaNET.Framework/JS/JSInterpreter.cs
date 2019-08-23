using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
using AlphaNET.Framework.Net;
using AlphaNET.Framework.Proxies;
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AlphaNET.Framework.JS
{
    /// <summary>
    /// <c>JSInterpreter</c> is a class used for the execution of JavaScript scripts
    /// </summary>
    public class JsInterpreter
    {
        private static string _systemScript;
        private readonly JsValue _global;
        private readonly FilesystemProxy _filesystemProxy;
        private readonly IConsole _console;
        private readonly List<Thread> _threadList;
        private readonly SocketManagerProxy _socketManagerProxy;
        private readonly ManualResetEvent _mre;

        public Stack<Process> ProcessStack;

        public JsInterpreter(Filesystem fs, IConsole console, SocketManager socketManager, string systemScript)
        {
            _systemScript = systemScript;
            _console = console;
            _filesystemProxy = new FilesystemProxy(fs);
            _socketManagerProxy = new SocketManagerProxy(socketManager);
            ProcessStack = new Stack<Process>();
            _threadList = new List<Thread>();
            _global = new Engine().Execute("var Global = {}").GetValue("Global");
            _mre = new ManualResetEvent(false);
        }

        public void Execute(Process process, bool blocking)
        {
            process.Thread = new Thread(() => RunThread(process, blocking));
            _threadList.Add(process.Thread);
            process.Thread.Name = "Execute" + _threadList.Count;
            process.Thread.Start();
            if(blocking)
            {
                _mre.WaitOne();
            }
        }

        private void RunThread(Process process, bool blocking)
        {
            var engine = CreateEngine();

            engine.SetValue("args", process.Args);
            engine.Execute(process.Script);
            if (!blocking) return;
            _mre.Set();
            _mre.Reset();
        }

        private Engine CreateEngine()
        {
            var engine = new Engine();
            InitApi(engine);
            return engine;
        }

        private void InitApi(Engine engine)
        {
            // Terminal
            engine.SetValue("Terminal", _console);
            // FilesystemObject abstract class
            engine.SetValue("FilesystemObject", TypeReference.CreateTypeReference(engine, typeof(FilesystemObject)));
            // File (Uppercase to avoid conflicts with Typescript, should be changed later when I write a VS Code extension to disable certain linting!)
            engine.SetValue("FILE", TypeReference.CreateTypeReference(engine, typeof(File)));
            // Directory
            engine.SetValue("DIRECTORY", TypeReference.CreateTypeReference(engine, typeof(Directory)));
            // StatusCodes
            engine.SetValue("IOStatusCode", TypeReference.CreateTypeReference(engine, typeof(IoStatusCode)));
            engine.SetValue("NetStatusCode", TypeReference.CreateTypeReference(engine, typeof(NetStatusCode)));
            // FilesystemProxy
            engine.SetValue("Filesystem", _filesystemProxy);
            // List, pretty ghetto way to offer "generic" lists to JavaScript, since it'll just accept anything, but oh well!
            engine.SetValue("List", TypeReference.CreateTypeReference(engine, typeof(System.Collections.Generic.List<object>)));
            // This ;) Make a proxy later
            engine.SetValue("JSInterpreter", new JsInterpreterProxy(this));
            // UTF8 encoding
            engine.SetValue("UTF8", Encoding.UTF8);
            // NET
            engine.SetValue("SocketManager", _socketManagerProxy);
            engine.SetValue("Socket", TypeReference.CreateTypeReference(engine, typeof(Socket)));
            engine.SetValue("Address", TypeReference.CreateTypeReference(engine, typeof(Address)));
            // Shared global object
            engine.SetValue("Global", _global);
            try
            {
                engine.Execute("var args = [];").Execute(_systemScript);
            }
            catch (Exception e)
            {
                _console.WriteLine(e.ToString());
            }
        }
    }
}
