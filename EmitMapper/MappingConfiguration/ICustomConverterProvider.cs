using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration
{
	/// <summary>
	/// Provider for getting detailed information about generic conversion
	/// </summary>
    public interface ICustomConverterProvider
    {
		/// <summary>
		/// Getting detailed information about generic conversion
		/// </summary>
		/// <param name="from">Type of source. Can be also generic class or abstract array.</param>
		/// <param name="to">Type of destination. Can be also generic class or abstract array.</param>
		/// <param name="mappingConfig">Current mapping configuration</param>
		/// <returns></returns>
        CustomConverterDescriptor GetCustomConverterDescr(Type from, Type to, MapConfigBaseImpl mappingConfig);
    }
}
