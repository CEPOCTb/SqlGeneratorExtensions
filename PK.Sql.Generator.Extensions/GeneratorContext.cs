using System.Collections.Generic;
using JetBrains.Annotations;

namespace PK.Sql.Generator.Extensions
{
	public class GeneratorContext
	{
		private int _index = 0;

		[NotNull]
		public Dictionary<object, string> Params { get; } = new Dictionary<object, string>();

		[NotNull]
		public Dictionary<string, string> ColumnsMappings { get; } = new Dictionary<string, string>();

		public string Add(object param)
		{
			if (!Params.TryGetValue(param, out var paramName))
			{
				paramName = "p" + _index++;
				Params.Add(param, paramName);	
			}
			return paramName;
		}
	}
}
