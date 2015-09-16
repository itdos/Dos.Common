using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EmitMapper.MappingConfiguration
{
	/// <summary>
	/// Detailed description of a generic converter. 
	/// </summary>
    public class CustomConverterDescriptor
    {
		/// <summary>
		/// Type of class which performs conversion. This class can be generic which will be parameterized with types 
		/// returned from "ConverterClassTypeArguments" property.
		/// </summary>
        public Type ConverterImplementation { get; set; }

		/// <summary>
		/// Name of conversion method of class returned from "ConverterImplementation" property.
		/// </summary>
        public string ConversionMethodName { get; set; }
		
		/// <summary>
		/// Type arguments for parameterizing generic converter determined by "ConverterImplementation" property.
		/// </summary>
        public Type[] ConverterClassTypeArguments { get; set; }
    }
}
