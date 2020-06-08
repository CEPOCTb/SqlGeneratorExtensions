using System.Collections.Generic;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions
{
	public class AssigmentResult
	{
		public List<IAssigmentOperation> Assigments { get; } = new List<IAssigmentOperation>();
		
		
	}
}
