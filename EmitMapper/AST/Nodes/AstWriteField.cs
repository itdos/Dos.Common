using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;

namespace EmitMapper.AST.Nodes
{
    class AstWriteField: IAstNode
    {
        public IAstRefOrAddr targetObject;
        public IAstRefOrValue value;
        public FieldInfo fieldInfo;

        public void Compile(CompilationContext context)
        {
            targetObject.Compile(context);
            value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, fieldInfo.FieldType, value.itemType);
            context.Emit(OpCodes.Stfld, fieldInfo);
        }
    }
}