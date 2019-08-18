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
        public Std Stdout { get; set; }
        public Std Stdin { get; set; }
        public Std Stderr { get; set; }

        public enum Std
        {
            Console,
            File
        }

        public Terminal()
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
