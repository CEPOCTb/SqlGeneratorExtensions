using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Interfaces;
using PK.Sql.Generator.Extensions.Extensions;

namespace PK.Sql.Generator.Extensions.Filters
{
	public class NotOperation : FilterOperationBase
	{
		[NotNull] private readonly IFilterOperation _expression;

		public override bool IsValue => false;

		public override object Value => throw CreateException();

		public NotOperation(
			[NotNull] UnaryExpression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper
			) : base(dialect)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			_expression = parseHelper.ParseExpression(expression.Operand.UnwrapConvert(), true);

			if (!(_expression is BinaryOperation || _expression is NotOperation || _expression is PseudoMethodCallOperation))
			{
				_expression = new UnaryToBinaryOperation(
					expression.Operand,
					_expression,
					new SimpleValueOperation(expression.Operand.Type == typeof(bool) ? (object) true : 1, Dialect, parseHelper),
					Dialect,
					true
					);
			}
		}

		public NotOperation([NotNull] IFilterOperation filterExpression, ISqlDialect dialect) : base(dialect)
		{
			_expression = filterExpression ?? throw new ArgumentNullException(nameof(filterExpression));
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			builder.Append(Dialect.NotKeyword + " " + Dialect.OpenBracket);
			_expression.AppendSql(builder, filterParams);
			builder.Append(Dialect.CloseBracket);
		}

	}
}