using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public class ParameterRefOperation : AssigmentOperationBase
	{
		[NotNull] private readonly MemberExpression _expression;
		[NotNull] private readonly INameConverter _nameConverter;

		public override bool IsValue => false;

		public override object Value => CreateException();

		public ParameterRefOperation(
			[NotNull] MemberExpression expression,
			[NotNull] string name,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter
			) : base(name, dialect, nameConverter)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_nameConverter = nameConverter ?? throw new ArgumentNullException(nameof(nameConverter));
			if (_expression.Expression.NodeType != ExpressionType.Parameter)
			{
				throw new ArgumentException($"Invalid expression type {_expression.Expression.NodeType}", nameof(expression));
			}
		}

		public override void AppendSql(StringBuilder builder, GeneratorContext generatorContext)
		{
			var columnName = _nameConverter.GetColumnName(_expression.Member.Name);
			generatorContext.ColumnsMappings[columnName] = _expression.Member.Name;
			
			builder.Append(
				Dialect.GetEscapedParameterName(
					columnName,
					((ParameterExpression) _expression.Expression).Name
					)
				);
		}
	}
}