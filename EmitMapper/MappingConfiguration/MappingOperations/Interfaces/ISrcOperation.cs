using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
	public interface ISrcOperation : IMappingOperation
	{
		MemberDescriptor Source { get; set; }
	}
}
