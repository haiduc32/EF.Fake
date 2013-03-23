using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EF.Fake
{
	public class FakeContext
	{
		#region private fields

		private Dictionary<Type, FakeDbSetBase> _fakeDbSetDictionary;

		/// <summary>
		/// List of entities that were added and are waiting for the Save operation to be commited.
		/// Only after that will be available in the DbSets.
		/// </summary>
		private List<object> _pendingSaveEntities;

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

		internal FakeDbSetBase Get(Type type)
		{
			FakeDbSetBase fakeDbSetBase;
			if (!_fakeDbSetDictionary.TryGetValue(type, out fakeDbSetBase))
			{
				fakeDbSetBase = (FakeDbSetBase)typeof(FakeContext)
					.GetMethods().Single(x=> x.Name == "Get" && x.IsGenericMethod)
					.MakeGenericMethod(type)
					.Invoke(this, new object[] { });
			}

			return fakeDbSetBase;
		}

		public IEntityStructureProvider EntityStructureProvider { get; private set; }

		#endregion properties

		#region .ctor

		public FakeContext(IEntityStructureProvider entityStructureProvider)
		{
			_fakeDbSetDictionary = new Dictionary<Type, FakeDbSetBase>();
			_pendingSaveEntities = new List<object>();

			EntityStructureProvider = entityStructureProvider;
		}

		#endregion .ctor

		#region internal methods

		///// <summary>
		///// Adds an entity to the context. The entity is added to the corresponding FakeDbSet when SaveChanges is invoked on the context.
		///// </summary>
		///// <param name="entity"></param>
		//internal void AddEntity(object entity)
		//{
			
		//	//add the entity to a pending list

		//	throw new NotImplementedException();
		//}

		/// <summary>
		/// Adds an entity to the context and to the corresponding FakeDbSet.
		/// </summary>
		/// <param name="entity"></param>
		internal void AddAndAttachEntity(object entity)
		{
			//add the entity directly to the corresponding FakeDbSet.
			FakeDbSetBase dbSetObj = Get(entity.GetType());

			//if (!_fakeDbSetDictionary.TryGetValue(entity.GetType(), out dbSetObj))
			//{
			//	//create the fake db set
			//	dbSetObj = (FakeDbSetBase)typeof(FakeContext)
			//		.GetMethod("Get")
			//		.MakeGenericMethod(entity.GetType())
			//		.Invoke(this, new object[] { });
			//}
			dbSetObj.AddTestData(entity);
		}

		internal void AddPendingData(object data)
		{
			//TODO: do some verifications first
			//like if it's a proxy object, or if it's already in our pending list
			_pendingSaveEntities.Add(data);
		}

		#endregion internal methods

		#region public methods

		public void SaveChanges()
		{
			foreach (object data in _pendingSaveEntities)
			{
				AddAndAttachEntity(data);
			}

			_pendingSaveEntities.Clear();
		}

		public void AddTestData(object data)
		{
			AddAndAttachEntity(data);
		}

		public void AddTestData<T>(ICollection<T> dataCollection)
		{
			foreach (object data in dataCollection)
			{
				AddTestData(data);
			}
		}

		#endregion public methods
	}
}
