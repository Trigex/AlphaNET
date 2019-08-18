using AlphaNET.Framework.IO;
using AlphaNET.Framework.JS;
using AlphaNET.Framework.Net;
using System;
using System.Text;

namespace AlphaNET.Framework.Client
{
    public class Computer
    {
        private Filesystem _fs;
        private JSInterpreter _interpreter;
        private IConsole _console;
        private TcpClient _TcpClient;
        private SocketManager _socketManager;
        private TypescriptCompiler _compiler;

        public Computer()
        {   
        }

        public void Init(Filesystem filesystem, bool offlineMode, string ip = null, int port = 0, IConsole console = null)
        {
            if (console != null)
                _console = console;
            else
                _console = new Terminal(); // By default, use a native terminal console implementation

            if (filesystem != null) // If no filesystem was specified in cli args
            {
                _fs = filesystem;
            }
            else
            {
                _fs = BootstrapFilesystem();
            }

            if (!offlineMode && ip != null && port != 0) // Not in offline mode
            {
                _TcpClient = new TcpClient(ip, port);
                _socketManager = new SocketManager(_TcpClient);
                _TcpClient.AddSocketManager(_socketManager);

                _console.WriteLine("Connecting to server...");
                try
                {
                    _TcpClient.Start();
                }
                catch (Exception e)
                {
                    _console.WriteLine("Error: " + e.Message + "\n\nUnable to establish connection with server; To retry a connection, issue \"net server connect\" to the shell...");
                }
            }
            _compiler = new TypescriptCompiler();
            string system = _compiler.Compile(new string[] { IOUtils.ReadManifestData<Computer>("system.ts") });
            // append third party libs
            system += "\n" + IOUtils.ReadManifestData<Computer>("minimist.js") + "\n" + IOUtils.ReadManifestData<Computer>("lodash.min.js");

            _interpreter = new JSInterpreter(_fs, _console, _socketManager, system);
            _console.WriteLine("Compiling kernel...");
            InstallOS(system);
        }

        public void Start()
        {
            _console.WriteLine("Running Init script...");
            var init = (File)_fs.GetObjectByAbsolutePath("/bin/init.js");
            _interpreter.Execute(new Process(Encoding.UTF8.GetString(init.Contents), new string[1]), true);
        }

        public Filesystem GetFilesystem()
        {
            return _fs;
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
            var cdProgram = IOUtils.ReadManifestData<Computer>("cd.ts");

            var bin = (Directory)_fs.GetObjectByAbsolutePath("/bin/");
            _fs.AddObject(new File("system.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(system)), bin);
            _fs.AddObject(new File("init.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(_compiler.Compile(initProgram))), bin);
            _fs.AddObject(new File("shell.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(_compiler.Compile(shellProgram))), bin);
            _fs.AddObject(new File("ls.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(_compiler.Compile(lsProgram))), bin);
            _fs.AddObject(new File("cat.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(_compiler.Compile(catProgram))), bin);
            _fs.AddObject(new File("net.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(_compiler.Compile(netProgram))), bin);
            _fs.AddObject(new File("cd.js", _fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes(_compiler.Compile(cdProgram))), bin);

            BinaryManager.WriteBinaryToFile("debug.fs", BinaryManager.CreateBinaryFromFilesystem(_fs));
            BinaryManager.ReloadFilesystemFromBinary(_fs, System.IO.File.ReadAllBytes("debug.fs"));
        }

        /// <summary>
        /// Generates a new <c>Filesystem</c>, with base directories, and writes it to a Filesystem binary
        /// </summary>
        /// <returns>The generated <c>Filesystem</c></returns>
        private Filesystem BootstrapFilesystem()
        {
            System.Console.WriteLine("Bootstraping filesystem...");
            Filesystem fs = new Filesystem();
            var root = new Directory("root", fs.GenerateFilesystemObjectID());
            root.Owner = root;
            var bin = new Directory("bin", fs.GenerateFilesystemObjectID());
            var lib = new Directory("lib", fs.GenerateFilesystemObjectID());
            var hello = new File("hello.txt", fs.GenerateFilesystemObjectID(), true, Encoding.UTF8.GetBytes("Hello, World!"));
            var src = new Directory("src", fs.GenerateFilesystemObjectID());

            fs.AddObject(root);
            fs.AddObject(bin, root);
            fs.AddObject(hello, root);
            fs.AddObject(lib, root);
            fs.AddObject(src, root);

            BinaryManager.WriteBinaryToFile("debug.fs", BinaryManager.CreateBinaryFromFilesystem(fs));
            return fs;
        }
    }
}
