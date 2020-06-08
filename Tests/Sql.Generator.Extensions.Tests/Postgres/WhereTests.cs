using System.Collections.Generic;
using JetBrains.Annotations;
using Sql.Generator.Extensions.Dialects;
using Sql.Generator.Extensions.Interfaces;
using Xunit;
using Xunit.Sdk;

namespace Sql.Generator.Extensions.Tests.Postgres
{
	public class WhereTests
	{
		private class TestClass
		{
			public string StringProperty { get; set; }
			
			public int IntProperty { get; set; }
			
			public bool BoolProperty { get; set; }

			public string StringProperty2 { get; set; }
			
			public int IntProperty2 { get; set; }
		}
		
		[Fact]
		public void SimpleBool()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(c => true);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal("1 = 1", result.Statement);
			Assert.Empty(result.Params);
		}

		[Fact]
		public void ColumnBool()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(c => c.BoolProperty);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal(@"""c"".""BoolProperty"" = ""c"".""BoolProperty""", result.Statement);
			Assert.Empty(result.Params);
		}

		[Fact]
		public void OneEqOne()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(c => 1 == 1);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal("1 = 1", result.Statement);
			Assert.Empty(result.Params);
		}

		[Fact]
		public void PropertyEqualsConstant()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(c => c.IntProperty == 1);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal(@"""c"".""IntProperty"" = 1", result.Statement);
			Assert.Empty(result.Params);
		}

		[Fact]
		public void PropertyEqualsAutoParam()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(c => c.StringProperty == "123");

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal(@"""c"".""StringProperty"" = @p0", result.Statement);
			Assert.Collection(result.Params, pair => Assert.True(pair.Key == "p0" && pair.Value as string == "123"));
		}

		[Fact]
		public void AndOperator()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(c => c.StringProperty == "123" && c.BoolProperty);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal(@"(""c"".""StringProperty"" = @p0) AND (""c"".""BoolProperty"" = @p1)", result.Statement);
			Assert.Collection(
				result.Params,
					pair => Assert.True(pair.Key == "p0" && pair.Value as string == "123"),
					pair => Assert.True((bool) pair.Value && pair.Key == "p1")
				);
		}

		[Fact]
		public void ReuseParams()
		{
			var generator = new SqlGenerator<PostgresDialect>();
			var result = generator.Where<WhereTests.TestClass>(
				c => c.StringProperty == "123" && c.StringProperty2 == "123");

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Equal(@"(""c"".""StringProperty"" = @p0) AND (""c"".""StringProperty2"" = @p0)", result.Statement);
			Assert.Collection(
				result.Params,
				pair => Assert.True(pair.Key == "p0" && pair.Value as string == "123")
				);
		}

		[Fact]
		public void InTest()
		{
			var generator = new SqlGenerator<PostgresDialect>();

			var list = new List<int>() { 1, 2, 3 };
			
			var result = generator.Where<WhereTests.TestClass>(
				c => list.Contains(c.IntProperty)
				);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Empty(result.Params);
			Assert.Equal(@"""c"".""IntProperty"" IN (1, 2, 3)", result.Statement);
		}

		[Fact]
		public void EmptyInTest()
		{
			var generator = new SqlGenerator<PostgresDialect>();

			var list = new List<int>();

			var result = generator.Where<WhereTests.TestClass>(
				c => list.Contains(c.IntProperty)
				);

			Assert.NotNull(result);
			Assert.NotNull(result.Params);
			Assert.Empty(result.Params);
			Assert.Equal(@"1 = 1", result.Statement);
		}
	}
}
