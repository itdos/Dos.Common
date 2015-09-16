using System;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstReadProperty : IAstRefOrValue
    {
        public IAstRefOrAddr sourceObject;
        public PropertyInfo propertyInfo;

        public Type itemType
        {
            get
            {
                return propertyInfo.PropertyType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            MethodInfo mi = propertyInfo.GetGetMethod();

            if (mi == null)
            {
                throw new Exception("Property " + propertyInfo.Name + " doesn't have get accessor");
            }

            AstBuildHelper.CallMethod(mi, sourceObject, null).Compile(context);
        }
    }

    class AstReadPropertyRef : AstReadProperty, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstReadPropertyValue : AstReadProperty, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }
}