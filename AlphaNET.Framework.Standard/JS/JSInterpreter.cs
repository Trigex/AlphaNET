using AlphaNET.Framework.Standard.Client;
using AlphaNET.Framework.Standard.IO;
using AlphaNET.Framework.Standard.Proxies;
using Jint;
using Jint.Runtime.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Standard.JS
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

        /// <summary>
        /// Instantiate an <c>JSInterpreter</c>
        /// </summary>
        /// <param name="filesystemProxy"></param>
        /// <param name="console"></param>
        public JSInterpreter(Filesystem fs, Client.Console console)
        {
            _engine = new Engine();
            _console = console;
            _filesystemProxy = new FilesystemProxy(fs);

            // Compiler should use a seperate engine, as not to muck up this classes' engine's global scope
            _compiler = new TypescriptCompiler(new Engine());
            CompilerProxy = new TypescriptCompilerProxy(_compiler);
        }

        /// <summary>
        /// Initalizes the C# CLR JavaScript API
        /// </summary>
        public void InitAPI(string kernelScript)
        {
            // Terminal
            _engine.SetValue("Terminal", _console);
            // FilesystemObject abstract class
            _engine.SetValue("FilesystemObject", TypeReference.CreateTypeReference(_engine, typeof(FilesystemObject)));
            // File
            _engine.SetValue("FSFile", TypeReference.CreateTypeReference(_engine, typeof(File)));
            // Directory
            _engine.SetValue("FSDirectory", TypeReference.CreateTypeReference(_engine, typeof(Directory)));
            // StatusCodes
            _engine.SetValue("StatusCode", TypeReference.CreateTypeReference(_engine, typeof(StatusCode)));
            // FilesystemProxy
            _engine.SetValue("Filesystem", _filesystemProxy);
            // List type??
            _engine.SetValue("FSObjectList", TypeReference.CreateTypeReference(_engine, typeof(List<FilesystemObject>)));
            // This ;) Make a proxy later
            _engine.SetValue("JSInterpreter", new JSInterpreterProxy(this));
            // UTF8 encoding
            _engine.SetValue("UTF_8", Encoding.UTF8);
            // TypescriptCompiler
            _engine.SetValue("TypescriptCompiler", CompilerProxy);

            // Create global object, and add kernel functions to global scope
            _engine.Execute("var Global = {};").Execute(kernelScript);
        }

        /// <summary>
        /// Executes a given script on the internal <c>Jint</c> JavaScript engine
        /// </summary>
        /// <param name="script">String with (should be) valid JavaScript code</param>
        /// <param name="isTypescript">Is the script uncompiled TypeScript?</param>
        public void ExecuteScript(string script, bool isTypescript)
        {
            if (isTypescript)
                script = _compiler.Compile(script);

            try
            {
                _engine.Execute(script);
            } catch(Exception e)
            {
                _console.WriteLine(string.Format("There was a JSInterpreter error!: {0}", e.ToString()));
            }
        }
    }
}
