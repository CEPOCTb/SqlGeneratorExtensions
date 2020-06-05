using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public class ValueOperation : AssigmentOperationBase
	{
		[NotNull] private readonly Expression _expression;

		public override bool IsValue => true;

		public override object Value => GetValue(_expression);

		public ValueOperation(
			[NotNull] Expression expression,
			[NotNull] string name,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter
			) : base(name, dialect, nameConverter)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			var paramName = filterParams.Add(Value);
			builder.Append(Dialect.GetParameterReference(paramName));
		}
	}
}