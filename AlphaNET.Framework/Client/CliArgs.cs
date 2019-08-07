using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaNET.Framework.Client
{
    public class ClientOptions
    {
        [Option('f', "filesystem", Required = false, HelpText = "Specify a filesystem to load")]
        public string FilesystemPath { get; set; }
        [Option('h', "host", Required = false, HelpText = "Specify a custom-server IP address")]
        public string Host { get; set; }
        [Option('p', "port", Required = false, HelpText = "Specify a custom-server port")]
        public int Port { get; set; }
        [Option('o', "offline", Required = false, HelpText = "Start in offline mode")]
        public bool Offline { get; set; }
    }

    public static class CliArgs
    {
        public static ParserResult<ClientOptions> Parse(string[] args)
        {
            return Parser.Default.ParseArguments<ClientOptions>(args);
        }
    }
}
