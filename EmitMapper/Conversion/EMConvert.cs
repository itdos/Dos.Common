using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration.MappingOperations;
using System.Reflection;

namespace EmitMapper
{
	public class EMConvert
	{
		static public object ChangeType(object value, Type conversionType)
		{
			if (value == null)
			{
				return null;
			}
			return ChangeType(value, value.GetType(), conversionType);
		}

		static public object ChangeTypeGeneric<TFrom, TTo>(object value)
		{
			return ChangeType(value, typeof(TFrom), typeof(TTo));
		}

		static public object ChangeType(object value, Type typeFrom, Type typeTo)
		{
			if (value == null)
			{
				return null;
			}

			if (typeTo.IsEnum)
			{
				return ConvertToEnum(value, typeFrom, typeTo);
			}

			if (typeFrom.IsEnum)
			{
				if (typeTo == typeof(string))
				{
					return value.ToString();
				}

#if SILVERLIGHT
                return ChangeType(ConvertSL.ChangeType(value, Enum.GetUnderlyingType(typeFrom)), Enum.GetUnderlyingType(typeFrom),typeTo);
#else
				return ChangeType(Convert.ChangeType(value, Enum.GetUnderlyingType(typeFrom)), Enum.GetUnderlyingType(typeFrom), typeTo);
#endif
			}

			if (typeTo == typeof(Guid))
			{
				if (value == null)
				{
					return new Guid();
				}
				return new Guid(value.ToString());
			}

			var isFromNullable = ReflectionUtils.IsNullable(typeFrom);
			var isToNullable = ReflectionUtils.IsNullable(typeTo);

			if (isFromNullable && !isToNullable)
			{
				return ChangeType(value, Nullable.GetUnderlyingType(typeFrom), typeTo);
			}

			if (isToNullable)
			{
				var ut = Nullable.GetUnderlyingType(typeTo);
				if (ut.IsEnum)
				{
					return ConvertToEnum(value, typeFrom, ut);
				}
				else
				{
					return ChangeType(value, typeFrom, ut);
				}
			}
#if SILVERLIGHT
            return ConvertSL.ChangeType(value, typeTo);
#else
			return Convert.ChangeType(value, typeTo);
#endif
		}

		public static string ObjectToString(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			return obj.ToString();
		}

		public static Guid StringToGuid(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return Guid.Empty;
			}
			return new Guid(str);
		}

		internal static TEnum ToEnum<TEnum, TUnder>(object obj)
		{
			if (obj is string)
			{
				var str = obj.ToString();

#if SILVERLIGHT
				return (TEnum)EnumSL.Parse(typeof(TEnum), str);
#else
				return (TEnum)Enum.Parse(typeof(TEnum), str);
#endif
			}
#if SILVERLIGHT
			return (TEnum)ConvertSL.ChangeType(obj, typeof(TUnder));
#else
			return (TEnum)Convert.ChangeType(obj, typeof(TUnder));
#endif
		}

		public static MethodInfo GetConversionMethod(Type from, Type to)
		{
			if (from == null || to == null)
			{
				return null;
			}

			if (to == typeof(string))
			{
				return typeof(EMConvert).GetMethod("ObjectToString", BindingFlags.Static | BindingFlags.Public);
			}

			if (to.IsEnum)
			{
				return typeof(EMConvert)
					.GetMethod("ToEnum", BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(to, Enum.GetUnderlyingType(to));
			}

			if (IsComplexConvert(from) || IsComplexConvert(to))
			{
				return
					typeof(EMConvert)
						.GetMethod(
							"ChangeTypeGeneric",
							BindingFlags.Static | BindingFlags.Public
						)
						.MakeGenericMethod(from, to);
			}
			return null;
		}

		private static bool IsComplexConvert(Type type)
		{
			if (type.IsEnum)
			{
				return true;
			}
			if (ReflectionUtils.IsNullable(type))
			{
				if (Nullable.GetUnderlyingType(type).IsEnum)
				{
					return true;
				}
			}
			return false;
		}

		static private object ConvertToEnum(object value, Type typeFrom, Type typeTo)
		{
			if (!typeFrom.IsEnum)
			{
				if (typeFrom == typeof(string))
				{
#if SILVERLIGHT
                    return EnumSL.Parse(typeTo, value.ToString());
#else
					return Enum.Parse(typeTo, value.ToString());
#endif
				}
			}
#if SILVERLIGHT
            return Enum.ToObject(typeTo, ConvertSL.ChangeType(value, Enum.GetUnderlyingType(typeTo)));
#else
			return Enum.ToObject(typeTo, Convert.ChangeType(value, Enum.GetUnderlyingType(typeTo)));
#endif
		}
	}
}