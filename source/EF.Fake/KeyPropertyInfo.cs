using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	public class KeyPropertyInfo
	{
		public PropertyInfo PK { get; set; }
		public bool IsIdentity { get; set; }
	}
}
