using Sql.Generator.Extensions.Dialects;
using Xunit;

namespace Sql.Generator.Extensions.Tests.Postgres
{
	public class AssignTests
	{
		[Fact]
		public void Test1()
		{
			var generator = new SqlGenerator<PostgresDialect>();

			bool b = true;
			
			var result = generator.Update(
				null,
				"test",
				() => new
					{
						BoolProperty = b && true,
						IntProperty = 5,
						StringProperty = "1234"
					},
				c => c.IntProperty == 2);

			Assert.NotNull(result);
			Assert.NotNull(result.Statement);
		}

	}
}
