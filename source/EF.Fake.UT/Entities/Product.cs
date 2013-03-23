using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake.UT.Entities
{
	public class Product
	{
		public int ProductId { get; set; }

		public int ProductCode { get; set; }

		public int CustomerId { get; set; }

		public int DepotId { get; set; }

		public virtual Customer Customer { get; set; }

		public virtual Depot Depot { get; set; }

		public Product()
		{
		}
	}
}
