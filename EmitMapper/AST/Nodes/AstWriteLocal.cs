using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstWriteLocal : IAstNode
    {
        public int localIndex;
        public Type localType;
        public IAstRefOrValue value;

        public AstWriteLocal()
        {
        }

        public AstWriteLocal(LocalBuilder loc, IAstRefOrValue value)
        {
            localIndex = loc.LocalIndex;
            localType = loc.LocalType;
            this.value = value;
        }


        public void Compile(CompilationContext context)
        {
            value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, localType, value.itemType);
            context.Emit(OpCodes.Stloc, localIndex);
        }
    }
}