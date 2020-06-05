using System;

namespace Sql.Generator.Extensions.Extensions.Filters
{
	public static class FiltersPseudoMethods
	{
		private static Exception Exception => new InvalidOperationException("This method is not supposed to be called from code directly. It is for using in filter expressions");

		public static bool Like(this string s, string pattern) => throw Exception;
	

	}
}
