using Microsoft.EntityFrameworkCore;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.EF.Dapper.Extensions
{
	public abstract class DapperHybridContext : DbContext
	{
		protected DapperHybridContext(DbContextOptions options) : base(options)
		{
		}

		public abstract ISqlGenerator SqlGenerator { get; }
	}
}
