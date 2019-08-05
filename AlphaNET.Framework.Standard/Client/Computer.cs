﻿using AlphaNET.Framework.Standard.IO;
using AlphaNET.Framework.Standard.JS;
using AlphaNET.Framework.Standard.Proxies;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Standard.Client
{
    public class Computer
    {
        private Filesystem _filesystem;
        private JSInterpreter _interpreter;
        private Console _console;

        public Computer(Filesystem filesystem)
        {
            _filesystem = filesystem;
            _console = new Console();
            _interpreter = new JSInterpreter(_filesystem, _console);
            _console.WriteLine("Compiling kernel...");
            _interpreter.InitAPI(_interpreter.CompilerProxy.CompileTypescript(new string[] { IOUtils.ReadManifestData<Computer>("kernel.ts"), IOUtils.ReadManifestData<Computer>("minimist.js") }));
            InstallOS();
        }

        public void Start()
        {
            _console.WriteLine("Running Init script...");
            var init = (File)_filesystem.GetFilesystemObjectsByTitle("init.js")[0];
            _interpreter.ExecuteScript(Encoding.UTF8.GetString(init.Contents), false);
        }

        /// <summary>
        /// Very temporary /dev/ method to build and install the OS every run
        /// </summary>
        private void InstallOS()
        {
            _console.WriteLine("Compiling and installing OS files...");
            var initProgram = IOUtils.ReadManifestData<Computer>("init.ts");
            var shellProgram = IOUtils.ReadManifestData<Computer>("shell.ts");
            var lsProgram = IOUtils.ReadManifestData<Computer>("ls.ts");
            var catProgram = IOUtils.ReadManifestData<Computer>("cat.ts");
            var cdProgram = IOUtils.ReadManifestData<Computer>("cd.ts");

            var bin = (Directory)_filesystem.GetFilesystemObjectsByTitle("bin")[0];
            _filesystem.AddFilesystemObject(new File("kernel.js", bin, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(IOUtils.ReadManifestData<Computer>("kernel.ts")))), bin);
            _filesystem.AddFilesystemObject(new File("init.js", bin, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(initProgram))), bin);
            _filesystem.AddFilesystemObject(new File("shell.js", bin, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(shellProgram))), bin);
            _filesystem.AddFilesystemObject(new File("ls.js", bin, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(lsProgram))), bin);
            _filesystem.AddFilesystemObject(new File("cat.js", bin, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(catProgram))), bin);
            _filesystem.AddFilesystemObject(new File("cd.js", bin, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(cdProgram))), bin);

            BinaryManager.WriteBinaryToFile("debug.fs", BinaryManager.CreateBinaryFromFilesystem(_filesystem));
            BinaryManager.ReloadFilesystemFromBinary(_filesystem, System.IO.File.ReadAllBytes("debug.fs"));
        }
    }
}
