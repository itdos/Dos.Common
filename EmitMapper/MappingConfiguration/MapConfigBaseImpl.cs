using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration.MappingOperations;
using System.Reflection;
using EmitMapper.Conversion;

namespace EmitMapper.MappingConfiguration
{
    public abstract class MapConfigBaseImpl : IMappingConfigurator
    {
        private string _configurationName;
        private TypeDictionary<Delegate> _customConverters = new TypeDictionary<Delegate>();
        private TypeDictionary<ICustomConverterProvider> _customConvertersGeneric = new TypeDictionary<ICustomConverterProvider>();
        private TypeDictionary<Delegate> _customConstructors = new TypeDictionary<Delegate>();
        private TypeDictionary<Delegate> _nullSubstitutors = new TypeDictionary<Delegate>();
        private TypeDictionary<Delegate> _postProcessors = new TypeDictionary<Delegate>();
        private TypeDictionary<List<string>> _ignoreMembers = new TypeDictionary<List<string>>();

        public abstract IMappingOperation[] GetMappingOperations(Type from, Type to);

		public MapConfigBaseImpl()
		{
			RegisterDefaultCollectionConverters();
		}

        public virtual string GetConfigurationName()
        {
			return _configurationName;
        }

		public virtual void BuildConfigurationName()
		{
			_configurationName = 
				new[] 
				{ 
					ToStr(_customConverters),
					ToStr(_nullSubstitutors),
					ToStr(_ignoreMembers),
                    ToStr(_postProcessors),
					ToStr(_customConstructors),
				}.ToCSV(";");
		}

		public virtual StaticConvertersManager GetStaticConvertersManager()
		{
			return null;
		}

        public virtual IRootMappingOperation GetRootMappingOperation(Type from, Type to)
        {
            var converter = _customConverters.GetValue(new[] { from, to });
            if (converter == null)
            {
                converter = GetGenericConverter(from, to);
            }

            return new RootMappingOperation(from, to)
            {
                TargetConstructor = _customConstructors.GetValue(new[] { to }),
                NullSubstitutor = _nullSubstitutors.GetValue(new[] { to }),
                ValuesPostProcessor = _postProcessors.GetValue(new[] { to }),
                Converter = converter
            };
        }

        /// <summary>
        /// Define custom type converter
        /// </summary>
        /// <typeparam name="From">Source type</typeparam>
        /// <typeparam name="To">Destination type</typeparam>
        /// <Param name="converter">Function which converts an inctance of the source type to an instance of the destination type</Param>
        /// <returns></returns>
        public IMappingConfigurator ConvertUsing<From, To>(Func<From, To> converter)
        {
            _customConverters.Add(new[] { typeof(From), typeof(To) }, (ValueConverter<From, To>)((v, s) => converter(v)));
            return this;
        }

		/// <summary>
		/// Define conversion for a generic. It is able to convert not one particular class but all generic family
		/// providing a generic converter.
		/// </summary>
		/// <Param name="from">Type of source. Can be also generic class or abstract array.</Param>
		/// <Param name="to">Type of destination. Can be also generic class or abstract array.</Param>
		/// <Param name="converterProvider">Provider for getting detailed information about generic conversion</Param>
		/// <returns></returns>
        public IMappingConfigurator ConvertGeneric(Type from, Type to, ICustomConverterProvider converterProvider)
        {
            _customConvertersGeneric.Add(new[] { from, to }, converterProvider);
            return this;
        }


        /// <summary>
        /// Setup function which returns value for destination if appropriate source member is null. 
        /// </summary>
        /// <typeparam name="TFrom">Type of source member</typeparam>
        /// <typeparam name="TTo">Type of destination member</typeparam>
        /// <Param name="nullSubstitutor">Function which returns value for destination if appropriate source member is null</Param>
        /// <returns></returns>
        public IMappingConfigurator NullSubstitution<TFrom, TTo>(Func<object, TTo> nullSubstitutor)
        {
            _nullSubstitutors.Add(new[] { typeof(TFrom), typeof(TTo) }, nullSubstitutor);
            return this;
        }

        /// <summary>
        /// Define members which should be ingored
        /// </summary>
        /// <Param name="typeFrom">Source type for which ignore members are defining</Param>
        /// <Param name="typeTo">Destination type for which ignore members are defining</Param>
        /// <Param name="ignoreNames">Array of member names which should be ingored</Param>
        /// <returns></returns>
        public IMappingConfigurator IgnoreMembers(Type typeFrom, Type typeTo, string[] ignoreNames)
        {
            var ig = _ignoreMembers.GetValue(new[] { typeFrom, typeTo });
            if (ig == null)
            {
                _ignoreMembers.Add(new[] { typeFrom, typeTo }, ignoreNames.ToList());
            }
            else
            {
                ig.AddRange(ignoreNames);
            }
            return this;
        }

        /// <summary>
        /// Define members which should be ingored
        /// </summary>
        /// <typeparam name="TFrom">Source type for which ignore members are defining</typeparam>
        /// <typeparam name="TTo">Destination type for which ignore members are defining</typeparam>
        /// <Param name="ignoreNames">Array of member names which should be ingored</Param>
        /// <returns></returns>
        public IMappingConfigurator IgnoreMembers<TFrom, TTo>(string[] ignoreNames)
        {
            return IgnoreMembers(typeof(TFrom), typeof(TTo), ignoreNames);
        }

        /// <summary>
        /// Define a custom constructor for the specified type
        /// </summary>
        /// <typeparam name="T">Type for which constructor is defining</typeparam>
        /// <Param name="constructor">Custom constructor</Param>
        /// <returns></returns>
        public IMappingConfigurator ConstructBy<T>(TargetConstructor<T> constructor)
        {
            _customConstructors.Add(new[] { typeof(T) }, constructor);
            return this;
        }

		/// <summary>
		/// Define postprocessor for specified type
		/// </summary>
		/// <typeparam name="T">Objects of this type and all it's descendants will be postprocessed</typeparam>
		/// <Param name="postProcessor"></Param>
		/// <returns></returns>
        public IMappingConfigurator PostProcess<T>(ValuesPostProcessor<T> postProcessor)
        {
            _postProcessors.Add(new[] { typeof(T) }, postProcessor);
            return this;
        }

        /// <summary>
        /// Set unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
        /// </summary>
        /// <Param name="mapperName">Configuration name</Param>
        /// <returns></returns>
        public IMappingConfigurator SetConfigName(string configurationName)
        {
            _configurationName = configurationName;
            return this;
        }

        protected IEnumerable<IMappingOperation> FilterOperations(
            Type from,
            Type to,
            IEnumerable<IMappingOperation> operations
        )
        {
            List<IMappingOperation> result = new List<IMappingOperation>();
            foreach (var op in operations)
            {
                if (op is IReadWriteOperation)
                {
                    var o = op as IReadWriteOperation;
                    if (TestIgnore(from, to, o.Source, o.Destination))
                    {
                        continue;
                    }

                    o.NullSubstitutor = _nullSubstitutors.GetValue(new[] { o.Source.MemberType, o.Destination.MemberType });
                    o.TargetConstructor = _customConstructors.GetValue(new[] { o.Destination.MemberType });
                    o.Converter = _customConverters.GetValue(new[] { o.Source.MemberType, o.Destination.MemberType });
                    if (o.Converter == null)
                    {
                        o.Converter = GetGenericConverter(o.Source.MemberType, o.Destination.MemberType);
                    }
                }
                if (op is ReadWriteComplex)
                {
                    var o = op as ReadWriteComplex;
                    o.ValuesPostProcessor = _postProcessors.GetValue(new[] { o.Destination.MemberType });
                }
                if (op is IComplexOperation)
                {
                    var o = op as IComplexOperation;
                    var orw = op as IReadWriteOperation;
                    o.Operations =
                        FilterOperations(
                            orw == null ? from : orw.Source.MemberType,
                            orw == null ? to : orw.Destination.MemberType,
                            o.Operations
                        ).ToList();
                }

                result.Add(op);
            }

            return result;
        }

        private Delegate GetGenericConverter(Type from, Type to)
        {
            var converter = _customConvertersGeneric.GetValue(new[] { from, to });
            if (converter == null)
            {
                return null;
            }

            var converterDescr = converter.GetCustomConverterDescr(from, to, this);

            if (converterDescr == null)
            {
                return null;
            }

            Type genericConverter;
            if (converterDescr.ConverterClassTypeArguments != null && converterDescr.ConverterClassTypeArguments.Length > 0)
            {
                genericConverter = converterDescr.ConverterImplementation.MakeGenericType(converterDescr.ConverterClassTypeArguments);
            }
            else
            {
                genericConverter = converterDescr.ConverterImplementation;
            }

            MethodInfo mi = genericConverter.GetMethod(converterDescr.ConversionMethodName);

            object converterObj = Activator.CreateInstance(genericConverter);
            if (converterObj is ICustomConverter)
            {
                ((ICustomConverter)converterObj).Initialize(from, to, this);
            }

            return Delegate.CreateDelegate(
                typeof(Func<,,>).MakeGenericType(from, typeof(object), to),
                converterObj,
                mi
            );
        }

        private bool TestIgnore(Type from, Type to, MemberDescriptor fromDescr, MemberDescriptor toDescr)
        {
            var ignore = _ignoreMembers.GetValue(new[] { from, to });
            if (
                ignore != null &&
                    (
                        ignore.Contains(fromDescr.MemberInfo.Name) ||
                        ignore.Contains(toDescr.MemberInfo.Name)
                    )
                )
            {
                return true;
            }
            return false;
        }


        protected static string ToStrEnum<T>(IEnumerable<T> t)
        {
            return t == null ? "" : t.ToCSV("|");
        }

        protected static string ToStr<T>(T t) where T : class
        {
            return t == null ? "" : t.ToString();
        }

		protected virtual void RegisterDefaultCollectionConverters()
		{
			ConvertGeneric(typeof(ICollection<>), typeof(Array), new ArraysConverterProvider());
		}
    }
}
