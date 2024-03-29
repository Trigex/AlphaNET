﻿using AlphaNET.Framework.Client;
using System;

namespace AlphaNET.Client.Visual
{
    public static class AlphaNET
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var game = new Visual(args))
            {
                game.Run();
            }
        }

        public static void ComputerThread(Computer computer, Tty tty)
        {
            computer.Init(null, false, "127.0.0.1", 1337, tty);
            computer.Start();
        }
    }
}
