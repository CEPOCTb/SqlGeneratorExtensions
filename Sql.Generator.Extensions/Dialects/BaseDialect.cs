using System.Text;
using Sql.Generator.Extensions.Interfaces;

namespace Sql.Generator.Extensions.Dialects
{
	public abstract class BaseDialect : ISqlDialect
	{
		#region Implementation of ISqlDialect

		/// <inheritdoc />
		public virtual string GetEscapedTableName(string table, string schema = null, string alias = null)
		{
			var sb = new StringBuilder();
			if (!string.IsNullOrWhiteSpace(schema))
			{
				sb.Append(NameEscapeOpen)
					.Append(schema)
					.Append(NameEscapeClose)
					.Append('.');
			}

			sb.Append(NameEscapeOpen)
				.Append(table)
				.Append(NameEscapeClose);

			if (!string.IsNullOrWhiteSpace(alias))
			{
				sb.Append(" AS ")
					.Append(NameEscapeOpen)
					.Append(alias)
					.Append(NameEscapeClose);
			}

			return sb.ToString();
		}

		/// <inheritdoc />
		public virtual string GetEscapedParameterName(string parameterName, string alias = null)
		{

			var sb = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(alias))
			{
				sb.Append(NameEscapeOpen)
					.Append(alias)
					.Append(NameEscapeClose)
					.Append('.');
			}

			return sb.Append(NameEscapeOpen)
				.Append(parameterName)
				.Append(NameEscapeClose)
				.ToString();
		}


		/// <inheritdoc />
		public virtual string GetParameterReference(string parameterName) =>
			new StringBuilder(ParameterPrefix)
				.Append(parameterName)
				.ToString();

		/// <inheritdoc />
		public abstract bool NotEqualsSupported { get; }

		/// <inheritdoc />
		public virtual string ParameterPrefix => "@";

		/// <inheritdoc />
		public virtual string NameEscapeOpen => "\"";

		/// <inheritdoc />
		public virtual string NameEscapeClose => "\"";

		/// <inheritdoc />
		public virtual string OpenBracket => "(";

		/// <inheritdoc />
		public virtual string CloseBracket => ")";

		/// <inheritdoc />
		public virtual string LogicalAnd => "AND";

		/// <inheritdoc />
		public virtual string LogicalOr => "OR";

		/// <inheritdoc />
		public virtual string LogicalEquals => "=";

		/// <inheritdoc />
		public string LogicalNotEquals => "!=";

		/// <inheritdoc />
		public virtual string PlusSign => "+";

		/// <inheritdoc />
		public virtual string MinusSign => "-";

		/// <inheritdoc />
		public virtual string MultiplySign => "*";

		/// <inheritdoc />
		public virtual string DivideSign => "/";

		/// <inheritdoc />
		public virtual string LikeKeyword => "LIKE";

		/// <inheritdoc />
		public virtual string NotKeyword => "NOT";

		/// <inheritdoc />
		public virtual string AssignSign => "=";

		#endregion
	}
}
