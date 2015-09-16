using EmitMapper.Mappers;
using System.Collections.Generic;

namespace EmitMapper
{
    public class ObjectsMapper<TFrom, TTo>
    {
        public TTo Map(TFrom from, TTo to, object state)
        {
			return (TTo)MapperImpl.Map(from, to, state);
        }

		public TTo Map(TFrom from, TTo to)
		{
			return (TTo)MapperImpl.Map(from, to, null);
		}

        public TTo Map(TFrom from)
        {
            return (TTo)MapperImpl.Map(from);
        }

		public TTo MapUsingState(TFrom from, object state)
		{
			return (TTo)MapperImpl.Map(from, null, state);
		}


        public ObjectsMapper(ObjectsMapperBaseImpl mapperImpl)
        {
            MapperImpl = mapperImpl;
        }

		public IEnumerable<TTo> MapEnum(IEnumerable<TFrom> sourceCollection)
		{
			foreach (TFrom src in sourceCollection)
			{
				yield return Map(src);
			}
		}

        public ObjectsMapperBaseImpl MapperImpl { get; set; }
    }
}