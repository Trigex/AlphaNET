﻿using AlphaNET.Framework.IO;
using AlphaNET.Framework.JS;
using AlphaNET.Framework.Net;
using System;
using System.Text;

namespace AlphaNET.Framework.Client
{
    public class Computer
    {
        private Filesystem _filesystem;
        private JSInterpreter _interpreter;
        private Console _console;
        private TcpClient _TcpClient;
        private SocketManager _socketManager;

        public Computer(Filesystem filesystem, bool offlineMode, string ip = null, int port = 0)
        {
            if (filesystem == null) // If no filesystem was specified in cli args
            {
                _filesystem = BootstrapFilesystem();
            }
            else
            {
                _filesystem = filesystem;
            }

            _console = new Console();

            if (!offlineMode && ip != null && port != 0) // Not in offline mode
            {
                _TcpClient = new TcpClient(ip, port);

                _console.WriteLine("Connecting to server...");
                try
                {
                    _socketManager = new SocketManager(_TcpClient);
                    _TcpClient.AddSocketManager(_socketManager);
                    _TcpClient.Start();
                }
                catch (Exception e)
                {
                    _console.WriteLine("Error: " + e.Message + "\n\nUnable to establish connection with server; To retry a connection, issue \"server reconnect\" to the shell...");
                }
            }
            else
            {
                _console.WriteLine("NOTICE: In Offline Mode");
            }

            _interpreter = new JSInterpreter(_filesystem, _console, _socketManager);
            _console.WriteLine("Compiling kernel...");
            string system = _interpreter.CompilerProxy.CompileTypescript(new string[] { IOUtils.ReadManifestData<Computer>("system.ts") });
            // append third party libs
            system += "\n" + IOUtils.ReadManifestData<Computer>("minimist.js") + "\n" + IOUtils.ReadManifestData<Computer>("lodash.min.js");
            _interpreter.InitAPI(system);
            InstallOS(system);
        }

        public void Start()
        {
            _console.WriteLine("Running Init script...");
            var init = (File)_filesystem.GetFilesystemObjectByAbsolutePath("/bin/shell.js");
            _interpreter.ExecuteScript(Encoding.UTF8.GetString(init.Contents), false, null);
        }

        public Filesystem GetFilesystem()
        {
            return _filesystem;
        }

        /// <summary>
        /// Very temporary /dev/ method to build and install the OS every run
        /// </summary>
        private void InstallOS(string system)
        {
            _console.WriteLine("Compiling and installing OS files...");
            var initProgram = IOUtils.ReadManifestData<Computer>("init.ts");
            var shellProgram = IOUtils.ReadManifestData<Computer>("shell.ts");
            var lsProgram = IOUtils.ReadManifestData<Computer>("ls.ts");
            var catProgram = IOUtils.ReadManifestData<Computer>("cat.ts");
            var netProgram = IOUtils.ReadManifestData<Computer>("net.ts");

            var bin = (Directory)_filesystem.GetFilesystemObjectByAbsolutePath("/bin/");
            _filesystem.AddFilesystemObject(new File("system.js", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(system)), bin);
            _filesystem.AddFilesystemObject(new File("init.js", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(initProgram))), bin);
            _filesystem.AddFilesystemObject(new File("shell.js", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(shellProgram))), bin);
            _filesystem.AddFilesystemObject(new File("ls.js", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(lsProgram))), bin);
            _filesystem.AddFilesystemObject(new File("cat.js", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(catProgram))), bin);
            _filesystem.AddFilesystemObject(new File("net.js", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes(_interpreter.CompilerProxy.CompileTypescript(netProgram))), bin);

            BinaryManager.WriteBinaryToFile("debug.fs", BinaryManager.CreateBinaryFromFilesystem(_filesystem));
            BinaryManager.ReloadFilesystemFromBinary(_filesystem, System.IO.File.ReadAllBytes("debug.fs"));
        }

        private Filesystem BootstrapFilesystem()
        {
            System.Console.WriteLine("Bootstraping filesystem...");
            Filesystem fs = new Filesystem();
            var root = new Directory("root", IOUtils.GenerateID());
            root.Owner = root;
            var bin = new Directory("bin", IOUtils.GenerateID());
            var sub = new Directory("sub", IOUtils.GenerateID());
            var lib = new Directory("lib", IOUtils.GenerateID());
            var hello = new File("hello.txt", IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes("Hello, World!"));
            var src = new Directory("src", IOUtils.GenerateID());

            fs.AddFilesystemObject(root);
            fs.AddFilesystemObject(bin, root);
            fs.AddFilesystemObject(sub, bin);
            fs.AddFilesystemObject(hello, sub);
            fs.AddFilesystemObject(lib, root);
            fs.AddFilesystemObject(src, root);

            BinaryManager.WriteBinaryToFile("debug.fs", BinaryManager.CreateBinaryFromFilesystem(fs));
            return BinaryManager.CreateFilesystemFromBinary(BinaryManager.CreateBinaryFromFilesystem(fs));
        }
    }
}