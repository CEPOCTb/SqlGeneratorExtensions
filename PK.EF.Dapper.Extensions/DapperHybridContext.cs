using Microsoft.EntityFrameworkCore;
using PK.Sql.Generator.Extensions;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.EF.Dapper.Extensions
{
	public abstract class DapperHybridContext : DbContext
	{
		protected DapperHybridContext(DbContextOptions options) : base(options)
		{
		}

		public abstract ISqlGenerator CreateSqlGenerator<TEntity>();

		public virtual INameConverter CreateNameConverter<TEntity>() => new EFNameConverter<TEntity>(this);
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
			return new SqlGenerator<T>(CreateNameConverter<TEntity>());
		}

		#endregion
	}
}
