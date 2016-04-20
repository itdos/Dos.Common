using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper.AST;
using EmitMapper.AST.Helpers;
using EmitMapper.AST.Interfaces;
using EmitMapper.AST.Nodes;

namespace EmitMapper.Mappers
{
    /// <summary>
    /// Mapper for collections. It can copy Array, List<>, ArrayList collections. 
    /// Collection type in source object and destination object can differ.
    /// </summary>
    internal class MapperForCollectionImpl : CustomMapperImpl
    {
		private ObjectsMapperDescr subMapper;

        /// <summary>
        /// Copies object properties and members of "from" to object "to"
        /// </summary>
        /// <Param name="from">Source object</Param>
        /// <Param name="to">Destination object</Param>
        /// <returns>Destination object</returns>
        internal override object MapImpl(object from, object to, object state)
        {
			if (to == null)
			{
				if (_targetConstructor != null)
				{
                    to = _targetConstructor.CallFunc();
				}
			}

            if (typeTo.IsArray)
            {
                if (from is IEnumerable)
                {
                    return CopyToArray((IEnumerable)from);
                }
                else 
                {
                    return CopyScalarToArray(from);
                }
            }
            else if (typeTo.IsGenericType && typeTo.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (from is IEnumerable)
                {
                    return CopyToListInvoke((IEnumerable)from);
                }
                else
                {
                    return CopyToListScalarInvoke(from);
                }
            }
            else if (typeTo == typeof(ArrayList))
            {
                if (from is IEnumerable)
                {
                    return CopyToArrayList((IEnumerable)from);
                }
                else
                {
                    return CopyToArrayListScalar(from);
                }

            }
			else if (typeof(IList).IsAssignableFrom(typeTo))
			{
				return CopyToIList((IList)to, from);
			}
            return null;
        }

		private object CopyToIList(IList iList, object from)
		{
			if (iList == null)
			{
				iList = (IList)Activator.CreateInstance(typeTo);
			}
			foreach (object obj in (from is IEnumerable ? (IEnumerable)from : new[]{from}))
			{
				if (obj == null)
				{
					iList.Add(null);
				}
				if (_rootOperation == null || _rootOperation.ShallowCopy)
				{
					iList.Add(obj);
				}
				else
				{
					ObjectsMapperBaseImpl Mapper = mapperMannager.GetMapperImpl(obj.GetType(), obj.GetType(), _mappingConfigurator);
					iList.Add(Mapper.Map(obj));
				}
			}
			return iList;
		}

        /// <summary>
        /// Copies object properties and members of "from" to object "to"
        /// </summary>
        /// <Param name="from">Source object</Param>
        /// <Param name="to">Destination object</Param>
        /// <returns>Destination object</returns>
        public override object Map(object from, object to, object state)
        {
			return base.Map(from, null, state);
        }

        /// <summary>
        /// Returns true if specified type is supported by this Mapper
        /// </summary>
        /// <Param name="type"></Param>
        /// <returns></returns>
        internal static bool IsSupportedType(Type type)
        {
            return 
                type.IsArray ||
                type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) ||
                type == typeof(ArrayList) ||
				typeof(IList).IsAssignableFrom(type) ||
				typeof(IList<>).IsAssignableFrom(type)
                ;
			
        }

        /// <summary>
        /// Creates an instance of Mapper for collections.
        /// </summary>
        /// <Param name="MapperName">Mapper name. It is used for registration in Mappers repositories.</Param>
        /// <Param name="mapperMannager">Mappers manager</Param>
        /// <Param name="TypeFrom">Source type</Param>
        /// <Param name="TypeTo">Destination type</Param>
        /// <Param name="SubMapper"></Param>
        /// <returns></returns>
        public static MapperForCollectionImpl CreateInstance(
            string MapperName,
            ObjectMapperManager mapperMannager,
            Type TypeFrom,
            Type TypeTo,
			ObjectsMapperDescr SubMapper,
            IMappingConfigurator mappingConfigurator
            )
        {
            TypeBuilder tb = DynamicAssemblyManager.DefineType(
                "GenericListInv_" + MapperName,
                typeof(MapperForCollectionImpl)
                );

			if (TypeTo.IsGenericType && TypeTo.GetGenericTypeDefinition() == typeof(List<>))
			{
				MethodBuilder methodBuilder = tb.DefineMethod(
					"CopyToListInvoke",
					MethodAttributes.Family | MethodAttributes.Virtual,
					typeof(object),
					new Type[] { typeof(IEnumerable) }
					);

				InvokeCopyImpl(TypeTo, "CopyToList").Compile(new CompilationContext(methodBuilder.GetILGenerator()));

				methodBuilder = tb.DefineMethod(
					"CopyToListScalarInvoke",
					MethodAttributes.Family | MethodAttributes.Virtual,
					typeof(object),
					new Type[] { typeof(object) }
					);

				InvokeCopyImpl(TypeTo, "CopyToListScalar").Compile(
					new CompilationContext(methodBuilder.GetILGenerator())
					);
			}

            MapperForCollectionImpl result = (MapperForCollectionImpl)Activator.CreateInstance(tb.CreateType());
			result.Initialize(mapperMannager, TypeFrom, TypeTo, mappingConfigurator, null);
            result.subMapper = SubMapper;
			
            return result;
        }

        private static IAstNode InvokeCopyImpl(Type copiedObjectType, string copyMethodName)
        {
            var mi = typeof(MapperForCollectionImpl).GetMethod(
               copyMethodName,
               BindingFlags.Instance | BindingFlags.NonPublic
            ).MakeGenericMethod(new Type[] { ExtractElementType(copiedObjectType) });

            return new AstReturn()
                       {
                           returnType = typeof(object),
                           returnValue = AstBuildHelper.CallMethod(
                               mi,
                               AstBuildHelper.ReadThis(typeof(MapperForCollectionImpl)),
                               new List<IAstStackItem>
                                   {
                                       new AstReadArgumentRef(){argumentIndex = 1, argumentType = typeof(object)}
                                   }
                               )
                       };
        }

        private static Type ExtractElementType(Type collection)
        {
            if (collection.IsArray)
            {
                return collection.GetElementType();
            }
            if (collection == typeof(ArrayList))
            {
                return typeof(object);
            }
            if (collection.IsGenericType && collection.GetGenericTypeDefinition() == typeof(List<>))
            {
                return collection.GetGenericArguments()[0];
            }
            return null;

        }

        internal static Type GetSubMapperTypeTo(Type to)
        {
            return ExtractElementType(to);
        }

        internal static Type GetSubMapperTypeFrom(Type from) 
        {
            Type result = ExtractElementType(from);
            if (result == null)
            {
                return from;
            }

            return result;
        }

        internal override object CreateTargetInstance()
        {
            return null;
        }
        
        protected MapperForCollectionImpl() :base(null, null, null, null, null)
        {
        }

        private Array CopyToArray(IEnumerable from)
        {
            if (from is ICollection)
            {
                Array result = Array.CreateInstance(typeTo.GetElementType(), ((ICollection)from).Count);
                int idx = 0;
                foreach (object obj in from)
                {
                    result.SetValue(subMapper.mapper.Map(obj), idx++);
                }
                return result;

            }
            else
            {
                ArrayList result = new ArrayList();
                foreach (object obj in from)
                {
                    result.Add(obj);
                }
                return result.ToArray(typeTo.GetElementType());
            }
        }

        private ArrayList CopyToArrayList(IEnumerable from)
        {
            if (ShallowCopy)
            {
                if (from is ICollection)
                {
                    return new ArrayList((ICollection)from);
                }
                else
                {
                    ArrayList res = new ArrayList();
                    foreach (object obj in from)
                    {
                        res.Add(obj);
                    }
                    return res;
                }				
            }

            ArrayList result = new ArrayList();
            if (from is ICollection)
            {
                result = new ArrayList(((ICollection)from).Count);
            }
            else
            {
                result = new ArrayList();
            }

            foreach (object obj in from)
            {
                if(obj == null)
                {
                    result.Add(null);
                }
                else
                {
                    ObjectsMapperBaseImpl Mapper = mapperMannager.GetMapperImpl(obj.GetType(), obj.GetType(), _mappingConfigurator);
                    result.Add(Mapper.Map(obj));
                }
            }
            return result;
        }

        private ArrayList CopyToArrayListScalar(object from)
        {
            ArrayList result = new ArrayList(1);
            if (ShallowCopy)
            {
                result.Add(from);
                return result;
            }
            ObjectsMapperBaseImpl Mapper = mapperMannager.GetMapperImpl(from.GetType(), from.GetType(), _mappingConfigurator);
            result.Add(Mapper.Map(from));
            return result;
        }

        protected List<T> CopyToList<T>(IEnumerable from)
        {
            List<T> result;
            if (from is ICollection)
            {
                result = new List<T>(((ICollection)from).Count);
            }
            else
            {
                result = new List<T>();
            }
            foreach (object obj in from)
            {
                result.Add((T)subMapper.mapper.Map(obj));
            }
            return result;
        }
        protected virtual object CopyToListInvoke(IEnumerable from)
        {
            return null;
        }

        protected List<T> CopyToListScalar<T>(object from)
        {
            List<T> result = new List<T>(1);
            result.Add((T)subMapper.mapper.Map(from));
            return result;
        }
        protected virtual object CopyToListScalarInvoke(object from)
        {
            return null;
        }

        private Array CopyScalarToArray(object scalar)
        {
            Array result = Array.CreateInstance(typeTo.GetElementType(), 1);
            result.SetValue(subMapper.mapper.Map(scalar), 0);
            return result;
        }
    }
}