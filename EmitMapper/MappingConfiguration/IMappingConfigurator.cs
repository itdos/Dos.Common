using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitMapper.MappingConfiguration.MappingOperations;

namespace EmitMapper
{
	public interface IMappingConfigurator
	{
        /// <summary>
        /// Get list of mapping operations. Each mapping mapping defines one copieng operation from source to destination. For this operation can be additionally defined the following custom operations: 
        /// - Custom getter which extracts values from source
        /// - Custom values converter which converts extracted from source value
        /// - Custom setter which writes value to destination
        /// </summary>
        /// <Param name="from">Source type</Param>
        /// <Param name="to">Destination type</Param>
        /// <returns></returns>
		IMappingOperation[] GetMappingOperations(Type from, Type to);

		IRootMappingOperation GetRootMappingOperation(Type from, Type to);

        /// <summary>
        /// Get unique configuration name to force Emit Mapper create new mapper instead using appropriate cached one.
        /// </summary>
        /// <returns></returns>
		string GetConfigurationName();

		StaticConvertersManager GetStaticConvertersManager();
	}
}
