using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using EmitMapper.AST.Helpers;
using System.Reflection.Emit;

namespace EmitMapper.EmitBuilders
{
	class BuilderUtils
	{
		/// <summary>
		/// Copies an argument to local variable
		/// </summary>
		/// <param name="loc"></param>
		/// <param name="argIndex"></param>
		/// <returns></returns>
		public static IAstNode InitializeLocal(LocalBuilder loc, int argIndex)
		{
			return new AstComplexNode()
			{
				nodes =
					new List<IAstNode>()
					{
						new AstInitializeLocalVariable(loc),
						new AstWriteLocal()
						{
							localIndex = loc.LocalIndex,
							localType = loc.LocalType,
							value = AstBuildHelper.ReadArgumentRV(argIndex, typeof(object))
						}
					}
			};
		}
	}
}
