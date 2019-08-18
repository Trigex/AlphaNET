using AlphaNET.Framework.Client;
using System;

namespace AlphaNET.Client.Visual
{
    public static class AlphaNET
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new Visual(args))
            {
                game.Run();
            }
        }

        public static void ComputerThread(Computer computer)
        {
            computer.Init();
            computer.Start();
        }
    }
}
