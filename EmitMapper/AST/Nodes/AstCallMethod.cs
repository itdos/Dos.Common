using System;
using System.Collections.Generic;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;


namespace EmitMapper.AST.Nodes
{
    class AstCallMethod: IAstRefOrValue
    {
        public MethodInfo methodInfo;
        public IAstRefOrAddr invocationObject;
        public List<IAstStackItem> arguments;

		public AstCallMethod(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
		{
			if (methodInfo == null)
			{
				throw new InvalidOperationException("methodInfo is null");
			}
			this.methodInfo = methodInfo;
			this.invocationObject = invocationObject;
			this.arguments = arguments;
		}

        public Type itemType
        {
            get
            {
                return methodInfo.ReturnType;
            }
        }

        public virtual void Compile(CompilationContext context)
        {
            CompilationHelper.EmitCall(context, invocationObject, methodInfo, arguments);
        }
    }

    class AstCallMethodRef : AstCallMethod, IAstRef
    {
        public AstCallMethodRef(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
            : base(methodInfo, invocationObject, arguments)
		{
		}

        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstCallMethodValue : AstCallMethod, IAstValue
    {
        public AstCallMethodValue(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
			: base(methodInfo, invocationObject, arguments)
		{
		}
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }
}