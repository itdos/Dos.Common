using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstCallMethodVoid : IAstNode
    {
        protected MethodInfo methodInfo;
		protected IAstRefOrAddr invocationObject;
        protected List<IAstStackItem> arguments;

		public AstCallMethodVoid(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
            List<IAstStackItem> arguments)
		{
			this.methodInfo = methodInfo;
			this.invocationObject = invocationObject;
			this.arguments = arguments;
		}

        public void Compile(CompilationContext context)
        {
			new AstCallMethod(methodInfo, invocationObject, arguments).Compile(context);

            if (methodInfo.ReturnType != typeof(void))
            {
                context.Emit(OpCodes.Pop);
            }
        }
    }
}