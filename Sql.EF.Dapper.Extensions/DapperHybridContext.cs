using Microsoft.EntityFrameworkCore;
using Sql.Generator.Extensions;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.EF.Dapper.Extensions
{
	public abstract class DapperHybridContext : DbContext
	{
		protected DapperHybridContext(DbContextOptions options) : base(options)
		{
		}

		public abstract ISqlGenerator CreateSqlGenerator<TEntity>();

		public abstract INameConverter CreateNameConverter<TEntity>();
	}

	public abstract class DapperHybridContext<T> : DapperHybridContext where T : ISqlDialect, new()
	{
		/// <inheritdoc />
		protected DapperHybridContext(DbContextOptions options) : base(options)
		{
		}

		#region Overrides of DapperHybridContext

		/// <inheritdoc />
		public override ISqlGenerator CreateSqlGenerator<TEntity>()
		{
			return new SqlGenerator<T>(new T(), CreateNameConverter<TEntity>());
		}

		/// <inheritdoc />
		public override INameConverter CreateNameConverter<TEntity>()
		{
			return new EFNameConverter<TEntity>(this);
		}

		#endregion
	}
}
