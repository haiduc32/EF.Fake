using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake
{
	public class ProxyEntityInterceptor : IInterceptor
	{
		private FakeDbSetBase _fakeDbSet;

		public ProxyEntityInterceptor(FakeDbSetBase fakeDbSet)
		{
			_fakeDbSet = fakeDbSet;
		}

		public void Intercept(IInvocation invocation)
		{
			if (invocation.Method.Name.StartsWith("get_"))
			{
				string navigationName = invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4);
				object navigationValue = _fakeDbSet.GetNavigationValue(invocation.Proxy, navigationName);
				invocation.ReturnValue = navigationValue;
				//if (typeof(System.Collections.ICollection).IsAssignableFrom(invocation.Method.ReturnType))
				//{

				//}
				//else
				//{
				//	//TODO: implement it
				//	throw new NotImplementedException();
				//}
				//invocation.ReturnValue

				//object currentValue = invocation.ReturnValue;
				//object currentValue = invocation.MethodInvocationTarget.Invoke(invocation.InvocationTarget, new object[] {});
			}
			else if (invocation.Method.Name.StartsWith("set_"))
			{
				//setup for constructor
				//string navigationName = invocation.Method.Name.Substring(4, invocation.Method.Name.Length - 4);
				//if (invocation.Proxy.GetType().GetProperty(navigationName).va

				//TODO: set the setter somehow.
				//it's unclear to me how this should work for collections
				//throw new NotImplementedException();
			}
		}
	}
}
