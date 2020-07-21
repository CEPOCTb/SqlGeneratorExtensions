using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using PK.Sql.Generator.Extensions.Extensions.Filters;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions.Filters
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
			var value = Value;

			if (value is null)
			{
				builder.Append("NULL");
			}
			else if (_parseHelper.IsSimpleType(value.GetType()))
			{
				builder.Append(value);
			}
			else
			{
				var paramName = filterParams.Add(value);
				builder.Append(Dialect.GetParameterReference(paramName));
			}
		}

	}
}
