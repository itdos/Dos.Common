using System;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadArgument : IAstStackItem
    {
        public int argumentIndex;
        public Type argumentType;

        public Type itemType
        {
            get
            {
                return argumentType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            switch (argumentIndex)
            {
                case 0:
                    context.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    context.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    context.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    context.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    context.Emit(OpCodes.Ldarg, argumentIndex);
                    break;
            }
        }
    }

    class AstReadArgumentRef : AstReadArgument, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstReadArgumentValue : AstReadArgument, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    class AstReadArgumentAddr : AstReadArgument, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            context.Emit(OpCodes.Ldarga, argumentIndex);
        }
    }
}