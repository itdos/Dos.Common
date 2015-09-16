using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
	
    /// <summary>
    /// Generates the following code:
    /// var tempSrc = Source.member;
    /// if(tempSrc == null)
    /// {
    ///     Destination.member = null;
    /// }
    /// else
    /// {
    ///     var tempDst = Destination.member;
    ///     if(tempDst == null)
    ///     {
    ///         tempDst = new DestinationMemberType();
    ///     }
    ///     // Operations:
    ///     tempDst.fld1 = tempSrc.fld1;
    ///     tempDst.fld2 = tempSrc.fld2;
    ///     ...
    ///     Destination.member = tempDst;
    /// }
    /// </summary>
	public class ReadWriteComplex : IComplexOperation, IReadWriteOperation
    {
        public bool ShallowCopy { get; set; }
        public MemberDescriptor Source { get; set; }
        public MemberDescriptor Destination { get; set; }
        public Delegate Converter { get; set; }
        public List<IMappingOperation> Operations { get; set; }
        public Delegate NullSubstitutor { get; set; }
        public Delegate TargetConstructor { get; set; }
        public Delegate ValuesPostProcessor { get; set; }

        public override string ToString()
        {
            return "ReadWriteComplex. Source member:" + Source + " Target member:" + Destination.ToString();
        }
    }
}
