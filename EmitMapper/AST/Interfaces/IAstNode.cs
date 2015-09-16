using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;

namespace EmitMapper.AST.Interfaces
{
    interface IAstNode
    {
        void Compile(CompilationContext context);
    }
}