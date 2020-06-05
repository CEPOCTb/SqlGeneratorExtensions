using System.Linq.Expressions;

namespace Sql.Generator.Extensions.Extensions
{
	public static class ExpressionHelpers
	{
		public static Expression UnwrapConvert(this Expression exp)
		{
			while (exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked)
			{
				exp = ((UnaryExpression) exp).Operand;
			}

			return exp;
		}

	}
}
