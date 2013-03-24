using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Repository : IRepository
    {
		private DbModelContainer _container;

		public Repository()
		{
			_container = new DbModelContainer();
		}

		public IDbSet<Customer> Customers { get { return _container.Customers; } }

		public IDbSet<Product> Products { get { return _container.Products; } }

		public IDbSet<Depot> Depots { get { return _container.Depots; } }

		public IDbSet<Location> Locations { get { return _container.Locations; } }

		public int SaveChanges()
		{
			return _container.SaveChanges();
		}
    }
}
