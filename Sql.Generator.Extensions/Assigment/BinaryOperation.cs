using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Assigment;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public class BinaryOperation : AssigmentOperationBase
	{
		[NotNull] private readonly IAssigmentOperation _left;
		[NotNull] private readonly IAssigmentOperation _right;

		[NotNull] private readonly BinaryExpression _expression;
		[NotNull] private readonly Expression _originalExpression;
		private readonly bool _skipBrackets;

		public override bool IsValue => _left.IsValue && _right.IsValue;

		public override object Value => GetValue(_originalExpression);

		public BinaryOperation(
			[NotNull] BinaryExpression expression,
			[NotNull] Expression originalExpression,
			[NotNull] string name,
			[NotNull] ISqlDialect dialect,
			[NotNull] AssigmentParseHelpers assigmentParseHelpers,
			[NotNull] INameConverter nameConverter,
			bool skipBrackets
			) : base(name, dialect, nameConverter)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_originalExpression = originalExpression ?? throw new ArgumentNullException(nameof(originalExpression));
			_skipBrackets = skipBrackets;

			_left = assigmentParseHelpers.ParseExpression(_expression.Left, name);
			_right = assigmentParseHelpers.ParseExpression(_expression.Right, name);
		}

		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			if (IsValue)
			{
				builder.Append(Value);
			}
			else
			{
				if (!_skipBrackets)
				{
					builder.Append(Dialect.OpenBracket);
				}

				_left.AppendSql(builder, filterParams);

				builder.Append(
					_expression.NodeType switch
						{
							ExpressionType.AndAlso => " " + Dialect.LogicalAnd + " ",
							ExpressionType.OrElse => " " + Dialect.LogicalOr + " ",
							ExpressionType.Equal => " " + Dialect.LogicalEquals + " ",
							ExpressionType.NotEqual => " " + Dialect.LogicalNotEquals + " ",
							ExpressionType.GreaterThan => " " + Dialect.LogicalGreater + " ",
							ExpressionType.GreaterThanOrEqual => " " + Dialect.LogicalGreaterEquals + " ",
							ExpressionType.LessThan => " " + Dialect.LogicalLess + " ",
							ExpressionType.LessThanOrEqual => " " + Dialect.LogicalLessEquals + " ",
							ExpressionType.Add => " " + Dialect.PlusSign + " ",
							ExpressionType.AddChecked => " " + Dialect.PlusSign + " ",
							ExpressionType.Subtract => " " + Dialect.MinusSign + " ",
							ExpressionType.SubtractChecked => " " + Dialect.MinusSign + " ",
							ExpressionType.Multiply => " " + Dialect.MultiplySign + " ",
							ExpressionType.MultiplyChecked => " " + Dialect.MultiplySign + " ",
							ExpressionType.Divide => " " + Dialect.DivideSign + " ",
							_ => throw new NotSupportedException($"Binary operation of type {_expression.NodeType} is not supported")
						}
					);

				_right.AppendSql(builder, filterParams);

				if (!_skipBrackets)
				{
					builder.Append(Dialect.CloseBracket);
				}
			}
		}
	}
}