using System;

namespace EmitMapper.AST
{
    class ILCompilationException:Exception
    {
        public ILCompilationException(string message)
            : base(message)
        {
        }

        public ILCompilationException(string message, params object[] p)
            : base(String.Format(message, p))
        {
        }

    }
}