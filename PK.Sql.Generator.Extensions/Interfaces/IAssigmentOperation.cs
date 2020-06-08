using System.Text;

namespace PK.Sql.Generator.Extensions.Interfaces
{
	public interface IAssigmentOperation
	{
		string Name { get; }
		
		string ColumnName { get; }
		
		bool IsValue { get; }

		object Value { get; }

		void AppendSql(StringBuilder builder, GeneratorContext filterParams);
	}
}
