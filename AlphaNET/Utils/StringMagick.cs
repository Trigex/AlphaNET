using System;
using MoonSharp.Interpreter;

namespace AlphaNET.Utils
{
    static class StringMagick
    {
        public static string[] SplitStringBySpaces(string input)
        {
            return input.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool StartsWith(string input, string startsWith)
        {
            return input.StartsWith(startsWith);
        }
    }
}
