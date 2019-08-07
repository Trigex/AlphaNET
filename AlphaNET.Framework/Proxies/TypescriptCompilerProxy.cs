using AlphaNET.Framework.JS;

namespace AlphaNET.Framework.Proxies
{
    public class TypescriptCompilerProxy
    {
        private TypescriptCompiler _compiler;

        public TypescriptCompilerProxy(TypescriptCompiler compiler)
        {
            _compiler = compiler;
        }

        public string CompileTypescript(string script)
        {
            return _compiler.Compile(script);
        }

        public string CompileTypescript(string[] scripts)
        {
            return _compiler.Compile(scripts);
        }
    }
}
