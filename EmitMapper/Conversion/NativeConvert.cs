using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;
using System.Reflection;
using EmitMapper.AST.Helpers;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EmitMapper
{
	class NativeConverter
	{
		private static Type[] convertTypes = new[]
		{
			typeof(Boolean),
			typeof(Char),
			typeof(SByte),
			typeof(Byte),
			typeof(Int16),
			typeof(Int32),
			typeof(Int64),
			typeof(UInt16),
			typeof(UInt32),
			typeof(UInt64),
			typeof(Single),
			typeof(Double),
			typeof(Decimal),
			typeof(DateTime),
			typeof(String)
		};

		public static bool IsNativeConvertionPossible(Type from, Type to)
		{
			if (from == null || to == null)
			{
				return false;
			}

			if (convertTypes.Contains(from) && convertTypes.Contains(to))
			{
				return true;
			}

			if (to == typeof(string))
			{
				return true;
			}

			if (from == typeof(string) && to == typeof(Guid))
			{
				return true;
			}

            if (from.IsEnum && to.IsEnum)
            {
                return true;
            }

			if (from.IsEnum && convertTypes.Contains(to))
			{
				return true;
			}

			if (to.IsEnum && convertTypes.Contains(from))
			{
				return true;
			}

			if (ReflectionUtils.IsNullable(from))
			{
				return IsNativeConvertionPossible(Nullable.GetUnderlyingType(from), to);
			}

			if (ReflectionUtils.IsNullable(to))
			{
				return IsNativeConvertionPossible(from, Nullable.GetUnderlyingType(to));
			}

			return false;
		}

		public static IAstRefOrValue Convert(
			Type destinationType, 
			Type sourceType, 
			IAstRefOrValue sourceValue
			)
		{
			if (destinationType == sourceValue.itemType)
			{
				return sourceValue;
			}

			if (destinationType == typeof(string))
			{
				return 
					new AstCallMethodRef(
						typeof(NativeConverter).GetMethod(
						"ObjectToString",
						BindingFlags.NonPublic | BindingFlags.Static
					),
					null,
                    new List<IAstStackItem>()
					{
						sourceValue
					}
				);
			}

			foreach (var m in typeof(Convert).GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				if (m.ReturnType == destinationType)
				{
					var parameters = m.GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == sourceType)
					{
						return
							AstBuildHelper.CallMethod(
								m,
								null,
                                new List<IAstStackItem> { sourceValue }
							);
					}
				}
			}

			return AstBuildHelper.CallMethod(
				typeof(EMConvert).GetMethod(
					"ChangeType", 
					new[] 
					{ 
						typeof(object), 
						typeof(Type), 
						typeof(Type)
					}
				),
				null,
				new List<IAstStackItem>{
					sourceValue,
					new AstTypeof(){type = sourceType},
					new AstTypeof(){type = destinationType}
				}
			);
		}

		internal static string ObjectToString(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			return obj.ToString();
		}
	}
}
