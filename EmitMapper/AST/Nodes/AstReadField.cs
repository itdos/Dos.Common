using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadField: IAstStackItem
    {
        public IAstRefOrAddr sourceObject;
        public FieldInfo fieldInfo;

        public Type itemType
        {
            get
            {
                return fieldInfo.FieldType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            sourceObject.Compile(context);
            context.Emit(OpCodes.Ldfld, fieldInfo);
        }
    }

    class AstReadFieldRef : AstReadField, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstReadFieldValue : AstReadField, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    class AstReadFieldAddr : AstReadField, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            sourceObject.Compile(context);
            context.Emit(OpCodes.Ldflda, fieldInfo);
        }
    }
}