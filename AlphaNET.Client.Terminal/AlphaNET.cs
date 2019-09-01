using AlphaNET.Framework.Client;
using AlphaNET.Framework.IO;
using CommandLine;

namespace AlphaNET.Client.Terminal
{
    internal class AlphaNET
    {
        private const string DefaultIp = "127.0.0.1";
        private const int DefaultPort = 1337;

        private static void Main(string[] args)
        {
            /*CliArgs.Parse(args).WithParsed(o =>
            {
                Computer computer;
                var ip = DefaultIp;
                var port = DefaultPort;
                var fsFilePath = "debug.fs";

                if (o.Host != null)
                    ip = o.Host;

                if (o.Port != 0)
                    port = o.Port;

                if (o.FilesystemPath != null)
                    fsFilePath = o.FilesystemPath;
                
                if (o.Offline)
                    computer = new Computer(fsFilePath, new Framework.Client.Terminal(), true);
                else
                    computer = new Computer(fsFilePath, new Framework.Client.Terminal(), false, ip, port);
                
                computer.Start();
            });*/
            var fs = new Filesystem("file.fs");
            // 256 mb fs
            fs.NewFilesystemAsync(536870912);
            //fs.LoadFilesystemAsync();
            fs.Close();
            
        }
    }
}
