namespace Sql.Generator.Extensions.Dialects
{
	public class PostgresDialect : BaseDialect
	{
		#region Overrides of BaseDialect

		/// <inheritdoc />
		public override bool NotEqualsSupported => true;

		#endregion
	}
}
