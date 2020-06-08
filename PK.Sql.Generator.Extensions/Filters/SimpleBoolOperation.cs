using System;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Filters
{
	public class SimpleBoolOperation : FilterOperationBase
	{
		private readonly IFilterOperation _operation;

		#region Overrides of FilterExpressionBase

		/// <inheritdoc />
		public override bool IsValue => false;

		/// <inheritdoc />
		public override object Value => throw CreateException();

		/// <inheritdoc />
		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			if ((bool) _operation.Value)
			{
				builder.Append("1 " + Dialect.LogicalEquals + " 1");
			}
			else
			{
				builder.Append("1 " + Dialect.LogicalEquals + " 0");
			}
		}

		#endregion

		/// <inheritdoc />
		public SimpleBoolOperation([NotNull] IFilterOperation operation, [NotNull] ISqlDialect dialect) : base(dialect)
		{
			_operation = operation ?? throw new ArgumentNullException(nameof(operation));
		}
	}
}
