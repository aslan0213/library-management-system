using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Abstractions.Repositories;
namespace Persistence.Repositories
{
	public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
	{
		public CategoryRepository(LibraryDbContext context) : base(context)
		{
		}
	}
}
