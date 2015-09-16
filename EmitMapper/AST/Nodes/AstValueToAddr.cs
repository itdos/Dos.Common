using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstValueToAddr: IAstAddr
    {
        public IAstValue value;
        public Type itemType
        {
            get 
            {
                return value.itemType; 
            }
        }

        public AstValueToAddr(IAstValue value)
        {
            this.value = value;
        }

        public void Compile(CompilationContext context)
        {
            LocalBuilder loc = context.ilGenerator.DeclareLocal(itemType);
            new AstInitializeLocalVariable(loc).Compile(context);
            new AstWriteLocal()
                {
                    localIndex = loc.LocalIndex,
                    localType = loc.LocalType,
                    value = value
                }.Compile(context);
            new AstReadLocalAddr(loc).Compile(context);
        }
    }
}