using System.Reflection;

namespace PK.Sql.Generator.Extensions.Interfaces
{
	public interface INameConverter
	{
		string GetColumnName(string propertyName);
	}
}
