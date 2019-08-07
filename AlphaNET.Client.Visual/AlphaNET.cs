using System;

namespace AlphaNET.Client.Visual
{
    public static class AlphaNET
    {
        const string FS_PATH = "debug.fs";

        [STAThread]
        static void Main()
        {
            using (var game = new Visual())
            {
                game.Run();
            }
        }
    }
}
