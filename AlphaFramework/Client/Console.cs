using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaFramework.Client
{
    public class Console
    {
        public Std stdout { get; set; }
        public Std stdin { get; set; }
        public Std stderr { get; set; }
        public enum Std
        {
            Console,
            File
        }

        public Console()
        {

        }

        public void Write(string text)
        {
            System.Console.Write(text);
        }

        public void WriteLine(string text)
        {
            System.Console.WriteLine(text);
        }

        public int Read()
        {
            return System.Console.Read();
        }
        
        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public void Clear()
        {
            System.Console.Clear();
        }
    }
}
