using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Filters
{
	public class UnaryToBinaryOperation : FilterOperationBase
	{
		[NotNull] private readonly IFilterOperation _left;
		[NotNull] private readonly IFilterOperation _right;

		[NotNull] private readonly Expression _expression;
		private readonly bool _skipBrackets;

		public override bool IsValue => _left.IsValue && _right.IsValue;

		public override object Value => GetValue(_expression);

		public UnaryToBinaryOperation(
			[NotNull] Expression expression,
			[NotNull] IFilterOperation left,
			[NotNull] IFilterOperation right,
			ISqlDialect dialect,
			bool skipBrackets) : base(dialect)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_left = left ?? throw new ArgumentNullException(nameof(left));
			_right = right ?? throw new ArgumentNullException(nameof(right));
			_skipBrackets = skipBrackets;
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			if (IsValue)
			{
				var paramName = filterParams.Add(Value);
				builder.Append(Dialect.GetParameterReference(paramName));
			}
			else
			{
				if (!_skipBrackets)
				{
					builder.Append(Dialect.OpenBracket);
				}

				_left.AppendSql(builder, filterParams);

				builder.Append(" = ");

				_right.AppendSql(builder, filterParams);

				if (!_skipBrackets)
				{
					builder.Append(Dialect.CloseBracket);
				}
			}
		}
	}
}