using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake.UT.Entities
{
	public class Depot
	{
		public int DepotId { get; set; }

		public int LocationId { get; set; }

		public string Name { get; set; }

		public virtual ICollection<Product> Products { get; set; }

		public virtual Location Location { get; set; }

		public Depot()
		{
			Products = new HashSet<Product>();
		}
	}
}
