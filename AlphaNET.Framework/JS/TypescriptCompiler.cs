using AlphaNET.Framework.IO;
using Jint;
using System;
using System.Linq;

namespace AlphaNET.Framework.JS
{
    public class TypescriptCompiler
    {
        private Engine _engine;

        public TypescriptCompiler()
        {
            _engine = new Engine();

            // get bootstrap and services files
            try
            {
                var bootstrap = IOUtils.ReadManifestData<TypescriptCompiler>("TypescriptBootstrap.js");
                var services = IOUtils.ReadManifestData<TypescriptCompiler>("TypescriptServices.js");
                // execute them, thus adding them to the global scope
                Console.WriteLine("Installing compiler...");
                _engine.Execute(services);
                _engine.Execute(bootstrap);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while initializing compiler: {e.ToString()}");
            }
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
                Console.WriteLine($"Error while compiling TypeScript: {e.ToString()}");
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
