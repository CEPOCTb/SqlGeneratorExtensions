namespace Sql.Generator.Extensions.Interfaces
{
	public interface ISqlDialect
	{
		string GetEscapedTableName(string table, string schema = null, string alias = null);

		string GetEscapedParameterName(string parameterName, string alias = null);

		string GetParameterReference(string parameterName);

		bool NotEqualsSupported { get; }
		string ParameterPrefix { get; }
		string NameEscapeOpen { get; }
		string NameEscapeClose { get; }
		string OpenBracket { get; }
		string CloseBracket { get; }
		string LogicalAnd { get; }
		string LogicalOr { get; }
		string LogicalEquals { get; }
		string LogicalNotEquals { get; }
		string PlusSign { get; }
		string MinusSign { get; }
		string MultiplySign { get; }
		string DivideSign { get; }
		string LikeKeyword { get; }
		string NotKeyword { get; }
		string AssignSign { get; }
	}
}
