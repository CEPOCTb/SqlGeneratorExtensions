using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Assigment
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
			var value = Value;
			if (value != null)
			{
				var paramName = filterParams.Add(value);
				builder.Append(Dialect.GetParameterReference(paramName));
			}
			else
			{
				builder.Append("NULL");
			}
		}
	}
}
