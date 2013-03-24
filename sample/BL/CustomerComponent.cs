using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CustomerComponent
    {
		private IRepository _repository;

		public CustomerComponent(IRepository repository)
		{
			_repository = repository;
		}

		public int FindDepotIdForCustoemr(int customerId)
		{
			Customer customer = _repository.Customers.Single(x => x.CustomerId == customerId);
			return customer.Depot.DepotId;
		}
    }
}
