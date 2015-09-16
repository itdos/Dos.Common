using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitMapper.AST.Nodes
{
    class AstConstantString: IAstRef
    {
        public string str;

        #region IAstStackItem Members

        public Type itemType
        {
            get 
            {
                return typeof(string);
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            context.Emit(OpCodes.Ldstr, str);
        }

        #endregion
    }
}