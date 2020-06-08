using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.EF.Dapper.Extensions
{
	public class DbContextOptionsExtension<T> : IDbContextOptionsExtension where T : class, ISqlGenerator
	{
		private class ExtensionInfo : DbContextOptionsExtensionInfo
		{
			/// <inheritdoc />
			public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension)
			{
			}

			#region Overrides of DbContextOptionsExtensionInfo

			/// <inheritdoc />
			public override long GetServiceProviderHashCode() => 0;

			/// <inheritdoc />
			public override void PopulateDebugInfo(IDictionary<string, string> debugInfo) =>
				debugInfo["Dapper:HybridContext"] = "1";

			/// <inheritdoc />
			public override bool IsDatabaseProvider => false;

			/// <inheritdoc />
			public override string LogFragment => $"Dapper:SqlGenerator used: {typeof(T).Name}";

			#endregion
		}

		#region Implementation of IDbContextOptionsExtension

		/// <inheritdoc />
		public void ApplyServices(IServiceCollection services)
		{
			services.AddSingleton<ISqlGenerator, T>();
		}

		/// <inheritdoc />
		public void Validate(IDbContextOptions options)
		{
		}

		/// <inheritdoc />
		public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

		#endregion
	}
}
