using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	public class EntityKeysInfo
	{
		public Type Entity { get; set; }
		public List<KeyPropertyInfo> Keys { get; set; }
	}
}
