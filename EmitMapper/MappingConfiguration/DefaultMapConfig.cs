using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using EmitMapper.Utils;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Mappers;

namespace EmitMapper.MappingConfiguration
{
	public class DefaultMapConfig : MapConfigBase<DefaultMapConfig>
	{
		class TypesPair
		{
			public Type t1;
			public Type t2;
			public TypesPair(Type t1, Type t2)
			{
				this.t1 = t1;
				this.t2 = t2;
			}

			public override bool Equals(object obj)
			{
				var rhs = (TypesPair)obj;
				return t1 == rhs.t1 && t2 == rhs.t2;
			}
			public override int GetHashCode()
			{
				return t1.GetHashCode() + t2.GetHashCode();
			}
		}

        class MappingItem
        {
			public bool ShallowCopy;
            public MemberDescriptor From;
            public MemberDescriptor To;
			public MappingItem(MemberDescriptor from, MemberDescriptor to, bool shallowCopy)
            {
                From = from;
                To = to;
				ShallowCopy = shallowCopy;
            }
        }
        private bool _shallowCopy;
        private Func<string, string, bool> _membersMatcher;
        private List<string> _shallowCopyMembers = new List<string>();
        private List<string> _deepCopyMembers = new List<string>();

		#region Constructors

		static DefaultMapConfig()
		{
			_instance = new DefaultMapConfig();
		}

		public DefaultMapConfig()
		{
			Init(this);
			_shallowCopy = true;
            _membersMatcher = (m1, m2) => m1 == m2;
        }
        #endregion

		private static DefaultMapConfig _instance;
		public static DefaultMapConfig Instance
		{
			get
			{
				return _instance;
			}
		}

        #region Configuration methods


		/// <summary>
		/// Define shallow map mode for the specified type. In that case all members of this type will be copied by reference if it is possible
		/// </summary>
		/// <typeparam name="T">Type for which shallow map mode is defining</typeparam>
		/// <returns></returns>
		public DefaultMapConfig ShallowMap<T>()
		{
			return ShallowMap(typeof(T));
		}

		/// <summary>
		/// Define shallow map mode for the specified type. In that case all members of this type will be copied by reference if it is possible
		/// </summary>
		/// <param name="type">Type for which shallow map mode is defining</param>
		/// <returns></returns>
		public DefaultMapConfig ShallowMap(Type type)
		{
			_shallowCopyMembers.Add(type.FullName);
			return this;
		}

		/// <summary>
		/// Define default shallow map mode. In that case all members will be copied by reference (if it is possible) by default.
		/// </summary>
		/// <returns></returns>
		public DefaultMapConfig ShallowMap()
		{
			_shallowCopy = true;
			return this;
		}

		/// <summary>
		/// Define deep map mode for the specified type. In that case all members of this type will be copied by value (new instances will be created)
		/// </summary>
		/// <typeparam name="T">Type for which deep map mode is defining</typeparam>
		/// <returns></returns>
		public DefaultMapConfig DeepMap<T>()
		{
			return DeepMap(typeof(T));
		}

		/// <summary>
		/// Define deep map mode for the specified type. In that case all members of this type will be copied by value (new instances will be created)
		/// </summary>
		/// <param name="type">Type for which deep map mode is defining</param>
		/// <returns></returns>
		public DefaultMapConfig DeepMap(Type type)
		{
			_deepCopyMembers.Add(type.FullName);
			return this;
		}

		/// <summary>
		/// Define default deep map mode. In that case all members will be copied by value (new instances will be created) by default
		/// </summary>
		/// <returns></returns>
		public DefaultMapConfig DeepMap()
		{
			_shallowCopy = false;
			return this;
		}

		/// <summary>
		/// Define a function to test two members if they have identical names.
		/// </summary>
		/// <param name="membersMatcher">Function to test two members if they have identical names. For example if you want to match members ignoring case you can define the following function: (m1, m2) => m1.ToUpper() == m2.ToUpper()</param>
		/// <returns></returns>
		public DefaultMapConfig MatchMembers(Func<string, string, bool> membersMatcher)
		{
			this._membersMatcher = membersMatcher;
			return this;
		}

        #endregion

        #region IMappingConfigurator Members

        public override IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			return FilterOperations(from, to, GetMappingItems(new HashSet<TypesPair>(), from, to, null, null)).ToArray();
		}

		public override IRootMappingOperation GetRootMappingOperation(Type from, Type to)
		{
			var res = base.GetRootMappingOperation(from, to);
			res.ShallowCopy = IsShallowCopy(from, to);
			return res;
		}

		public override string GetConfigurationName()
		{
			var configName = base.GetConfigurationName() +
				new[] 
				{ 
					_shallowCopy.ToString(),
					ToStr(_membersMatcher),
					ToStrEnum(_shallowCopyMembers),
					ToStrEnum(_deepCopyMembers)
				}.ToCSV(";");

			return configName;
		}

		#endregion

		#region protected members
		protected virtual bool MatchMembers(string m1, string m2)
        {
            return _membersMatcher(m1, m2);
		}
		#endregion

		#region private util method

		private bool MappingItemNameInList(IEnumerable<string> list, ReadWriteSimple mo)
        {
            return
                list.Any(l => MatchMembers(l, mo.Destination.MemberInfo.Name)) ||
                list.Any(l => MatchMembers(l, mo.Source.MemberInfo.Name));
        }

		private bool TypeInList(IEnumerable<string> list, Type t)
        {
            return list.Any(l => MatchMembers(l, t.FullName));
        }


		private bool MappingItemTypeInList(IEnumerable<string> list, ReadWriteSimple mo)
        {
            return
                TypeInList(list, mo.Destination.MemberType) ||
                TypeInList(list, mo.Source.MemberType);
        }

        private List<IMappingOperation> GetMappingItems(
			HashSet<TypesPair> processedTypes,
            Type fromRoot,
            Type toRoot,
            IEnumerable<MemberInfo> toPath, 
            IEnumerable<MemberInfo> fromPath
        )
        {
			if (toPath == null)
			{
				toPath = new MemberInfo[0];
			}
			if (fromPath == null)
			{
				fromPath = new MemberInfo[0];
			}

            Type from, to;
            if (fromPath.Count() == 0)
            {
                from = fromRoot;
            }
            else
            {
                from = ReflectionUtils.GetMemberType(fromPath.Last());
            }

			if (toPath.Count() == 0)
			{
				to = toRoot;
			}
			else
			{
				to = ReflectionUtils.GetMemberType(toPath.Last());
			}

			var tp = new TypesPair(from, to);
			processedTypes.Add(tp);

			MemberInfo[] toMembers = ReflectionUtils.GetPublicFieldsAndProperties(to);
			MemberInfo[] fromMembers = ReflectionUtils.GetPublicFieldsAndProperties(from);

            var result = new List<IMappingOperation>();

			foreach (MemberInfo toMi in toMembers)
			{
				if (toMi.MemberType == MemberTypes.Property)
				{
					var setMethod = ((PropertyInfo)toMi).GetSetMethod();
					if (setMethod == null || setMethod.GetParameters().Length != 1)
					{
						continue;
					}
				}

				MemberInfo fromMi = fromMembers.FirstOrDefault(mi => MatchMembers(mi.Name, toMi.Name));
				if (fromMi == null)
				{
					continue;
				}

				if (fromMi.MemberType == MemberTypes.Property)
				{
					var getMethod = ((PropertyInfo)fromMi).GetGetMethod();
					if (getMethod == null)
					{
						continue;
					}
				}

				var op = CreateMappingOperation(processedTypes, fromRoot, toRoot, toPath, fromPath, fromMi, toMi);
				if (op != null)
				{
					result.Add(op);
				}
            }
			processedTypes.Remove(tp);
            return result;
        }

		private IMappingOperation CreateMappingOperation(
			HashSet<TypesPair> processedTypes,
            Type fromRoot,
            Type toRoot,
            IEnumerable<MemberInfo> toPath, 
            IEnumerable<MemberInfo> fromPath,
			MemberInfo fromMi,
			MemberInfo toMi
			)
		{
			var origDestMemberDescr = new MemberDescriptor(toPath.Concat(new[] { toMi }).ToArray());
			var origSrcMemberDescr = new MemberDescriptor(fromPath.Concat(new[] { fromMi }).ToArray());

			if (ReflectionUtils.IsNullable(ReflectionUtils.GetMemberType(fromMi)))
			{
				fromPath = fromPath.Concat(new[] { fromMi });
				fromMi = ReflectionUtils.GetMemberType(fromMi).GetProperty("Value");
			}
			if (ReflectionUtils.IsNullable(ReflectionUtils.GetMemberType(toMi)))
			{
				toPath = fromPath.Concat(new[] { toMi });
				toMi = ReflectionUtils.GetMemberType(toMi).GetProperty("Value");
			}

			var destMemberDescr = new MemberDescriptor(toPath.Concat(new[] { toMi }).ToArray());
			var srcMemberDescr = new MemberDescriptor(fromPath.Concat(new[] { fromMi }).ToArray());
			Type typeFromMember = srcMemberDescr.MemberType;
			Type typeToMember = destMemberDescr.MemberType;

			bool shallowCopy = IsShallowCopy(srcMemberDescr, destMemberDescr);

			if (
				IsNativeDeepCopy(typeFromMember, typeToMember, srcMemberDescr.MemberInfo, destMemberDescr.MemberInfo, shallowCopy)
				&& !processedTypes.Contains(new TypesPair(typeFromMember, typeToMember))
				)
			{
                return
                    new ReadWriteComplex()
                    {
                        Destination = origDestMemberDescr,
                        Source = origSrcMemberDescr,
                        ShallowCopy = shallowCopy,
                        Operations =
                            GetMappingItems(
								processedTypes,
                                srcMemberDescr.MemberType,
                                destMemberDescr.MemberType,
                                null,
                                null
                            )
                    };
			}
			else
			{
				return
					new ReadWriteSimple()
					{
						Source = origSrcMemberDescr,
						Destination = origDestMemberDescr,
						ShallowCopy = shallowCopy,
					};
			}
		}

		private bool IsShallowCopy(Type from, Type to)
		{
			if (TypeInList(_shallowCopyMembers, to) || TypeInList(_shallowCopyMembers, from))
			{
				return true;
			}
			else if (TypeInList(_deepCopyMembers, to) || TypeInList(_deepCopyMembers, from))
			{
				return false;
			}
			else
			{
				return _shallowCopy;
			}
		}

		private bool IsShallowCopy(MemberDescriptor from, MemberDescriptor to)
		{
			return IsShallowCopy(from.MemberType, to.MemberType);
		}

        private bool IsNativeDeepCopy(Type typeFrom, Type typeTo, MemberInfo fromMi, MemberInfo toMi, bool shallowCopy)
        {
            if (NativeConverter.IsNativeConvertionPossible(typeFrom, typeTo))
            {
                return false;
            }

            if (MapperForCollectionImpl.IsSupportedType(typeFrom) || MapperForCollectionImpl.IsSupportedType(typeTo))
            {
                return false;
            }

            if((typeTo != typeFrom || !shallowCopy))
            {
                return true;
            }
            
            return false;
        }

        #endregion
	}
}
