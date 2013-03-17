using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	public class NavigationPropertyInfo
	{
		public string FullName { get; set; }
		public Type From { get; set; }
		public Type To { get; set; }
		public List<PropertyInfo> FK { get; set; }
		public List<PropertyInfo> PK { get; set; }
		public PropertyInfo FromNavigation { get; set; }
		public PropertyInfo ToNavigation { get; set; }
		public RelationshipMultiplicity FromMultiplicity { get; set; }
		public RelationshipMultiplicity ToMultiplicity { get; set; }
	}
}
