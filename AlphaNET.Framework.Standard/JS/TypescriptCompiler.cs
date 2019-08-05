using AlphaNET.Framework.Standard.IO;
using Jint;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;

namespace AlphaNET.Framework.Standard.JS
{
    public class TypescriptCompiler
    {
        private Engine _engine;

        public TypescriptCompiler(Engine engine)
        {
            _engine = engine;

            // get bootstrap and services files
            try
            {
                var bootstrap = IOUtils.ReadManifestData<TypescriptCompiler>("TypescriptBootstrap.js");
                var services = IOUtils.ReadManifestData<TypescriptCompiler>("TypescriptServices.js");
                // execute them, thus adding them to the global scope
                Console.WriteLine("Installing compiler...");
                _engine.Execute(services);
                _engine.Execute(bootstrap);
            } catch(Exception e)
            {
                Console.WriteLine(String.Format("Error while initalizing compiler: {0}", e.ToString()));
            }
        }

        public string Compile(string script)
        {
            string outputSource = "";
            try
            {
                var transpiledSource = _engine.Invoke("tsTranspile", script).AsObject();
                outputSource = _engine.Invoke("getTranspileResultCode", transpiledSource).AsString();
            } catch (Exception e)
            {
                Console.WriteLine(String.Format("Error while compiling TypeScript: {0}", e.ToString()));
            }
            return outputSource;
        }

        public string Compile(string[] scripts)
        {
            string input = "";
            // Create one single string input. Order matters!
            foreach (string script in scripts)
            {
                input += script + "\n";
            }

            return Compile(input);
        }
    }
}
