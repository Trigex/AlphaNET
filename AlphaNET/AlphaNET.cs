using System;
using System.Diagnostics;
using System.Text;
using AlphaFramework.Client;
using AlphaFramework.IO;
using AlphaFramework.JS;
using Jint;

namespace AlphaNET
{
    class AlphaNET
    {
        const string FS_PATH = "debug.fs";
        static void Main(string[] args)
        {
            // Create filesystem
            Filesystem fs = BootstrapFilesystem();
            // Create computer
            Computer computer = new Computer(fs);
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
