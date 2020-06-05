using System.Reflection;

namespace Sql.Generator.Extensions.Interfaces
{
	public interface INameConverter
	{
		string GetColumnName(string propertyName);
	}
}
