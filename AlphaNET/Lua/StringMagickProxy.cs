using MoonSharp.Interpreter;
using AlphaNET.Utils;

namespace AlphaNET.Lua
{
    /// <summary>
    /// Proxy to the StringMagick class, to add type support for MoonSharp
    /// </summary>
    [MoonSharpUserData]
    static class StringMagickProxy
    {
        public static string[] SplitStringBySpaces(DynValue input)
        {
            string buf = input.CastToString();
            return StringMagick.SplitStringBySpaces(buf);
        }
    }
}
