namespace PK.Sql.Generator.Extensions.Dialects
{
	public class SqlServerDialect : BaseDialect
	{
		#region Overrides of BaseDialect

		/// <inheritdoc />
		public override string NameEscapeOpen => "[";

		/// <inheritdoc />
		public override string NameEscapeClose => "]";

		#endregion
	}
}
