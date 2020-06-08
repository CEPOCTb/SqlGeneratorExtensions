using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Extensions.Assigment;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Interfaces;
using PK.Sql.Generator.Extensions.Models;

namespace PK.Sql.Generator.Extensions
{
	public class SqlGenerator : ISqlGenerator
	{
		[NotNull] private readonly ISqlDialect _dialect;
		private readonly FilterParseHelpers _filterParseHelpers;
		private readonly AssigmentParseHelpers _assigmentParseHelpers;

		public SqlGenerator([NotNull] ISqlDialect dialect, [NotNull] INameConverter nameConverter)
		{
			if (dialect == null)
			{
				throw new ArgumentNullException(nameof(dialect));
			}

			if (nameConverter == null)
			{
				throw new ArgumentNullException(nameof(nameConverter));
			}

			_dialect = dialect;
			_filterParseHelpers = new FilterParseHelpers(_dialect, nameConverter);
			_assigmentParseHelpers = new AssigmentParseHelpers(_dialect, nameConverter);
		}

		#region Implementation of ISqlGenerator

		/// <inheritdoc />
		public InsertStatement Insert(string schema, string table, IDictionary<string, string> parameterNames)
		{
			var parameters = parameterNames.ToList();

			var stringBuilder = new StringBuilder("INSERT INTO ")
				.Append(_dialect.GetEscapedTableName(table, schema))
				.Append(" (")
				.Append(
					string.Join(
						", ",
						parameters.Select(p => _dialect.GetEscapedParameterName(p.Key))
						)
					)
				.Append(") VALUES(")
				.Append(
					string.Join(
						", ",
						parameters.Select(p => _dialect.GetParameterReference(p.Value))
						)
					)
				.Append(')');

			return new InsertStatement { Statement = stringBuilder.ToString() };
		}

		/// <inheritdoc />
		public WhereStatement Where<TParam>(Expression<Func<TParam, bool>> filterExpression)
		{
			var generatorContext = new GeneratorContext();

			var sb = new StringBuilder();
			
			var filter = _filterParseHelpers.ParseFilter(filterExpression.Body);
			filter.AppendSql(sb, generatorContext);

			return new WhereStatement
				{
					Statement = sb.ToString(),
					// reverse dictionary
					Params = generatorContext.Params.ToDictionary(
						p => p.Value,
						p => p.Key
						)
				};
		}

		public UpdateStatement Update<TParam>(string schema, string table, Expression<Func<TParam>> updateAction, Expression<Func<TParam, bool>> filterExpression = null)
		{
			var generatorContext = new GeneratorContext();

			var tableAlias = _filterParseHelpers.GetTableAlias(filterExpression);
			var updateParams = _assigmentParseHelpers.ParseAssigment(updateAction.Body);

			var sb = new StringBuilder("UPDATE ")
				.Append(_dialect.GetEscapedTableName(table, schema, tableAlias))
				.Append(" SET ");

			var first = updateParams.Assigments.First();
			foreach (var assigment in updateParams.Assigments)
			{
				if (assigment != first)
				{
					sb.Append(", ");
				}

				generatorContext.ColumnsMappings[assigment.ColumnName] = assigment.Name;
				
				sb.Append(_dialect.GetEscapedParameterName(assigment.ColumnName, tableAlias))
					.Append(" ")
					.Append(_dialect.AssignSign)
					.Append(" ");

				assigment.AppendSql(sb, generatorContext);
			}

			if (filterExpression != null)
			{
				var filter = _filterParseHelpers.ParseFilter(filterExpression.Body);
				sb.Append(" WHERE ");
				filter.AppendSql(sb, generatorContext);
			}

			return new UpdateStatement
				{
					Statement = sb.ToString(),
					ColumnsMap = generatorContext.ColumnsMappings,
					Params = generatorContext.Params.ToDictionary(
						p => p.Value,
						p => p.Key
						)
				};
		}

		/// <inheritdoc />
		public DeleteStatement Delete<TParam>(string schema, string table, Expression<Func<TParam, bool>> filterExpression)
		{
			var generatorContext = new GeneratorContext();

			var tableAlias = _filterParseHelpers.GetTableAlias(filterExpression);

			var sb = new StringBuilder("DELETE FROM ")
				.Append(_dialect.GetEscapedTableName(table, schema, tableAlias));

			var filter = _filterParseHelpers.ParseFilter(filterExpression.Body);
			sb.Append(" WHERE ");
			filter.AppendSql(sb, generatorContext);

			return new DeleteStatement
				{
					Statement = sb.ToString(),
					Params = generatorContext.Params.ToDictionary(
						p => p.Value,
						p => p.Key
						)
				};
		}

		#endregion
	}
	
	public class SqlGenerator<T> : SqlGenerator where T : ISqlDialect, new()
	{
		public SqlGenerator() : this(new T(), new DumbNameConverter())
		{
		}

		public SqlGenerator(INameConverter nameConverter) : this(new T(), nameConverter)
		{
		}

		public SqlGenerator(T dialect, INameConverter nameConverter) : base(dialect, nameConverter)
		{
		}

	}
}
