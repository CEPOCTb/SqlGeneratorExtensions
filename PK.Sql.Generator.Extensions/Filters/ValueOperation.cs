using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Filters
{
	public class ValueOperation : FilterOperationBase
	{
		[NotNull] private readonly Expression _expression;
		[NotNull] private readonly FilterParseHelpers _parseHelper;

		public override bool IsValue => true;

		public override object Value => GetValue(_expression);

		public ValueOperation(
			[NotNull] Expression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper
			) : base(dialect)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_parseHelper = parseHelper ?? throw new ArgumentNullException(nameof(parseHelper));
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			var value = Value;
			if (value is null)
			{
				builder.Append("NULL");
			}
			else if (_parseHelper.IsSimpleType(value.GetType()))
			{
				builder.Append(value);
			}
			else
			{
				var paramName = filterParams.Add(Value);
				builder.Append(Dialect.GetParameterReference(paramName));
			}
		}
	}
}
