using System.Collections.Generic;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions
{
	public class AssigmentResult
	{
		public List<IAssigmentOperation> Assigments { get; } = new List<IAssigmentOperation>();
		
		
	}
}
