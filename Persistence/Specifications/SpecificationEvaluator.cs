using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Specifications;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Specifications
{
	public static class SpecificationEvaluator
	{
		public static IQueryable<T> Apply<T>(IQueryable<T> source, ISpecification<T> specification) where T : class
		{
			var query = source;

			if(specification.Criteria != null)
			{
				query = query.Where(specification.Criteria);
			}

			foreach(var include in specification.Includes) //also can use Aggregate function (it will be in one line)
			{
				query = query.Include(include);
			}

			if (specification.OrderBy != null)
			{
				query = query.OrderBy(specification.OrderBy);
			}

			if(specification.OrderByDescending != null)
			{
				query = query.OrderByDescending(specification.OrderByDescending);
			}

			return query;
		}
	}
}
