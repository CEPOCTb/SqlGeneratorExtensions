using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions;
using Sql.Generator.Extensions.Extensions.Assigment;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public class NotOperation : AssigmentOperationBase
	{
		[NotNull] private readonly IAssigmentOperation _operation;

		public override bool IsValue => false;

		public override object Value => CreateException();

		public NotOperation(
			[NotNull] UnaryExpression expression,
			[NotNull] string name,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter,
			[NotNull] AssigmentParseHelpers assigmentParseHelpers
			) : base(name, dialect, nameConverter)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			_operation = assigmentParseHelpers.ParseExpression(expression.Operand.UnwrapConvert(), name, true);
		}

		public NotOperation(
			[NotNull] IAssigmentOperation assigmentOperation,
			[NotNull] string name,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter
			) : base(name, dialect, nameConverter)
		{
			_operation = assigmentOperation ?? throw new ArgumentNullException(nameof(assigmentOperation));
		}

		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			builder.Append(Dialect.NotKeyword + " " + Dialect.OpenBracket);
			_operation.AppendSql(builder, filterParams);
			builder.Append(Dialect.CloseBracket);
		}
	}
}