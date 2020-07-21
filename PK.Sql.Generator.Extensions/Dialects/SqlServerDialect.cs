using System;
using System.Linq.Expressions;
using PK.Sql.Generator.Extensions.Extensions.Assigment;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Models;

namespace PK.Sql.Generator.Extensions.Dialects
{
	public class SqlServerDialect : BaseDialect
	{
		#region Overrides of BaseDialect

		/// <inheritdoc />
		public override string NameEscapeOpen => "[";

		/// <inheritdoc />
		public override string NameEscapeClose => "]";

		#endregion
	}
}
