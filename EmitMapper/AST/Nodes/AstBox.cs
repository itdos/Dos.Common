using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstBox : IAstRef
    {
        public IAstRefOrValue value;

        #region IAstReturnValueNode Members

        public Type itemType
        {
            get 
            {
                return value.itemType;  
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            value.Compile(context);

            if (value.itemType.IsValueType)
            {
                context.Emit(OpCodes.Box, itemType);
            }
        }

        #endregion
    }
}