using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	public abstract class FakeDbSetBase
	{
		internal abstract void AddTestData(object item);

		internal abstract object GetNavigationValue(object proxyObject, string navigationPropertyName);

		internal abstract List<object> GetObjectsByKeys(Dictionary<PropertyInfo, object> keys);
	}
}
