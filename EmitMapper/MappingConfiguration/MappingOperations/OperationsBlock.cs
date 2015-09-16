using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration.MappingOperations
{
    public class OperationsBlock: IComplexOperation
    {
        public List<IMappingOperation> Operations { get; set; }
    }
}
