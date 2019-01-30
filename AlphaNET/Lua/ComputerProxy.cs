using MoonSharp.Interpreter;
using AlphaNET.Core;

namespace AlphaNET.Lua
{
    [MoonSharpUserData]
    class ComputerProxy
    {
        private Computer computer;

        public ComputerProxy(Computer computer)
        {
            this.computer = computer;
        }

        public void Write(string text)
        {
            computer.Write(text);
        }

        public void WriteLine(string text)
        {
            computer.WriteLine(text);
        }

        public string ReadLine()
        {
            return computer.ReadLine();
        }

        public void Clear()
        {
            computer.Clear();
        }
    }
}
