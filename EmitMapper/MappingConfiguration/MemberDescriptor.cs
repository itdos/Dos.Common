using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using EmitMapper.Utils;

namespace EmitMapper.MappingConfiguration
{
	public class MemberDescriptor
	{
		public MemberDescriptor(MemberInfo singleMember)
		{
			MembersChain = new[] { singleMember };
		}

		public MemberDescriptor(MemberInfo[] membersChain)
		{
			MembersChain = membersChain;
		}

		public MemberInfo[] MembersChain { get; set; }

		public MemberInfo MemberInfo
		{
			get
			{
				return MembersChain == null || MembersChain.Length == 0 ? null : MembersChain[MembersChain.Length - 1];
			}
		}

		public Type MemberType
		{
			get
			{
				return ReflectionUtils.GetMemberType(MemberInfo);
			}
		}

        public override string ToString()
        {
            return "[" + MembersChain.Select(mc => ReflectionUtils.GetMemberType(mc).Name + ":" + mc.Name).ToCSV(",") + "]";
        }
	}
}
