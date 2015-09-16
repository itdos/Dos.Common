using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadLocal : IAstStackItem
    {
        public int localIndex;
        public Type localType;

        public Type itemType
        {
            get
            {
                return localType;
            }
        }

        public AstReadLocal()
        {
        }

        public AstReadLocal(LocalBuilder loc)
        {
            localIndex = loc.LocalIndex;
            localType = loc.LocalType;
        }

        public virtual void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldloc, localIndex);
        }
    }

    class AstReadLocalRef : AstReadLocal, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstReadLocalValue : AstReadLocal, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    class AstReadLocalAddr : AstReadLocal, IAstAddr
    {
        public AstReadLocalAddr(LocalBuilder loc)
        {
            localIndex = loc.LocalIndex;
            localType = loc.LocalType.MakeByRefType();
        }

        override public void Compile(CompilationContext context)
        {
            //CompilationHelper.CheckIsValue(itemType);
            context.Emit(OpCodes.Ldloca, localIndex);
        }
    }
}