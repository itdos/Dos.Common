using System;
using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    class AstExceptionHandlingBlock : IAstNode
    {
        IAstNode protectedBlock;
        IAstNode handlerBlock;
        Type exceptionType;
        LocalBuilder eceptionVariable;

        public AstExceptionHandlingBlock(
            IAstNode protectedBlock, 
            IAstNode handlerBlock, 
            Type exceptionType,
            LocalBuilder eceptionVariable)
        {
            this.protectedBlock = protectedBlock;
            this.handlerBlock = handlerBlock;
            this.exceptionType = exceptionType;
            this.eceptionVariable = eceptionVariable;
        }

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            var endBlock = context.ilGenerator.BeginExceptionBlock();
            protectedBlock.Compile(context);
            context.ilGenerator.BeginCatchBlock(exceptionType);
            context.ilGenerator.Emit(OpCodes.Stloc, eceptionVariable);
            handlerBlock.Compile(context);
            context.ilGenerator.EndExceptionBlock();
        }

        #endregion
    }
}