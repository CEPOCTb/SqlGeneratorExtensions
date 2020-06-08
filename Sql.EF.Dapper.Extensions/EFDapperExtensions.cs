using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Sql.Generator.Extensions;

namespace Sql.EF.Dapper.Extensions
{
	[PublicAPI]
	public static class EFDapperExtensions
	{
		public static MappedDynamicParameters GetParameters<TEntity>(
			this DbContext context,
			TEntity entity
			)
		{
			var entityType = context.Model.FindEntityType(typeof(TEntity));
			if (entityType == null)
			{
				throw new InvalidOperationException($"Entity of type {typeof(TEntity).FullName} is not registered if DbContext. Use overload with table name and schema");
			}

			var parameters = new MappedDynamicParameters();

			foreach (var p in entityType.GetProperties())
			{
				var typeMapping = p.FindRelationalMapping();
				var valueConverter = p.GetValueConverter() ?? typeMapping.Converter;

				var columnName = p.GetColumnName();
				parameters.PropertyToColumnMapping.Add(p.Name, columnName);
				parameters.ColumnToParameterMapping.Add(columnName, columnName);

				if (p.IsShadowProperty())
				{
					var generator = p.GetValueGeneratorFactory().Invoke(p, entityType);
					// this only supports simple generators, where passed entity is not used!
					var val = generator.Next(null);
					parameters.Add(
						columnName,
						valueConverter != null ? valueConverter.ConvertToProvider(val) : val,
						typeMapping.DbType,
						size: typeMapping.Size
						);
					continue;
				}

				parameters.Add(
					columnName,
					valueConverter != null
						? valueConverter.ConvertToProvider(p.GetGetter().GetClrValue(entity))
						: p
							.GetGetter()
							.GetClrValue(entity),
					typeMapping.DbType,
					size: typeMapping.Size
					);
			
			}

			return parameters;
		}

		[PublicAPI]
		public static async Task InsertAsync<TEntity>(this DapperHybridContext context, TEntity entity)
		{
			var connection = context.Database.GetDbConnection();
			var entityType = context.Model.FindEntityType(typeof(TEntity));
			if (entityType == null)
			{
				throw new InvalidOperationException($"Entity of type {typeof(TEntity).FullName} is not registered if DbContext. Use overload with table name and schema");
			}
			
			var parameters = context.GetParameters(entity);
			var schema = entityType.GetSchema();
			var table = entityType.GetTableName();

			var statement = context.CreateSqlGenerator<TEntity>().Insert(schema, table, parameters.ColumnToParameterMapping);
			
			await connection.ExecuteAsync(
				statement.Statement,
				parameters,
				transaction: context.Database.CurrentTransaction?.GetDbTransaction())
				.ConfigureAwait(false);
		}

		[PublicAPI]
		public static async Task InsertAsync<TEntity>(this DapperHybridContext context, string schema, string table, TEntity entity)
		{
			var connection = context.Database.GetDbConnection();
			var parameters = new MappedDynamicParameters(entity);

			var statement = context.CreateSqlGenerator<TEntity>().Insert(schema, table, parameters.ColumnToParameterMapping);
			
			await connection.ExecuteAsync(
					statement.Statement,
					parameters,
					transaction: context.Database.CurrentTransaction?.GetDbTransaction()
					)
				.ConfigureAwait(false);
		}

		[PublicAPI]
		public static async Task UpdateAsync<TEntity>(this DapperHybridContext context, string schema, string table, Expression<Func<TEntity>> updateExpression, Expression<Func<TEntity, bool>> filterExpression = null)
		{
			var filterParameters = new GeneratorContext();

			var connection = context.Database.GetDbConnection();
			var updateStatement = context.CreateSqlGenerator<TEntity>()
				.Update(
					schema,
					table,
					updateExpression,
					filterExpression
					);

			var parameters = new DynamicParameters();
			foreach (var parameter in updateStatement.Params)
			{
				parameters.Add(parameter.Key, parameter.Value);
			}

			await connection.ExecuteAsync(
					updateStatement.Statement,
					parameters,
					transaction: context.Database.CurrentTransaction?.GetDbTransaction()
					)
				.ConfigureAwait(false);
		}

		[PublicAPI]
		public static Task UpdateAsync<TEntity>(this DapperHybridContext context, Expression<Func<TEntity>> updateExpression, Expression<Func<TEntity, bool>> filterExpression = null)
		{
			var entityType = context.Model.FindEntityType(typeof(TEntity));
			if (entityType == null)
			{
				throw new InvalidOperationException($"Entity of type {typeof(TEntity).FullName} is not registered if DbContext. Use overload with table name and schema");
			}

			return UpdateAsync(
				context,
				entityType.GetSchema(),
				entityType.GetTableName(),
				updateExpression,
				filterExpression
				);
		}

		[PublicAPI]
		public static async Task DeleteAsync<TEntity>(this DapperHybridContext context, string schema, string table, Expression<Func<TEntity, bool>> filterExpression)
		{
			var filterParameters = new GeneratorContext();

			var connection = context.Database.GetDbConnection();
			var deleteStatement = context.CreateSqlGenerator<TEntity>()
				.Delete(
					schema,
					table,
					filterExpression
					);

			var parameters = new DynamicParameters();
			foreach (var parameter in deleteStatement.Params)
			{
				parameters.Add(parameter.Key, parameter.Value);
			}

			await connection.ExecuteAsync(
					deleteStatement.Statement,
					parameters,
					transaction: context.Database.CurrentTransaction?.GetDbTransaction()
					)
				.ConfigureAwait(false);
		}

		[PublicAPI]
		public static Task DeleteAsync<TEntity>(this DapperHybridContext context, Expression<Func<TEntity, bool>> filterExpression)
		{
			var entityType = context.Model.FindEntityType(typeof(TEntity));
			if (entityType == null)
			{
				throw new InvalidOperationException($"Entity of type {typeof(TEntity).FullName} is not registered if DbContext. Use overload with table name and schema");
			}

			return DeleteAsync(context, entityType.GetSchema(), entityType.GetTableName(), filterExpression);
		}
		
	}
}
