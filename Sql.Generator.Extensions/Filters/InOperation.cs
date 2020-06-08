using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Filters;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Filters
{
	public class InOperation : PseudoMethodCallOperation
	{
		[CanBeNull] private readonly IFilterOperation _left;
		[CanBeNull] private readonly IEnumerable<IFilterOperation> _values;
		[CanBeNull] private readonly IFilterOperation _emptyEnumOperation;
		
		public override bool IsValue => false;

		public override object Value => throw CreateException();
		
		/// <inheritdoc />
		public InOperation(
			[NotNull] Expression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper,
			bool skipBrackets
			) : base(expression, dialect, skipBrackets)
		{
			var right = (CallExpression.Method.IsStatic
				? CallExpression.Arguments[0]
				: CallExpression.Object) ?? throw new ArgumentException("Null object for Contains method");

			var enumerable = Expression.Lambda(right).Compile().DynamicInvoke() as IEnumerable;
			var values = (enumerable ?? throw new InvalidOperationException("Expression was not evaluated to IEnumerable!"))
				.Cast<object>()
				.Select(val => new SimpleValueOperation(val, dialect, parseHelper))
				.ToList();
			
			if (values.Count > 0)
			{
				_left = (CallExpression.Method.IsStatic
					? parseHelper.ParseExpression(CallExpression.Arguments[1])
					: parseHelper.ParseExpression(CallExpression.Arguments[0])) ?? throw new ArgumentException("Null parameter for Contains method");

				_values = values;
			}
			else
			{
				_emptyEnumOperation = new SimpleBoolOperation(new SimpleValueOperation(true, Dialect, parseHelper), dialect);
			}
		}

		/// <inheritdoc />
		public override void AppendSql(StringBuilder builder, GeneratorContext filterParams)
		{
			if (!SkipBrackets)
			{
				builder.Append(Dialect.OpenBracket);
			}

			if (_emptyEnumOperation != null)
			{
				_emptyEnumOperation.AppendSql(builder, filterParams);
			}
			else
			{
				// ReSharper disable once PossibleNullReferenceException
				_left.AppendSql(builder, filterParams);

				builder.Append(" " + Dialect.InKeyword + " ");

				builder.Append(Dialect.OpenBracket);

				// ReSharper disable once PossibleNullReferenceException
				using var enumerator = _values.GetEnumerator();
				IFilterOperation first = null;
				while (enumerator.MoveNext())
				{
					var current = enumerator.Current;
					
					if (first != null && current != null)
					{
						builder.Append(", ");
					}
					else
					{
						first = enumerator.Current;
					}

					current?.AppendSql(builder, filterParams);
				}

				builder.Append(Dialect.CloseBracket);
			}

			if (!SkipBrackets)
			{
				builder.Append(Dialect.CloseBracket);
			}
		}
	}
}
