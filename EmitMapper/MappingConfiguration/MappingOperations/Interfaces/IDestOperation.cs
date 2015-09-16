using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
	public interface IDestOperation : IMappingOperation
	{
		MemberDescriptor Destination { get; set; }
	}
}
