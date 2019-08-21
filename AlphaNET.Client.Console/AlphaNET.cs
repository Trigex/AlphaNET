using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
using CommandLine;

namespace AlphaNET.Client.Console
{
    internal class AlphaNET
    {
        private const string DefaultIp = "127.0.0.1";
        private const int DefaultPort = 1337;

        private static void Main(string[] args)
        {
            var computer = new Computer();

            CliArgs.Parse(args).WithParsed(o =>
            {
                var ip = DefaultIp;
                var port = DefaultPort;

                o.FilesystemPath = "debug.fs";

                if (o.Host != null)
                    ip = o.Host;

                if (o.Port != 0)
                    port = o.Port;

                var fs = o.FilesystemPath != null ? BinaryManager.CreateFilesystemFromBinary(BinaryManager.ReadBinaryFromFile(o.FilesystemPath), o.FilesystemPath) : null;

                if (o.Offline)
                    computer.Init(fs, true);
                else
                    computer.Init(fs, false, ip, port);
            });

            computer.Start();
        }
    }
}
