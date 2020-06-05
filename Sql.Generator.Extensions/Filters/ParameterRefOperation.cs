using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Filters
{
	public class ParameterRefOperation : FilterOperationBase
	{
		[NotNull] private readonly MemberExpression _expression;
		[NotNull] private readonly INameConverter _nameConverter;

		public override bool IsValue => false;

		public override object Value => CreateException();

		public ParameterRefOperation(
			[NotNull] MemberExpression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter
			) : base(dialect)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_nameConverter = nameConverter ?? throw new ArgumentNullException(nameof(nameConverter));
			if (_expression.Expression.NodeType != ExpressionType.Parameter)
			{
				throw new ArgumentException($"Invalid expression type {_expression.Expression.NodeType}", nameof(expression));
			}
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			builder.Append(
				Dialect.GetEscapedParameterName(
					_nameConverter.GetColumnName(_expression.Member.Name),
					((ParameterExpression) _expression.Expression).Name
					)
				);
		}
	}
}