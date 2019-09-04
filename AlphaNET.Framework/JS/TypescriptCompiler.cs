using AlphaNET.Framework.IO;
using Jint;
using System;
using System.Diagnostics;
using System.Linq;

namespace AlphaNET.Framework.JS
{
    public class TypescriptCompiler
    {
        private readonly Engine _engine;

        public TypescriptCompiler()
        {
            _engine = new Engine();

            // get bootstrap and services files
            var bootstrap = IoUtils.ReadManifestData<TypescriptCompiler>("TypescriptBootstrap.js");
            var services = IoUtils.ReadManifestData<TypescriptCompiler>("TypescriptServices.js");
            // execute them, thus adding them to the global scope
            _engine.Execute(services);
            _engine.Execute(bootstrap);
        }

        public string Compile(string script)
        {
            string outputSource = "";
            try
            {
                var transpiledSource = _engine.Invoke("tsTranspile", script).AsObject();
                outputSource = _engine.Invoke("getTranspileResultCode", transpiledSource).AsString();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error while compiling TypeScript: {e}");
            }
            return outputSource;
        }

        public string Compile(string[] scripts)
        {
            var input = scripts.Aggregate("", (current, script) => current + (script + "\n"));

            return Compile(input);
        }
    }
}
