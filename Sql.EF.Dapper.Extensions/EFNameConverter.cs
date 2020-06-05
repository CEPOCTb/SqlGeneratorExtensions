using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.EF.Dapper.Extensions
{
	public class EFNameConverter<TEntity> : INameConverter
	{
		[NotNull] private readonly IEntityType _entityType;

		public EFNameConverter([NotNull] DbContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			_entityType = context.Model.FindEntityType(typeof(TEntity))
				?? throw new InvalidOperationException($"Entity of type {typeof(TEntity).FullName} is not registered if DbContext. Use overload with table name and schema");
		}

		#region Implementation of INameConverter

		/// <inheritdoc />
		public string GetColumnName(string propertyName)
		{
			var property = _entityType.FindProperty(propertyName);
			if (property == null)
			{
				return propertyName;
			}
			
			return property.GetColumnName();
		}

		#endregion
	}
}
