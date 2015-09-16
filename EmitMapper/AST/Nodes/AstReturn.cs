using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReturn : IAstNode, IAstAddr
    {
        public Type returnType;
        public IAstRefOrValue returnValue;

        public void Compile(CompilationContext context)
        {
            returnValue.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, returnType, returnValue.itemType);
            context.Emit(OpCodes.Ret);
        }

        public Type itemType
        {
            get { return returnType; }
        }
    }
}