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
                Filesystem fs = null;

                if (o.Host != null)
                    ip = o.Host;

                if (o.Port != 0)
                    port = o.Port;

                if (o.FilesystemPath != null)
                    fs = BinaryManager.CreateFilesystemFromBinary(BinaryManager.ReadBinaryFromFile(o.FilesystemPath), o.FilesystemPath);
                
                if (o.Offline)
                    computer.Init(fs, true);
                else
                    computer.Init(fs, false, ip, port);
            });

            computer.Start();
        }
    }
}
