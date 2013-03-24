using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DAL
{
	public interface IRepository
	{
		IDbSet<Customer> Customers { get; }

		IDbSet<Product> Products { get; }

		IDbSet<Depot> Depots { get; }

		IDbSet<Location> Locations { get; }

		int SaveChanges();
	}
}
