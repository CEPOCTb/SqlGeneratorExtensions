using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Assigment;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public class CustomColumnOperation : AssigmentOperationBase
	{
		[NotNull] private readonly IAssigmentOperation _operation;

		#region Overrides of AssigmentOperationBase

		/// <inheritdoc />
		public override bool IsValue => _operation.IsValue;

		/// <inheritdoc />
		public override object Value => _operation.Value;
		

		#endregion

		/// <inheritdoc />
		public CustomColumnOperation(
			[NotNull] MethodCallExpression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter,
			[NotNull] AssigmentParseHelpers assigmentParseHelpers
			) : base(null, dialect, nameConverter)
		{
			if (expression == null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			Name = expression.Arguments[1].NodeType == ExpressionType.Constant
				? ((ConstantExpression) expression.Arguments[1]).Value as string
				: throw new InvalidOperationException();
			_operation = assigmentParseHelpers.ParseExpression(expression.Arguments[2], Name, true);
		}

		/// <inheritdoc />
		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			_operation.AppendSql(builder, filterParams);
		}

	}
}