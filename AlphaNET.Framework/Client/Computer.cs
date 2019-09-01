﻿using AlphaNET.Framework.IO;
using AlphaNET.Framework.JS;
using AlphaNET.Framework.Net;
using System;
using AlphaNET.Framework.Client.Visual;

namespace AlphaNET.Framework.Client
{
    /// <summary>
    /// The Computer class is the main controlling entity of an AlphaNET Client. It holds all components that the client uses throughout it's run,
    /// such as the Filesystem, the JsInterpreter, Console implementation, TcpClient, etc. It also is what gets the AlphaOS init script up
    /// and running.
    /// </summary>
    public class Computer
    {
        /// <summary>
        /// The Filesystem instance
        /// </summary>
        private Filesystem _fs;
        /// <summary>
        /// The JsInterpreter instance, used to run all the scripts this Computer may want to execute
        /// </summary>
        private readonly JsInterpreter _interpreter;
        /// <summary>
        /// The Console implementation this Computer reads and writes stdout text with
        /// </summary>
        private readonly IConsole _console;
        /// <summary>
        /// The TcpClient instance this Computer uses to communicate with the AlphaNET server
        /// </summary>
        private TcpClient _tcpClient;
        /// <summary>
        /// The SocketManager this Computer uses to manage Virtual Sockets
        /// </summary>
        private SocketManager _socketManager;
        /// <summary>
        /// Is the computer currently in offline mode?
        /// </summary>
        private bool _offlineMode;
        /// <summary>
        /// The TypescriptCompiler this Computer uses to dynamically compile scripts
        /// </summary>
        private TypescriptCompiler _compiler;
        
        /// <summary>
        /// Instantiates a new Computer, with basic initialization of all components
        /// </summary>
        /// <param name="fsFilePath">The path to the target Filesystem File</param>
        /// <param name="console">Console implementation the Computer should use</param>
        /// <param name="offlineMode">Is the Computer in Offline Mode?</param>
        /// <param name="serverIp">If not in Offline Mode, the target AlphaNET Server Ip</param>
        /// <param name="serverPort">If not in Offline Mode, the target AlphaNET Server port</param>
        /// <exception cref="Exception">Invalid server address arguments!</exception>
        public Computer(string fsFilePath, IConsole console, bool offlineMode, string serverIp = null, int serverPort = 0)
        {
            _console = console;
            _offlineMode = offlineMode;
            
            // Setup network stuff
            // online mode
            if (!offlineMode)
            {
                if (serverIp == null || serverPort == 0 || serverPort > 65535) // we're set to online mode, but the server parameters were invalid!
                {
                    // TODO: Make custom Exceptions to handle these bits
                    throw new Exception("Online Mode was enabled, but invalid server address parameters were provided!");
                }
                else // everything is cool
                {
                    // instantiate tcp client
                    _tcpClient = new TcpClient(serverIp, serverPort);
                    // instantiate socket manager
                    _socketManager = new SocketManager(_tcpClient);
                    _tcpClient.AddSocketManager(_socketManager);
                }
            }
            // offline mode
            else
            {
                _tcpClient = null;
                _socketManager = null;
            }
            
            // Setup filesystem stuff
            // fs file is initialized
            //if (FilesystemUtils.IsFsFileInitialized(fsFilePath))
            //{
            //    // TODO: Load Filesystem from here
            //}
            //else // fs file is not initialized!
            //{
                // TODO: Provide interface to create a Filesystem
            //}
            
            // TODO: Initialize JsInterpreter
        }

        public void Start()
        {
            /*_console.WriteLine("Running Init script...");
            var init = (File)_fs.GetObjectByAbsolutePath("/bin/init.js");
            _interpreter.Execute(new Process(Encoding.UTF8.GetString(init.Contents), new string[1]), true);*/
        }

        /// <summary>
        /// Very temporary /dev/ method to build and install the OS every run
        /// </summary>
        private void InstallOs(string system)
        {
            /*
            _console.WriteLine("Compiling and installing OS files...");
            var initProgram = IOUtils.ReadManifestData<Computer>("init.ts");
            var shellProgram = IOUtils.ReadManifestData<Computer>("shell.ts");
            var lsProgram = IOUtils.ReadManifestData<Computer>("ls.ts");
            var catProgram = IOUtils.ReadManifestData<Computer>("cat.ts");
            var netProgram = IOUtils.ReadManifestData<Computer>("net.ts");
            var cdProgram = IOUtils.ReadManifestData<Computer>("cd.ts");

            var bin = (Directory)_fs.GetObjectByAbsolutePath("/bin/");
            _fs.AddObject(new File("system.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(system)), bin);
            _fs.AddObject(new File("init.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(_compiler.Compile(initProgram))), bin);
            _fs.AddObject(new File("shell.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(_compiler.Compile(shellProgram))), bin);
            _fs.AddObject(new File("ls.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(_compiler.Compile(lsProgram))), bin);
            _fs.AddObject(new File("cat.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(_compiler.Compile(catProgram))), bin);
            _fs.AddObject(new File("net.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(_compiler.Compile(netProgram))), bin);
            _fs.AddObject(new File("cd.js", _fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes(_compiler.Compile(cdProgram))), bin);*/
        }

        /// <summary>
        /// Generates a new <c>Filesystem</c>, with base directories, and writes it to a Filesystem binary
        /// </summary>
        /// <returns>The generated <c>Filesystem</c></returns>
        private static Filesystem BootstrapFilesystem()
        {
            /*
            Console.WriteLine("Bootstrapping filesystem...");
            var fs = new Filesystem(FsPath);
            // run create binary to get basic layout setup
            BinaryManager.WriteBinaryToFile(FsPath, BinaryManager.CreateBinaryFromFilesystem(fs));
            var root = new Directory("root", fs.GenerateFilesystemObjectId());
            root.Owner = root;
            var bin = new Directory("bin", fs.GenerateFilesystemObjectId());
            var lib = new Directory("lib", fs.GenerateFilesystemObjectId());
            var hello = new File("hello.txt", fs.GenerateFilesystemObjectId(), true, Encoding.UTF8.GetBytes("Hello, World!"));
            var src = new Directory("src", fs.GenerateFilesystemObjectId());

            // Adding fs objects will automatically append to the fs binary
            fs.AddObject(root);
            fs.AddObject(bin, root);
            fs.AddObject(hello, root);
            fs.AddObject(lib, root);
            fs.AddObject(src, root);

            //BinaryManager.WriteBinaryToFile("debug.fs", BinaryManager.CreateBinaryFromFilesystem(fs));
            return fs;*/

            return null;
        }
    }
}
