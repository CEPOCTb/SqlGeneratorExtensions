using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sql.Generator.Extensions.Models;

namespace Sql.Generator.Extensions.Interfaces
{
	public interface ISqlGenerator
	{
		InsertStatement Insert(string schema, string table, IDictionary<string, string> parameterNames);

		WhereStatement Where<TParam>(Expression<Func<TParam, bool>> filterExpression);

		UpdateStatement Update<TParam>(string schema, string table, Expression<Func<TParam>> updateAction, Expression<Func<TParam, bool>> filterExpression = null);

		DeleteStatement Delete<TParam>(string schema, string table, Expression<Func<TParam, bool>> filterExpression);
	}
}
