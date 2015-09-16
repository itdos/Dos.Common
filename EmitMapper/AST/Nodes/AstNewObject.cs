using System;
using System.Linq;
using EmitMapper.AST.Interfaces;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.Utils;
using EmitMapper.AST.Helpers;
using System.Collections.Generic;

namespace EmitMapper.AST.Nodes
{
    class AstNewObject: IAstRef
    {
        public Type objectType;
        public IAstStackItem[] ConstructorParams;

        public AstNewObject()
        { 
        }

        public AstNewObject(Type objectType, IAstStackItem[] ConstructorParams)
        {
            this.objectType = objectType;
            this.ConstructorParams = ConstructorParams;
        }

		
        #region IAstStackItem Members

        public Type itemType
        {
            get 
            {
                return objectType; 
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(CompilationContext context)
        {
			if (ReflectionUtils.IsNullable(objectType))
			{
				IAstRefOrValue underlyingValue;
				var underlyingType = Nullable.GetUnderlyingType(objectType);
                if (ConstructorParams == null || ConstructorParams.Length == 0)
				{
					LocalBuilder temp = context.ilGenerator.DeclareLocal(underlyingType);
					new AstInitializeLocalVariable(temp).Compile(context);
					underlyingValue = AstBuildHelper.ReadLocalRV(temp);
				}
				else
				{
					underlyingValue = (IAstValue)ConstructorParams[0];
				}

				ConstructorInfo constructor = objectType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, 
					null, 
					new[] { underlyingType }, 
					null);

				underlyingValue.Compile(context);
				context.EmitNewObject(constructor);
			}
			else
			{
				Type[] types;
				if (ConstructorParams == null || ConstructorParams.Length == 0)
				{
					types = new Type[0];
				}
				else
				{
					types = ConstructorParams.Select(c => c.itemType).ToArray();
					foreach (var p in ConstructorParams)
					{
						p.Compile(context);
					}
				}

				ConstructorInfo ci = objectType.GetConstructor(types);
				if (ci != null)
				{
					context.EmitNewObject(ci);
				}
                else if (objectType.IsValueType)
                {
                    LocalBuilder temp = context.ilGenerator.DeclareLocal(objectType);
                    new AstInitializeLocalVariable(temp).Compile(context);
                    AstBuildHelper.ReadLocalRV(temp).Compile(context);
                }
                else
                {
                    throw new Exception(
                        String.Format("Constructor for types [{0}] not found in {1}", types.ToCSV(","), objectType.FullName)
                    );
                }
			}
        }

        #endregion
    }
}