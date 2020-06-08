using System.Text;

namespace PK.Sql.Generator.Extensions.Interfaces
{
	public interface IFilterOperation
	{
		bool IsValue { get; }

		object Value { get; }

		void AppendSql(StringBuilder builder, GeneratorContext filterParams);
	}
}
