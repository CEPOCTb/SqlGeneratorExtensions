using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Filters;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Filters
{
	public class BinaryOperation : FilterOperationBase
	{
		[NotNull] private readonly IFilterOperation _left;
		[NotNull] private readonly IFilterOperation _right;
		[CanBeNull] private readonly IFilterOperation _override;

		[NotNull] private readonly BinaryExpression _expression;
		private readonly bool _skipBrackets;

		public override bool IsValue => _left.IsValue && _right.IsValue;

		public override object Value => GetValue(_expression);

		public BinaryOperation(
			[NotNull] BinaryExpression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper, 
			bool skipBrackets) : base(dialect)
		{
			_expression = expression ?? throw new ArgumentNullException(nameof(expression));
			_skipBrackets = skipBrackets;

			_left = parseHelper.ParseExpression(_expression.Left);
			_right = parseHelper.ParseExpression(_expression.Right);
			if (_expression.NodeType == ExpressionType.AndAlso || _expression.NodeType == ExpressionType.OrElse)
			{
				if (!(_left is BinaryOperation || _left is NotOperation || _left is PseudoMethodCallOperation))
				{
					_left = new UnaryToBinaryOperation(
						_expression.Left,
						_left,
						new SimpleValueOperation(_expression.Left.Type == typeof(bool) ? (object) true : 1, Dialect, parseHelper),
						Dialect,
						false
						);
				}

				if (!(_right is BinaryOperation || _right is NotOperation || _right is PseudoMethodCallOperation))
				{
					_right = new UnaryToBinaryOperation(
						_expression.Right,
						_right,
						new SimpleValueOperation(_expression.Right.Type == typeof(bool) ? (object) true : 1, Dialect, parseHelper),
						Dialect,
						false
						);
				}
			}
			else if (_expression.NodeType == ExpressionType.NotEqual && !Dialect.NotEqualsSupported)
			{
				_override = new NotOperation(
					new UnaryToBinaryOperation(
						_expression,
						_left,
						_right,
						Dialect,
						skipBrackets
						),
					Dialect
					);
			}
		}

		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			if (_override != null)
			{
				_override.AppendSql(builder, filterParams);
			}
			else if (IsValue)
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

				builder.Append(
					_expression.NodeType switch
						{
							ExpressionType.AndAlso => " " + Dialect.LogicalAnd + " ",
							ExpressionType.OrElse => " " + Dialect.LogicalOr + " ",
							ExpressionType.Equal => " " + Dialect.LogicalEquals + " ",
							ExpressionType.NotEqual => " " + Dialect.LogicalNotEquals + " ",
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