using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PK.Sql.Generator.Extensions.Extensions.Assigment;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Interfaces;
using PK.Sql.Generator.Extensions.Models;

namespace PK.Sql.Generator.Extensions.Dialects
{
	public abstract class BaseDialect : ISqlDialect
	{
		#region Implementation of ISqlDialect

		/// <inheritdoc />
		public virtual string GetEscapedTableName(string table, string schema = null, string alias = null)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(schema))
			{
				sb.Append(NameEscapeOpen)
					.Append(schema)
					.Append(NameEscapeClose)
					.Append('.');
			}

			sb.Append(NameEscapeOpen)
				.Append(table)
				.Append(NameEscapeClose);

			if (!string.IsNullOrWhiteSpace(alias))
			{
				sb.Append(" AS ")
					.Append(NameEscapeOpen)
					.Append(alias)
					.Append(NameEscapeClose);
			}

			return sb.ToString();
		}

		/// <inheritdoc />
		public virtual string GetEscapedParameterName(string parameterName, string alias = null)
		{

			var sb = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(alias))
			{
				sb.Append(NameEscapeOpen)
					.Append(alias)
					.Append(NameEscapeClose)
					.Append('.');
			}

			return sb.Append(NameEscapeOpen)
				.Append(parameterName)
				.Append(NameEscapeClose)
				.ToString();
		}


		/// <inheritdoc />
		public virtual string GetParameterReference(string parameterName) =>
			new StringBuilder(ParameterPrefix)
				.Append(parameterName)
				.ToString();

		/// <inheritdoc />
		public virtual bool NotEqualsSupported => true;

		/// <inheritdoc />
		public virtual string ParameterPrefix => "@";

		/// <inheritdoc />
		public virtual string NameEscapeOpen => "\"";

		/// <inheritdoc />
		public virtual string NameEscapeClose => "\"";

		/// <inheritdoc />
		public virtual string OpenBracket => "(";

		/// <inheritdoc />
		public virtual string CloseBracket => ")";

		/// <inheritdoc />
		public virtual string LogicalAnd => "AND";

		/// <inheritdoc />
		public virtual string LogicalOr => "OR";

		/// <inheritdoc />
		public virtual string LogicalEquals => "=";

		/// <inheritdoc />
		public virtual string NullEquals => "IS";

		/// <inheritdoc />
		public virtual string LogicalGreater => ">";

		/// <inheritdoc />
		public virtual string LogicalGreaterEquals => ">=";

		/// <inheritdoc />
		public virtual string LogicalLess => "<";

		/// <inheritdoc />
		public virtual string LogicalLessEquals => "<=";

		/// <inheritdoc />
		public string LogicalNotEquals => "<>";

		/// <inheritdoc />
		public virtual string PlusSign => "+";

		/// <inheritdoc />
		public virtual string MinusSign => "-";

		/// <inheritdoc />
		public virtual string MultiplySign => "*";

		/// <inheritdoc />
		public virtual string DivideSign => "/";

		/// <inheritdoc />
		public virtual string LikeKeyword => "LIKE";

		/// <inheritdoc />
		public virtual string NotKeyword => "NOT";

		/// <inheritdoc />
		public string InKeyword => "IN";

		/// <inheritdoc />
		public virtual string AssignSign => "=";

		public virtual InsertStatement Insert(string schema, string table, IDictionary<string, string> parameterNames)
		{
			var parameters = parameterNames.ToList();

			var stringBuilder = new StringBuilder("INSERT INTO ")
				.Append(GetEscapedTableName(table, schema))
				.Append(" (")
				.Append(
					string.Join(
						", ",
						parameters.Select(p => GetEscapedParameterName(p.Key))
						)
					)
				.Append(") VALUES(")
				.Append(
					string.Join(
						", ",
						parameters.Select(p => GetParameterReference(p.Value))
						)
					)
				.Append(')');

			return new InsertStatement { Statement = stringBuilder.ToString() };
		}

		/// <inheritdoc />
		public WhereStatement Where<TParam>(Expression<Func<TParam, bool>> filterExpression, FilterParseHelpers filterParseHelpers)
		{
			var generatorContext = new GeneratorContext();

			var sb = new StringBuilder();

			var filter = filterParseHelpers.ParseFilter(filterExpression.Body);
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

		public virtual UpdateStatement Update<TParam>(
			string schema,
			string table,
			Expression<Func<TParam>> updateAction,
			Expression<Func<TParam, bool>> filterExpression,
			FilterParseHelpers filterParseHelpers,
			AssigmentParseHelpers assigmentParseHelpers
			)
		{
			var generatorContext = new GeneratorContext();

			var tableAlias = filterParseHelpers.GetTableAlias(filterExpression);
			var updateParams = assigmentParseHelpers.ParseAssigment(updateAction.Body);

			var sb = new StringBuilder("UPDATE ")
				.Append(GetEscapedTableName(table, schema, tableAlias))
				.Append(" SET ");

			var first = updateParams.Assigments.First();
			foreach (var assigment in updateParams.Assigments)
			{
				if (assigment != first)
				{
					sb.Append(", ");
				}

				generatorContext.ColumnsMappings[assigment.ColumnName] = assigment.Name;

				sb.Append(GetEscapedParameterName(assigment.ColumnName, tableAlias))
					.Append(" ")
					.Append(AssignSign)
					.Append(" ");

				assigment.AppendSql(sb, generatorContext);
			}

			if (filterExpression != null)
			{
				var filter = filterParseHelpers.ParseFilter(filterExpression.Body);
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
		public virtual DeleteStatement Delete<TParam>(string schema, string table, Expression<Func<TParam, bool>> filterExpression, FilterParseHelpers filterParseHelpers)
		{
			var generatorContext = new GeneratorContext();

			var tableAlias = filterParseHelpers.GetTableAlias(filterExpression);

			var sb = new StringBuilder("DELETE FROM ")
				.Append(GetEscapedTableName(table, schema, tableAlias));

			var filter = filterParseHelpers.ParseFilter(filterExpression.Body);
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
}
