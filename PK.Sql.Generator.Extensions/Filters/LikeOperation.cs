using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Filters
{
	public class LikeOperation : PseudoMethodCallOperation
	{
		[NotNull] private readonly IFilterOperation _left;
		[NotNull] private readonly IFilterOperation _right;

		public override bool IsValue => false;

		public override object Value => throw CreateException();

		public LikeOperation(
			[NotNull] Expression expression,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper,
			bool skipBrackets) : base(expression, dialect, skipBrackets)
		{
			_left = parseHelper.ParseExpression(CallExpression.Arguments[0]);
			_right = parseHelper.ParseExpression(CallExpression.Arguments[1]);
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			if (!SkipBrackets)
			{
				builder.Append(Dialect.OpenBracket);
			}

			_left.AppendSql(builder, filterParams);

			builder.Append(" " + Dialect.LikeKeyword + " ");

			_right.AppendSql(builder, filterParams);
			
			if (!SkipBrackets)
			{
				builder.Append(Dialect.CloseBracket);
			}
		}
	}
}