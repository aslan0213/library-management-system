using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Abstractions.Specifications
{
	public abstract class SpecificationBase<T> : ISpecification<T>
	{
		public Expression<Func<T, bool>>? Criteria { get; private set; }

		public List<Expression<Func<T, object>>> Includes { get; } = new();

		public Expression<Func<T, bool>>? OrderBy { get; private set; }

		public Expression<Func<T, bool>>? OrderByDescending { get; private set; }	

		//protected void AddCriteria(Expression<Func<T, bool>> criteria)
		//{
		//	Criteria = Criteria is null ?  criteria : CombinedWithAnd(Criteria, criteria);
		//}

		protected void AddInclude(Expression<Func<T, object>> includeExpression)
		{
			Includes.Add(includeExpression);
		}
		private void AddOrderBy(Expression<Func<T, bool>> orderByExpression)
		{
			OrderBy = orderByExpression;
		}
		private void AddOrderByDescending(Expression<Func<T, bool>> orderByDescendingExpression)
		{
			OrderByDescending = orderByDescendingExpression;
		}
		//private static Expression<Func<T, bool>> CombinedWithAnd(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
		//{
		//	var parameter = Expression.Parameter(typeof(T));
		//	var leftBody = ReplaceParameter(left.Body, left.Parameters[0], parameter);
		//	var rightBody = ReplaceParameter(right.Body, right.Parameters[0], parameter);
		//	var combined = Expression.AndAlso(leftBody, rightBody);
		//	return Expression.Lambda<Func<T, bool>>(combined, parameter);
		//}
		//private static Expression ReplaceParameter(Expression body, ParameterExpression oldParam, ParameterExpression newParam)
		//{
		//	return new ParameterReplacer(oldParam, newParam).Visit(body);
		//}
		//private class ParameterReplacer : ExpressionVisitor
		//{
		//	private readonly ParameterExpression _oldParam;
		//	private readonly ParameterExpression _newParam;
		//	public ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam)
		//	{
		//		_oldParam = oldParam;
		//		_newParam = newParam;
		//	}
		//	protected override Expression VisitParameter(ParameterExpression node)
		//	{
		//		if (node == _oldParam)
		//			return _newParam;
		//		return base.VisitParameter(node);
		//	}

		//}
	}


}
