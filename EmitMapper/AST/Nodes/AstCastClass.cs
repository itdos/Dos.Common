using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.AST.Interfaces;
using System.Reflection.Emit;
using EmitMapper.AST.Helpers;

namespace EmitMapper.AST.Nodes
{
	class AstCastclass : IAstRefOrValue
	{
        protected IAstRefOrValue _value;
        protected Type _targetType;

        public AstCastclass(IAstRefOrValue value, Type targetType)
		{
			_value = value;
			_targetType = targetType;
		}

		#region IAstStackItem Members

		public Type itemType
		{
			get { return _targetType; }
		}

		#endregion

		#region IAstNode Members

		public virtual void Compile(CompilationContext context)
		{

            if (_value.itemType != _targetType)
            {
                if (!_value.itemType.IsValueType && !_targetType.IsValueType)
                {
                    _value.Compile(context);
                    context.Emit(OpCodes.Castclass, _targetType);
                    return;
                }
                else if (_targetType.IsValueType && !_value.itemType.IsValueType)
                {
                    new AstUnbox() { refObj = (IAstRef)_value, unboxedType = _targetType }.Compile(context);
                    return;
                }

                throw new EmitMapperException();
            }
            else
            {
                _value.Compile(context);
            }
		}

		#endregion
	}

    class AstCastclassRef : AstCastclass, IAstRef
    {
        public AstCastclassRef(IAstRefOrValue value, Type targetType): base(value, targetType)
		{
		}

        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    class AstCastclassValue : AstCastclass, IAstValue
    {
        public AstCastclassValue(IAstRefOrValue value, Type targetType): base(value, targetType)
		{
		}

        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }
}
