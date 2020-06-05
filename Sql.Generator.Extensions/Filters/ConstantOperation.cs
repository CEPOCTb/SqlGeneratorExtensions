using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Filters;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Filters
{
	public class ConstantOperation : FilterOperationBase
	{
		[NotNull] private readonly ConstantExpression _expression;
		[NotNull] private readonly FilterParseHelpers _parseHelper;

		public override bool IsValue => true;

		public override object Value => _expression.Value;

		public ConstantOperation(
			[NotNull] ConstantExpression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper)
			: base(dialect)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_parseHelper = parseHelper ?? throw new ArgumentNullException(nameof(parseHelper));
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			var value = Value;
			if (_parseHelper.IsSimpleType(value.GetType()))
			{
				builder.Append(value);
			}
			else
			{
				var paramName = filterParams.Add(value);
				builder.Append(Dialect.GetParameterReference(paramName));
			}
		}

	}
}