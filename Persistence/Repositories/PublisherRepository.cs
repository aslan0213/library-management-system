using System;
using System.Collections.Generic;
using System.Text;
using Abstractions.Repositories;
using Domain.Entities;
namespace Persistence.Repositories
{
	public class PublisherRepository : RepositoryBase<Publisher>, IPublisherRepository
	{
		public PublisherRepository(LibraryDbContext context) : base(context)
		{
		}
	}
}
