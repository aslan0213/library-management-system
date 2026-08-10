using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Abstractions.Specifications
{
	public interface ISpecification<T>
	{
		Expression<Func<T, bool>>? Criteria { get; }
		List<Expression<Func<T, object>>> Includes { get; }
		Expression<Func<T, bool>>? OrderBy { get; }
		Expression<Func<T, bool>>? OrderByDescending { get; }
	}
}
