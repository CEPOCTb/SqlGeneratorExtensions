using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Filters
{
	public abstract class PseudoMethodCallOperation : FilterOperationBase
	{
		protected MethodCallExpression CallExpression { get; }
		protected bool SkipBrackets { get; }

		protected PseudoMethodCallOperation([NotNull] MethodCallExpression expression, ISqlDialect dialect, bool skipBrackets) : base(dialect)
		{
			CallExpression = expression ?? throw new ArgumentNullException(nameof(expression));
			SkipBrackets = skipBrackets;
		}

	}
}