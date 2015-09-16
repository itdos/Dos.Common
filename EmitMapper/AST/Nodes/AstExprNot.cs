using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
	class AstExprNot : IAstValue
	{
		IAstRefOrValue _value;

		public Type itemType
		{
			get { return typeof(Int32); }
		}

        public AstExprNot(IAstRefOrValue value)
		{
			_value = value;
		}

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ldc_I4_0);
			_value.Compile(context);
			context.Emit(OpCodes.Ceq);
		}
	}
}
