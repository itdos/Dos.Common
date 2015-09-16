using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EmitMapper.MappingConfiguration
{
	public class CustomMapConfig : IMappingConfigurator
	{
		public Func<Type, Type, IMappingOperation[]> GetMappingOperationFunc { get; set; }
		public string ConfigurationName { get; set; }

		#region IMappingConfigurator Members

		public IMappingOperation[] GetMappingOperations(Type from, Type to)
		{
			if (GetMappingOperationFunc == null)
			{
				return new IMappingOperation[0];
			}
			return GetMappingOperationFunc(from, to);
		}

		public string GetConfigurationName()
		{
			return ConfigurationName;
		}

		public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
		{
			return null;
		}


		#endregion


		#region IMappingConfigurator Members


		public StaticConvertersManager GetStaticConvertersManager()
		{
			return null;
		}

		#endregion
	}
}
