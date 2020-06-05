using System.Reflection;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions
{
	public class DumbNameConverter : INameConverter
	{
		#region Implementation of INameConverter

		/// <inheritdoc />
		public string GetColumnName(string propertyName) => propertyName;

		#endregion
	}
}
