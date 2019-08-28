namespace AlphaNET.Framework.Client
{
    /// <summary>
    /// Interface providing methods for text-based input and output 
    /// </summary>
    public interface IConsole
    {
        void Write(string text);
        void WriteLine(string text);
        int Read();
        string ReadLine();
        void Clear();
    }
}