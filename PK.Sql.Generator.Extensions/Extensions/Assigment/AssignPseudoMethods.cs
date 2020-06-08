using System;

namespace PK.Sql.Generator.Extensions.Extensions.Assigment
{
	public static class AssignPseudoMethods
	{
		private static Exception Exception => new InvalidOperationException("This method is not supposed to be called from code directly. It is for using in filter expressions");

		public static T Column<T>(this T obj, string columnName, object val) => throw Exception;
	}
}
