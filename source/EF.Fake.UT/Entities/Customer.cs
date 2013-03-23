using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake.UT.Entities
{
	public class Customer
	{
		public int CustomerId { get; set; }

		public string Name { get; set; }

		public int DepotId { get; set; }

		public virtual  ICollection<Product> Products { get; set; }

		public virtual Depot Depot { get; set; }

		public Customer()
		{
			Products = new HashSet<Product>();
		}
	}
}
