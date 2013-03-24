using DAL;
using EF.Fake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT
{
	class FakeRepository : IRepository
	{
		private FakeContext _fakeContext;

		public FakeRepository(FakeContext fakeContext)
		{
			_fakeContext = fakeContext;
		}

		public System.Data.Entity.IDbSet<Customer> Customers
		{
			get { return _fakeContext.Get<Customer>(); }
		}

		public System.Data.Entity.IDbSet<Product> Products
		{
			get { return _fakeContext.Get<Product>(); }
		}

		public System.Data.Entity.IDbSet<Depot> Depots
		{
			get { return _fakeContext.Get<Depot>(); }
		}

		public System.Data.Entity.IDbSet<Location> Locations
		{
			get { return _fakeContext.Get<Location>(); }
		}

		public int SaveChanges()
		{
			_fakeContext.SaveChanges();
			return 0;
		}
	}
}
