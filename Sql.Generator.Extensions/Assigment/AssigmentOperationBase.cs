using System;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Assigment
{
	public abstract class AssigmentOperationBase : IAssigmentOperation
	{
		[NotNull] private readonly INameConverter _nameConverter;

		[NotNull]
		protected ISqlDialect Dialect { get; }

		protected AssigmentOperationBase(string name, [NotNull] ISqlDialect dialect, [NotNull] INameConverter nameConverter)
		{
			Name = name;
			_nameConverter = nameConverter ?? throw new ArgumentNullException(nameof(nameConverter));
			Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
		}


		#region Implementation of IFilterExpression

		/// <inheritdoc />
		public virtual string Name { get; protected set; }
		
		/// <inheritdoc />
		public string ColumnName  => _nameConverter.GetColumnName(Name);

		/// <inheritdoc />
		public abstract bool IsValue { get; }

		/// <inheritdoc />
		public abstract object Value { get; }

		/// <inheritdoc />
		public abstract void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams);

		#endregion

		protected object GetValue([NotNull] Expression expression)
		{
			return IsValue
				? Expression.Lambda(expression).Compile().DynamicInvoke()
				: throw CreateException();
		}

		protected Exception CreateException() =>
			throw new InvalidOperationException("This filter can't be reduced to a value. Check IsValue property before use.");

	}
}
