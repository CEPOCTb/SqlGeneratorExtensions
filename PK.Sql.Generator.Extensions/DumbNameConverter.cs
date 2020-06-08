using System.Reflection;
using PK.Sql.Generator.Extensions.Interfaces;

namespace PK.Sql.Generator.Extensions
{
	public class DumbNameConverter : INameConverter
	{
		#region Implementation of INameConverter

		/// <inheritdoc />
		public string GetColumnName(string propertyName) => propertyName;

		#endregion
	}
}
