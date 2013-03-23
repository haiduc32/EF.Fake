using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF.Fake
{
	public class FakeContext
	{
		#region private fields

		Dictionary<Type, FakeDbSetBase> _fakeDbSetDictionary;

		#endregion private fields

		#region properties

		public FakeDbSet<T> Get<T>() where T : class
		{
			FakeDbSetBase fakeDbSetBase;
			if (_fakeDbSetDictionary.TryGetValue(typeof(T), out fakeDbSetBase))
			{
				return (FakeDbSet<T>)fakeDbSetBase;
			}

			FakeDbSet<T> fakeDbSet = new FakeDbSet<T>(this);
			_fakeDbSetDictionary.Add(typeof(T), fakeDbSet);

			return fakeDbSet;
		}

		public IEntityStructureProvider EntityStructureProvider { get; private set; }

		#endregion properties

		#region .ctor

		public FakeContext(IEntityStructureProvider entityStructureProvider)
		{
			_fakeDbSetDictionary = new Dictionary<Type, FakeDbSetBase>();

			EntityStructureProvider = entityStructureProvider;
		}

		#endregion .ctor

		#region internal methods

		/// <summary>
		/// Adds an entity to the context. The entity is added to the corresponding FakeDbSet when SaveChanges is invoked on the context.
		/// </summary>
		/// <param name="entity"></param>
		internal void AddEntity(object entity)
		{
			
			//add the entity to a pending list

			throw new NotImplementedException();
		}

		/// <summary>
		/// Adds an entity to the context and to the corresponding FakeDbSet.
		/// </summary>
		/// <param name="entity"></param>
		internal void AddAndAttachEntity(object entity)
		{
			//add the entity directly to the corresponding FakeDbSet.
			FakeDbSetBase dbSetObj;

			if (!_fakeDbSetDictionary.TryGetValue(entity.GetType(), out dbSetObj))
			{
				//create the fake db set
				dbSetObj = (FakeDbSetBase)typeof(FakeContext)
					.GetMethod("Get")
					.MakeGenericMethod(entity.GetType())
					.Invoke(this, new object[] { });
			}
			dbSetObj.AddTestData(entity);
		}

		#endregion internal methods

		#region public methods

		public void SaveChanges()
		{
			throw new NotImplementedException();
		}

		public void AddTestData(object data)
		{
			throw new NotImplementedException();
		}

		public void AddTestData<T>(ICollection<T> data)
		{
			throw new NotImplementedException();
		}

		#endregion public methods
	}
}
