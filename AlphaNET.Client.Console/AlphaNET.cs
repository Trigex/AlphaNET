using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
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
                    fs = null;

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
    }
}
