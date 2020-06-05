using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Extensions.Filters;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Filters
{
	public class SimpleValueOperation : FilterOperationBase
	{
		[NotNull] private readonly FilterParseHelpers _parseHelper;
		public override bool IsValue => true;

		public override object Value { get; }

		public SimpleValueOperation(
			object value,
			[NotNull] ISqlDialect dialect,
			[NotNull] FilterParseHelpers parseHelper) : base(dialect)
		{
			_parseHelper = parseHelper ?? throw new ArgumentNullException(nameof(parseHelper));
			Value = value;
		}

		public override void AppendSql([NotNull] StringBuilder builder, [NotNull] GeneratorContext filterParams)
		{
			if (_parseHelper.IsSimpleType(Value.GetType()))
			{
				builder.Append(Value);
			}
			else
			{
				var paramName = filterParams.Add(Value);
				builder.Append(Dialect.GetParameterReference(paramName));
			}
		}

	}
}