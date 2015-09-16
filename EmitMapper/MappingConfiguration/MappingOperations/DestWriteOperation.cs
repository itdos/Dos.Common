using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
	public struct ValueToWrite<T>
	{
		public enum Actions : int
		{
			write = 0,
			skip = 1
		}

		public T value;
		public Actions action;

		public static ValueToWrite<T> ReturnValue(T value)
		{
			return new ValueToWrite<T> { action = Actions.write, value = value };
		}
		public static ValueToWrite<T> Skip()
		{
			return new ValueToWrite<T> { action = Actions.skip };
		}
	}

	public delegate ValueToWrite<T> ValueGetter<T>(object value, object state);

	public class DestWriteOperation : IDestWriteOperation
	{
		public Delegate Getter { get; set; }
		public MemberDescriptor Destination { get; set; }

        public override string ToString()
        {
            return "DestWriteOperation. Target member:" + Destination.ToString();
        }
	}
}
