using System.Reflection.Emit;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstIf: IAstNode
    {
        public IAstValue condition;
        public AstComplexNode trueBranch;
        public AstComplexNode falseBranch;

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
            Label elseLabel = context.ilGenerator.DefineLabel();
            Label endIfLabel = context.ilGenerator.DefineLabel();

            condition.Compile(context);
            context.Emit(OpCodes.Brfalse, elseLabel);

            if (trueBranch != null)
            {
                trueBranch.Compile(context);
            }
            if (falseBranch != null)
            {
                context.Emit(OpCodes.Br, endIfLabel);
            }

            context.ilGenerator.MarkLabel(elseLabel);
            if (falseBranch != null)
            {
                falseBranch.Compile(context);
            }
            context.ilGenerator.MarkLabel(endIfLabel);
        }

        #endregion
    }
}