using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF.Fake
{
	public class FakeContext
	{
		Dictionary<Type, object> fakeDbSetDictionary;

		public FakeContext()
		{
			fakeDbSetDictionary = new Dictionary<Type, object>();
		}

		public FakeDbSet<T> Get<T>() where T : class
		{
			object fakeDbSetObj;
			if (fakeDbSetDictionary.TryGetValue(typeof(T), out fakeDbSetObj))
			{
				return (FakeDbSet<T>)fakeDbSetObj;
			}

			FakeDbSet<T> fakeDbSet = new FakeDbSet<T>();
			fakeDbSetDictionary.Add(typeof(T), fakeDbSet);

			return fakeDbSet;
		}

		public void AddTestData(object data)
		{
			throw new NotImplementedException();
		}

		public void AddTestData<T>(ICollection<T> data)
		{
			throw new NotImplementedException();
		}


	}
}
