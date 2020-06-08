using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Interfaces;
using PK.Sql.Generator.Extensions.Extensions;

namespace PK.Sql.Generator.Extensions.Filters
{
	public abstract class PseudoMethodCallOperation : FilterOperationBase
	{
		protected MethodCallExpression CallExpression { get; }
		protected bool SkipBrackets { get; }

		protected PseudoMethodCallOperation([NotNull] Expression expression, ISqlDialect dialect, bool skipBrackets) : base(dialect)
		{
			CallExpression = expression?.UnwrapConvert() as MethodCallExpression ?? throw new ArgumentNullException(nameof(expression));
			SkipBrackets = skipBrackets;
		}

	}
}