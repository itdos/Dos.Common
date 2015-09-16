using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
	public delegate void ValueSetter(object obj, object value, object state);
	public class SrcReadOperation : ISrcReadOperation
	{
		public ValueSetter Setter { get; set; }
		public MemberDescriptor Source { get; set; }

        public override string ToString()
        {
            return "SrcReadOperation. Source member:" + Source.ToString();
        }
	}
}
