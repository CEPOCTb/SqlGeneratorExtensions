using System.Collections.Generic;

namespace PK.Sql.Generator.Extensions.Models
{
	public class DeleteStatement
	{
		public string Statement { get; set; }

		public IDictionary<string, object> Params { get; set; }
	}
}
