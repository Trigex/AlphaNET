using System;

namespace AlphaNET.Framework.Client
{
    /// <summary>
    /// The Terminal Emulator based Console implementation, used in the AlphaNET.Client.Terminal client
    /// </summary>
    public class Terminal : IConsole
    {
        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public int Read()
        {
            return Console.Read();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}