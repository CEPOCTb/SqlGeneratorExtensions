using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Assigment;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Extensions.Assigment
{
	public class AssigmentParseHelpers
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

		private readonly Dictionary<MethodInfo, Func<MethodCallExpression, ISqlDialect, INameConverter, AssigmentParseHelpers, IAssigmentOperation>> _dictionaryInitPseudoMethods =
			new Dictionary<MethodInfo, Func<MethodCallExpression, ISqlDialect, INameConverter, AssigmentParseHelpers, IAssigmentOperation>>()
				{
					[typeof(AssignPseudoMethods).GetMethod(nameof(AssignPseudoMethods.Column))!] =
						(expression, dialect, converter, helper) => new CustomColumnOperation(expression, dialect, converter, helper),
				};

		[NotNull] private readonly ISqlDialect _dialect;
		[NotNull] private readonly INameConverter _nameConverter;

		public AssigmentParseHelpers(
			[NotNull] ISqlDialect dialect,
			[NotNull] INameConverter nameConverter,
			IEnumerable<
				KeyValuePair<
					MethodInfo,
					Func<
						MethodCallExpression,
						ISqlDialect,
						INameConverter,
						AssigmentParseHelpers,
						IAssigmentOperation>
					>
				> customHandlers = null)
		{
			_dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
			_nameConverter = nameConverter ?? throw new ArgumentNullException(nameof(nameConverter));

			if (customHandlers != null)
			{
				foreach (var pair in customHandlers)
				{
					_dictionaryInitPseudoMethods.Add(pair.Key, pair.Value);
				}
			}
		}

		public bool IsSimpleType(Type type)
		{
			return SimpleTypes.Contains(type);
		}

		
		public AssigmentResult ParseAssigment(Expression expression)
		{
			var exp = expression.UnwrapConvert();

			var assigmentResult = new AssigmentResult();

			if (exp.NodeType == ExpressionType.Call)
			{
				return ParseCallInitExpression((MethodCallExpression) exp, assigmentResult);
			}

			if (exp.NodeType == ExpressionType.MemberInit)
			{
				return ParseMemberInitExpression((MemberInitExpression) exp, assigmentResult);
			}

			if (exp.NodeType == ExpressionType.New)
			{
				return ParseNewExpression((NewExpression) exp, assigmentResult);
			}
			
			throw new InvalidOperationException($"Not supported expression type {exp.NodeType}");
		}

		public IAssigmentOperation ParseExpression(Expression expression, string name, bool skipBrackets = false)
		{
			var exp = expression.UnwrapConvert();
			if (exp is BinaryExpression bin)
			{
				return new BinaryOperation(
					bin,
					expression,
					name,
					_dialect,
					this,
					_nameConverter,
					skipBrackets
					);
			}
			else
			{
				return ParseUnaryExpression(expression, name, skipBrackets);
			}
		}

		private AssigmentResult ParseCallInitExpression(MethodCallExpression expression, AssigmentResult assigmentResult)
		{
			if (_dictionaryInitPseudoMethods.TryGetValue(expression.Method, out var operation))
			{
				var op = operation(expression, _dialect, _nameConverter, this);
				assigmentResult.Assigments.Add(op); 
			}

			throw new InvalidOperationException($"Not supported method {expression.Method.Name}");
		}

		private AssigmentResult ParseMemberInitExpression(MemberInitExpression expression, AssigmentResult assigmentResult)
		{
			ParseNewExpression(expression.NewExpression, assigmentResult);

			foreach (var memberBinding in expression.Bindings)
			{
				var assigment = (MemberAssignment) memberBinding;
				var operation = ParseBinding(assigment);
				assigmentResult.Assigments.Add(operation);
			}

			return assigmentResult;
		}

		private AssigmentResult ParseNewExpression(NewExpression expression, AssigmentResult assigmentResult)
		{
			for (int i = 0; i < expression.Arguments.Count; i++)
			{
				assigmentResult.Assigments.Add(ParseExpression(expression.Arguments[i], expression.Members[i].Name));
			}

			return assigmentResult;
		}

		private IAssigmentOperation ParseBinding(MemberAssignment binding)
		{
			return ParseExpression(binding.Expression, binding.Member.Name);
		}

		private IAssigmentOperation ParseUnaryExpression(Expression expression, string name, bool skipBrackets = false)
		{
			var exp = expression.UnwrapConvert();
			return exp.NodeType switch
				{
					ExpressionType.Not => new NotOperation((UnaryExpression) exp, name, _dialect, _nameConverter, this),
					ExpressionType.MemberAccess => ParseMemberAccessExpression(expression, name),
					_ => new ValueOperation(expression, name, _dialect, _nameConverter)
				};
		}

		private IAssigmentOperation ParseMemberAccessExpression(Expression expression, string name)
		{
			var exp = expression.UnwrapConvert() as MemberExpression;
			if (exp.Expression.NodeType == ExpressionType.Parameter)
			{
				return new ParameterRefOperation(exp, name, _dialect, _nameConverter);
			}

			return new ValueOperation(expression, name, _dialect, _nameConverter);
		}
	}
}
