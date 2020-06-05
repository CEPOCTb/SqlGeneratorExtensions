using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Sql.EF.Dapper.Extensions
{
	public class MappedDynamicParameters : DynamicParameters
	{
		/// <inheritdoc />
		public MappedDynamicParameters()
		{
		}

		public MappedDynamicParameters(object entity) : base(entity)
		{
			PropertyToColumnMapping = ParameterNames.ToDictionary(p => p, p => p);
			ColumnToParameterMapping = ParameterNames.ToDictionary(p => p, p => p);
		}

		public IDictionary<string, string> PropertyToColumnMapping { get; } = new Dictionary<string, string>();
		public IDictionary<string, string> ColumnToParameterMapping { get; } = new Dictionary<string, string>();
	}
}
