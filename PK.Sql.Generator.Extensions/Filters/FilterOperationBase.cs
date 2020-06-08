using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Filters
{
	public abstract class FilterOperationBase : IFilterOperation
	{
		[NotNull]
		protected ISqlDialect Dialect { get; }

		protected FilterOperationBase([NotNull] ISqlDialect dialect)
		{
			Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
		}

		
		#region Implementation of IFilterExpression

		/// <inheritdoc />
		public abstract bool IsValue { get; }

		/// <inheritdoc />
		public abstract object Value { get; }

		/// <inheritdoc />
		public abstract void AppendSql(StringBuilder builder, GeneratorContext filterParams);

		#endregion

		protected object GetValue(Expression expression)
		{
			return IsValue
				? Expression.Lambda(expression).Compile().DynamicInvoke()
				: throw CreateException();
		}
		
		protected Exception CreateException() =>
			throw new InvalidOperationException("This filter can't be reduced to a value. Check IsValue property before use.");
		
	}
}
