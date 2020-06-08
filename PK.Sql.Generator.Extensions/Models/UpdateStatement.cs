using System.Collections.Generic;

namespace PK.Sql.Generator.Extensions.Models
{
	public class UpdateStatement
	{
		public string Statement { get; set; }

		public IDictionary<string, object> Params { get; set; }
		
		public IDictionary<string, string> ColumnsMap { get; set; }
	}
}
