using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Filters;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Extensions.Filters
{
	public class FilterParseHelpers
	{
		private static readonly HashSet<Type> SimpleTypes = new HashSet<Type>
			{
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(short),
				typeof(ushort),
				typeof(byte),
				typeof(sbyte)
			};

		private readonly Dictionary<MethodInfo, Func<MethodCallExpression, ISqlDialect, FilterParseHelpers, bool, IFilterOperation>> _dictionaryPseudoMethods =
			new Dictionary<MethodInfo, Func<MethodCallExpression, ISqlDialect, FilterParseHelpers, bool, IFilterOperation>>()
				{
					[typeof(FiltersPseudoMethods).GetMethod(nameof(FiltersPseudoMethods.Like))!] =
						(expression, dialect, parseHelper, skipBraces) => new LikeOperation(expression, dialect, parseHelper, skipBraces),
				};


		[NotNull] private readonly ISqlDialect _dialect;
		[NotNull] private readonly INameConverter _nameConverter;
		
		public FilterParseHelpers(
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter,
			IEnumerable<
				KeyValuePair<
					MethodInfo,
					Func<
						MethodCallExpression,
						ISqlDialect,
						FilterParseHelpers,
						bool,
						IFilterOperation>
					>
				> customHandlers = null
			)
		{
			_dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
			_nameConverter = nameConverter ?? throw new ArgumentNullException(nameof(nameConverter));
			
			if (customHandlers != null)
			{
				foreach (var pair in customHandlers)
				{
					_dictionaryPseudoMethods.Add(pair.Key, pair.Value);
				}
			}
		}

		public bool IsSimpleType(Type type)
		{
			return SimpleTypes.Contains(type);
		}

		public string GetTableAlias(LambdaExpression filterExpression)
		{
			return filterExpression?.Parameters?.First()?.Name;
		}
		
		public IFilterOperation ParseFilter(Expression expression)
		{
			var exp = ParseExpression(expression, true);
			if (!(exp is BinaryOperation || exp is NotOperation || exp is PseudoMethodCallOperation))
			{
				if (expression.Type == typeof(bool) && exp.IsValue)
				{
					exp = new SimpleBoolOperation(exp, _dialect);
				}
				else
				{
					exp = new UnaryToBinaryOperation(
						expression,
						exp,
						exp,
						_dialect,
						true
						);
				}
			}

			return exp;
		}

		public IFilterOperation ParseExpression(Expression expression, bool skipBrackets = false)
		{
			var exp = expression.UnwrapConvert();
			if (exp is BinaryExpression bin)
			{
				return new BinaryOperation(bin, _dialect, this, skipBrackets);
			}
			else
			{
				return ParseUnaryExpression(exp, skipBrackets);
			}
		}

		private IFilterOperation ParseUnaryExpression(Expression expression, bool skipBrackets = false)
		{
			var exp = expression.UnwrapConvert();
			return exp.NodeType switch
				{
					ExpressionType.Not => new NotOperation((UnaryExpression) exp, _dialect, this),
					ExpressionType.MemberAccess => ParseMemberAccessExpression(exp as MemberExpression),
					ExpressionType.Constant => new ConstantOperation((ConstantExpression) exp, _dialect, this),
					ExpressionType.Call => ParseCallExpression(exp as MethodCallExpression, skipBrackets),
					_ => new ValueOperation(exp, _dialect, this)
				};
		}

		private IFilterOperation ParseMemberAccessExpression(MemberExpression expression)
		{
			if (expression.Expression.NodeType == ExpressionType.Parameter)
			{
				return new ParameterRefOperation(expression, _dialect, _nameConverter);
			}

			return new ValueOperation(expression, _dialect, this);
		}

		private IFilterOperation ParseCallExpression(MethodCallExpression expression, bool skipBrackets = false)
		{
			if (_dictionaryPseudoMethods.TryGetValue(expression.Method, out var filter))
			{
				return filter(expression, _dialect, this, skipBrackets);
			}

			return new ValueOperation(expression, _dialect, this);
		}


	}
}
