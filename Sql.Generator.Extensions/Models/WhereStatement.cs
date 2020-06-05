using System.Collections.Generic;

namespace Sql.Generator.Extensions.Models
{
	public class WhereStatement
	{
		public string Statement { get; set; }

		public IDictionary<string, object> Params { get; set; }
	}
}
