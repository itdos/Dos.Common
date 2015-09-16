using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitMapper.Utils
{
	static class MiscUtils
	{
		public static string ToCSV<T>(this IEnumerable<T> collection, string delim)
		{
			if (collection == null)
			{
				return "";
			}

			StringBuilder result = new StringBuilder();
			foreach (T value in collection)
			{
				result.Append(value);
				result.Append(delim);
			}
			if (result.Length > 0)
			{
				result.Length -= delim.Length;
			}
			return result.ToString();
		}

	}
}
