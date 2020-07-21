using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using PK.Sql.Generator.Extensions.Extensions.Assigment;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Models;

namespace PK.Sql.Generator.Extensions.Interfaces
{
	public interface ISqlDialect
	{
		string GetEscapedTableName(string table, string schema = null, string alias = null);

		string GetEscapedParameterName(string parameterName, string alias = null);

		string GetParameterReference(string parameterName);

		bool NotEqualsSupported { get; }
		string ParameterPrefix { get; }
		string NameEscapeOpen { get; }
		string NameEscapeClose { get; }
		string OpenBracket { get; }
		string CloseBracket { get; }
		string LogicalAnd { get; }
		string LogicalOr { get; }
		string LogicalEquals { get; }
		string NullEquals { get; }
		string LogicalGreater { get; }
		string LogicalGreaterEquals { get; }
		string LogicalLess { get; }
		string LogicalLessEquals { get; }
		string LogicalNotEquals { get; }
		string PlusSign { get; }
		string MinusSign { get; }
		string MultiplySign { get; }
		string DivideSign { get; }
		string LikeKeyword { get; }
		string NotKeyword { get; }
		string InKeyword { get; }
		string AssignSign { get; }

		InsertStatement Insert(string schema, string table, IDictionary<string, string> parameterNames);

		WhereStatement Where<TParam>(Expression<Func<TParam, bool>> filterExpression, FilterParseHelpers filterParseHelpers);

		UpdateStatement Update<TParam>(string schema, string table, Expression<Func<TParam>> updateAction, Expression<Func<TParam, bool>> filterExpression, FilterParseHelpers filterParseHelpers, AssigmentParseHelpers assigmentParseHelpers);

		DeleteStatement Delete<TParam>(string schema, string table, Expression<Func<TParam, bool>> filterExpression, FilterParseHelpers filterParseHelpers);
	}
}
