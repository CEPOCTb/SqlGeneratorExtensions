using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Assigment;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public class ConstantOperation : AssigmentOperationBase
	{
		[NotNull] private readonly ConstantExpression _expression;
		[NotNull] private readonly AssigmentParseHelpers _assigmentParseHelpers;

		public override bool IsValue => true;

		public override object Value => _expression.Value;

		public ConstantOperation(
			[NotNull] ConstantExpression expression,
			[NotNull] string name,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter,
			[NotNull] AssigmentParseHelpers assigmentParseHelpers
			) : base(name, dialect, nameConverter)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_assigmentParseHelpers = assigmentParseHelpers ?? throw new ArgumentNullException(nameof(assigmentParseHelpers));
		}

		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			var value = Value;
			if (_assigmentParseHelpers.IsSimpleType(value.GetType()))
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