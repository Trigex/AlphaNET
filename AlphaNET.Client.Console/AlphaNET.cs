using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
using System.Text;
using CommandLine;

namespace AlphaNET.Client.Console
{
    class AlphaNET
    {
        static string FS_PATH = "debug.fs";
        static string DEFAULT_IP = "127.0.0.1";
        const int DEFAULT_PORT = 1337;

        static void Main(string[] args)
        {
            CliArgs.Parse(args).WithParsed(o =>
            {
                Filesystem fs;
                string ip = DEFAULT_IP;
                int port = DEFAULT_PORT;

                if (o.Host != null)
                    ip = o.Host;

                if (o.Port != 0)
                    port = o.Port;

                if (o.FilesystemPath != null)
                    fs = BinaryManager.CreateFilesystemFromBinary(BinaryManager.ReadBinaryFromFile(o.FilesystemPath));
                else
                    fs = BootstrapFilesystem();

                if (o.Offline)
                    Init(new Computer(fs, true));
                else
                    Init(new Computer(fs, false, ip, port));

            });
        }

        static void Init(Computer computer)
        {
            computer.Start();
        }

        static Filesystem BootstrapFilesystem()
        {
            System.Console.WriteLine("Bootstraping filesystem...");
            Filesystem fs = new Filesystem();
            var root = new Directory("root", IOUtils.GenerateID());
            root.Owner = root;
            var bin = new Directory("bin", IOUtils.GenerateID());
            var sub = new Directory("sub", IOUtils.GenerateID());
            var lib = new Directory("lib", IOUtils.GenerateID());
            var hello = new File("hello.txt", sub, IOUtils.GenerateID(), true, Encoding.UTF8.GetBytes("Hello, World!"));
            var src = new Directory("src", IOUtils.GenerateID());

            fs.AddFilesystemObject(root);
            fs.AddFilesystemObject(bin, root);
            fs.AddFilesystemObject(sub, bin);
            fs.AddFilesystemObject(hello, sub);
            fs.AddFilesystemObject(lib, root);
            fs.AddFilesystemObject(src, root);

            BinaryManager.WriteBinaryToFile(FS_PATH, BinaryManager.CreateBinaryFromFilesystem(fs));
            return BinaryManager.CreateFilesystemFromBinary(BinaryManager.CreateBinaryFromFilesystem(fs));
        }
    }
}
