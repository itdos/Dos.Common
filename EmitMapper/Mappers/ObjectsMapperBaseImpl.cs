using System;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.EmitInvoker;

namespace EmitMapper.Mappers
{
    /// <summary>
    /// Base class for Mappers
    /// </summary>
    public abstract class ObjectsMapperBaseImpl
    {
        /// <summary>
        /// Copies object properties and members of "from" to object "to"
        /// </summary>
        /// <param name="from">Source object</param>
        /// <param name="to">Destination object</param>
        /// <returns>Destination object</returns>
        public virtual object Map(object from, object to, object state)
        {
            object result;
            if (from == null)
            {
                result = _nullSubstitutor == null ? null : _nullSubstitutor.CallFunc();
            }
            else if (_converter != null)
            {
                result = _converter.CallFunc(from, state);
            }
            else
            {
                if (to == null)
                {
                    to = ConstructTarget();
                }

                result = MapImpl(from, to, state);
            }

            if (_valuesPostProcessor != null)
            {
                result = _valuesPostProcessor.CallFunc(result, state);
            }
            return result;
        }

        /// <summary>
        /// Creates new instance of destination object and initializes it by values from "from" object
        /// </summary>
        /// <param name="from">source object</param>
        /// <returns></returns>
        public virtual object Map(object from)
        {
            if (from == null)
            {
                return null;
            }
            object to = ConstructTarget();
            return Map(from, to, null);
        }

		public IMappingConfigurator MappingConfigurator
        {
            get
            {
				return _mappingConfigurator;
            }
        }

        #region Non-public members

        /// <summary>
        /// Mapper manager
        /// </summary>
        internal ObjectMapperManager mapperMannager;

        /// <summary>
        /// Type of source object
        /// </summary>
        internal Type typeFrom;

        /// <summary>
        /// Type of destination object
        /// </summary>
        internal Type typeTo;

        /// <summary>
        /// True, if reference properties and members of same type should
        /// be copied by reference (shallow copy, without creating new instance for destination object)
        /// </summary>
        internal bool ShallowCopy = true;

        /// <summary>
        /// Copies object properties and members of "from" to object "to"
        /// </summary>
        /// <param name="from">Source object</param>
        /// <param name="to">Destination object</param>
        /// <returns>Destination object</returns>
        internal abstract object MapImpl(object from, object to, object state);

        /// <summary>
        /// Creates an instance of destination object
        /// </summary>
        /// <returns>Destination object</returns>
        internal abstract object CreateTargetInstance();

		protected IMappingConfigurator _mappingConfigurator;
		protected IRootMappingOperation _rootOperation;

		public object[] StroredObjects;

        protected DelegateInvokerFunc_0 _targetConstructor;
        protected DelegateInvokerFunc_0 _nullSubstitutor;
        protected DelegateInvokerFunc_2 _converter;
        protected DelegateInvokerFunc_2 _valuesPostProcessor;

        internal void Initialize(
            ObjectMapperManager MapperMannager, 
            Type TypeFrom, 
            Type TypeTo, 
			IMappingConfigurator mappingConfigurator,
			object[] stroredObjects)
        {
            mapperMannager = MapperMannager;
            typeFrom = TypeFrom;
            typeTo = TypeTo;
			_mappingConfigurator = mappingConfigurator;
			StroredObjects = stroredObjects;
			if (_mappingConfigurator != null)
			{
				_rootOperation = _mappingConfigurator.GetRootMappingOperation(TypeFrom, TypeTo);
				if (_rootOperation == null)
				{
					_rootOperation = new RootMappingOperation(TypeFrom, TypeTo);
				}
				var constructor = _rootOperation.TargetConstructor;
                if (constructor != null)
                {
                    _targetConstructor = (DelegateInvokerFunc_0)DelegateInvoker.GetDelegateInvoker(constructor);
                }

                var valuesPostProcessor = _rootOperation.ValuesPostProcessor;
                if (valuesPostProcessor != null)
                {
                    _valuesPostProcessor = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker(valuesPostProcessor);
                }

                var converter = _rootOperation.Converter;
                if (converter != null)
                {
                    _converter = (DelegateInvokerFunc_2)DelegateInvoker.GetDelegateInvoker(converter);
                }

                var nullSubstitutor = _rootOperation.NullSubstitutor;
                if (nullSubstitutor != null)
                {
                    _nullSubstitutor = (DelegateInvokerFunc_0)DelegateInvoker.GetDelegateInvoker(nullSubstitutor);
                }
			}
        }

        protected object ConstructTarget()
        {
            if (_targetConstructor != null)
            {
				return _targetConstructor.CallFunc();
            }
            return CreateTargetInstance();
        }

        #endregion
    }
}