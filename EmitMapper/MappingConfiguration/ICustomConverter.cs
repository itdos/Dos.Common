using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.MappingConfiguration
{
    public interface ICustomConverter
    {
        void Initialize(Type from, Type to, MapConfigBaseImpl mappingConfig);
    }
}
