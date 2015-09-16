using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    class AstThrow: IAstNode
    {
        public IAstRef exception;

        public void Compile(CompilationContext context)
        {
            exception.Compile(context);
            context.Emit(OpCodes.Throw);
        }
    }
}