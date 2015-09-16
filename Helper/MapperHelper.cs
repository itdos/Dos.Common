using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dos.Common
{
    public class MapperHelper
    {
        public static TTo Map<TFrom, TTo>(TFrom from)
        {
            if (typeof(TFrom) == Types.Object)
            {
                return EntityCopy<TTo>(from);
            }
            var mapper = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<TFrom, TTo>();
            return mapper.Map(from);
        }
        public static List<TTo> Map<TFrom, TTo>(List<TFrom> from)
        {
            if (typeof(TFrom) == Types.Object)
            {
                return EntityCopy<TFrom,TTo>(from);
            }
            var mapper = EmitMapper.ObjectMapperManager.DefaultInstance.GetMapper<List<TFrom>, List<TTo>>();
            return mapper.Map(from);
        }
        private static TResult EntityCopy<TResult>(object input)
        {
            if (input == null)
            {
                return default(TResult);
            }
            if (input.GetType() == typeof(TResult))
            {
                return (TResult)input;
            }
            return (TResult)EntityCopy(input, typeof(TResult));
        }

        private static List<TResult> EntityCopy<TEntity, TResult>(IList<TEntity> input)
        {
            return input.Select(entity => EntityCopy<TResult>(entity)).ToList();
        }

        private static object EntityCopy(object input, Type targetType)
        {
            //emitmapper
            var objResult = Activator.CreateInstance(targetType);
            var properties = targetType.GetProperties();
            var type = input.GetType();
            foreach (var info in properties)
            {
                if (!info.CanWrite) continue;
                var property = type.GetProperty(info.Name);
                if (property == null) continue;
                var objTemp = property.GetValue(input, null);
                info.SetValue(objResult, objTemp, null);
            }
            return objResult;
        }
    }
}
