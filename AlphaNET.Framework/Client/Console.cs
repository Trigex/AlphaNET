using System;

namespace AlphaNET.Framework.Client
{
    public interface IConsole
    {
        void Write(string text);
        void WriteLine(string text);
        int Read();
        string ReadLine();
        void Clear();
    }

    public class Terminal : IConsole
    {
        public enum Std
        {
            Console,
            File
        }

        public Std Stdout { get; set; }
        public Std Stdin { get; set; }
        public Std Stderr { get; set; }

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