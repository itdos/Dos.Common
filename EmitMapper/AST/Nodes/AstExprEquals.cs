using System;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstExprEquals : IAstValue
    {
        IAstValue leftValue;
        IAstValue rightValue;

        public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
        }

        #region IAstReturnValueNode Members

        public Type itemType
        {
            get { return typeof(Int32); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            leftValue.Compile(context);
            rightValue.Compile(context);
            context.Emit(OpCodes.Ceq);
        }

        #endregion
    }
}