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
    public class JSInterpreter
    {
        /// <summary>
        /// The internal <c>Jint</c> JavaScript engine, used for the actual JavaScript interpretation
        /// </summary>
        private Engine _engine;
        /// <summary>
        /// A proxy to a <c>Filesystem</c> instance, used for saftey within the C# CLR JavaScript API
        /// </summary>
        private FilesystemProxy _filesystemProxy;
        /// <summary>
        /// An instance of a <c>Console</c>
        /// </summary>
        private Client.Console _console;
        public TypescriptCompilerProxy CompilerProxy;
        private TypescriptCompiler _compiler;
        private SocketManager _socketManager;
        private UtilsProxy _utilsProxy;

        /// <summary>
        /// Instantiate an <c>JSInterpreter</c>
        /// </summary>
        /// <param name="filesystemProxy"></param>
        /// <param name="console"></param>
        public JSInterpreter(Filesystem fs, Client.Console console, SocketManager socketManager)
        {
            _engine = new Engine();
            _console = console;
            _filesystemProxy = new FilesystemProxy(fs);
            _socketManager = socketManager;


            // Compiler should use a seperate engine, as not to muck up this classes' engine's global scope
            _compiler = new TypescriptCompiler(new Engine());
            CompilerProxy = new TypescriptCompilerProxy(_compiler);
        }

        /// <summary>
        /// Initalizes the C# CLR JavaScript API
        /// </summary>
        public void InitAPI(string systemScript)
        {
            // Terminal
            _engine.SetValue("Terminal", _console);
            // FilesystemObject abstract class
            _engine.SetValue("FilesystemObject", TypeReference.CreateTypeReference(_engine, typeof(FilesystemObject)));
            // File
            _engine.SetValue("FILE", TypeReference.CreateTypeReference(_engine, typeof(File)));
            // Directory
            _engine.SetValue("DIRECTORY", TypeReference.CreateTypeReference(_engine, typeof(Directory)));
            // StatusCodes
            _engine.SetValue("StatusCode", TypeReference.CreateTypeReference(_engine, typeof(StatusCode)));
            // FilesystemProxy
            _engine.SetValue("Filesystem", _filesystemProxy);
            // List type??
            _engine.SetValue("List", TypeReference.CreateTypeReference(_engine, typeof(System.Collections.Generic.List<object>)));
            // This ;) Make a proxy later
            _engine.SetValue("JSInterpreter", new JSInterpreterProxy(this));
            // UTF8 encoding
            _engine.SetValue("UTF8", Encoding.UTF8);
            // TypescriptCompiler
            _engine.SetValue("TypescriptCompiler", CompilerProxy);
            // NET
            _engine.SetValue("SocketManager", _socketManager);
            _engine.SetValue("Socket", TypeReference.CreateTypeReference(_engine, typeof(Socket)));
            _engine.SetValue("Address", TypeReference.CreateTypeReference(_engine, typeof(Address)));
            // Create global object, and add system functions to global scope
            try
            {
                _engine.Execute("var Global = {};").Execute(systemScript);
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Executes a given script on the internal <c>Jint</c> JavaScript engine
        /// </summary>
        /// <param name="script">String with (should be) valid JavaScript code</param>
        /// <param name="isTypescript">Is the script uncompiled TypeScript?</param>
        public int ExecuteScript(string script, bool isTypescript, string[] args, bool async = true)
        {
            

            // If it's typescript, compile, then run
            if (isTypescript)
            {
                script = _compiler.Compile(script);
            }

            if (async)
            {
                var thread = new Thread(() => Execution(script, args));
                thread.Start();
            } else
            {
                Execution(script, args);
            }

            return 0;
        }

        private int Execution(string script, string[] args)
        {
            int returnValue = -1;

            try
            {
                // Get main function
                var main = _engine.Execute(script).GetValue("Main");
                returnValue = (int)main.Invoke(JsValue.FromObject(_engine, args)).AsNumber();
            }
            catch (Exception e)
            {
                _console.WriteLine(string.Format("There was a JSInterpreter error!: {0}", e.ToString()));
            }

            return returnValue;
        }
    }
}
