using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System.Reflection;

namespace EmitMapper.MappingConfiguration
{
    public abstract class MapConfigBase<TDerived> : MapConfigBaseImpl where TDerived : class
	{
		private TDerived _derived;
		public void Init(TDerived derived)
		{
			_derived = derived;
		}

		/// <summary>
		/// Define custom type converter
		/// </summary>
		/// <typeparam name="From">Source type</typeparam>
		/// <typeparam name="To">Destination type</typeparam>
		/// <Param name="converter">Function which converts an inctance of the source type to an instance of the destination type</Param>
		/// <returns></returns>
		public TDerived ConvertUsing<From, To>(Func<From, To> converter)
		{
            return (TDerived)base.ConvertUsing(converter);
		}

		/// <summary>
		/// Define conversion for a generic. It is able to convert not one particular class but all generic family
		/// providing a generic converter.
		/// </summary>
		/// <Param name="from">Type of source. Can be also generic class or abstract array.</Param>
		/// <Param name="to">Type of destination. Can be also generic class or abstract array.</Param>
		/// <Param name="converterProvider">
		/// Provider for getting detailed information about generic conversion.
		/// </Param>
		/// <returns></returns>
        public TDerived ConvertGeneric(Type from, Type to, ICustomConverterProvider converterProvider)
        {
            return (TDerived)base.ConvertGeneric(from, to, converterProvider);
        }

		/// <summary>
		/// Setup function which returns value for destination if appropriate source member is null. 
		/// </summary>
		/// <typeparam name="TFrom">Type of source member</typeparam>
		/// <typeparam name="TTo">Type of destination member</typeparam>
		/// <Param name="nullSubstitutor">Function which returns value for destination if appropriate source member is null</Param>
		/// <returns></returns>
        public TDerived NullSubstitution<TFrom, TTo>(Func<object, TTo> nullSubstitutor)
		{
            return (TDerived)base.NullSubstitution<TFrom, TTo>(nullSubstitutor);
		}

		/// <summary>
		/// Define members which should be ingored
		/// </summary>
		/// <Param name="typeFrom">Source type for which ignore members are defining</Param>
		/// <Param name="typeTo">Destination type for which ignore members are defining</Param>
		/// <Param name="ignoreNames">Array of member names which should be ingored</Param>
		/// <returns></returns>
		public TDerived IgnoreMembers(Type typeFrom, Type typeTo, string[] ignoreNames)
		{
            return (TDerived)base.IgnoreMembers(typeFrom, typeTo, ignoreNames);
		}

		/// <summary>
		/// Define members which should be ingored
		/// </summary>
		/// <typeparam name="TFrom">Source type for which ignore members are defining</typeparam>
		/// <typeparam name="TTo">Destination type for which ignore members are defining</typeparam>
		/// <Param name="ignoreNames">Array of member names which should be ingored</Param>
		/// <returns></returns>
		public TDerived IgnoreMembers<TFrom, TTo>(string[] ignoreNames)
		{
            return (TDerived)base.IgnoreMembers<TFrom, TTo>(ignoreNames);
		}

		/// <summary>
		/// Define a custom constructor for the specified type
		/// </summary>
		/// <typeparam name="T">Type for which constructor is defining</typeparam>
		/// <Param name="constructor">Custom constructor</Param>
		/// <returns></returns>
		public TDerived ConstructBy<T>(TargetConstructor<T> constructor)
		{
            return (TDerived)base.ConstructBy<T>(constructor);
		}

		/// <summary>
		/// Define postprocessor for specified type
		/// </summary>
		/// <typeparam name="T">Objects of this type and all it's descendants will be postprocessed</typeparam>
		/// <Param name="postProcessor"></Param>
		/// <returns></returns>
		public TDerived PostProcess<T>(ValuesPostProcessor<T> postProcessor)
		{
            return (TDerived)base.PostProcess<T>(postProcessor);
		}

		/// <summary>
		/// Set unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
		/// </summary>
		/// <Param name="mapperName">Configuration name</Param>
		/// <returns></returns>
		public TDerived SetConfigName(string configurationName)
		{
            return (TDerived)base.SetConfigName(configurationName);
		}
	}
}
