using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstConstantNull : IAstRefOrValue
    {
        #region IAstReturnValueNode Members

        public Type itemType
        {
            get { return typeof(object); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldnull);
        }

        #endregion
    }
}