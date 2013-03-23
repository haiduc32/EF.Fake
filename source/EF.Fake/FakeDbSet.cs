//using AutoMapper;
//using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EF.Fake
{
	public abstract class FakeDbSetBase
	{
		internal abstract void AddTestData(object item);
	}

	public class FakeDbSet<T> : FakeDbSetBase, IDbSet<T> where T : class
	{
		#region private fields

		private readonly HashSet<T> _data;
		private readonly IQueryable _query;
		private int _identity = 1;
		private List<PropertyInfo> _keyProperties;
		private FakeContext _fakeContext;

		#endregion private fields

		#region .ctor

		public FakeDbSet(FakeContext context)
		{
			GetKeyProperties();
			_data = new HashSet<T>();
			_query = _data.AsQueryable();
			_fakeContext = context;
			//Mapper.CreateMap<T, T>();
			//Mapper.CreateMap<UserDepotRole, UserDepotRole>();
		}

		#endregion .ctor

		#region internal methods

		/// <summary>
		/// Internal methods to be used by the FakeContext, when SaveChanges is invoked and pending entities are saved.
		/// Also is used for adding the data as prerequisites for the test.
		/// </summary>
		/// <param name="item"></param>
		internal override void AddTestData(object item)
		{
			// if PK are set skip this block
				//if the primary key is an identity, calculate the value
				//if (each) PK is a FK check if are different from 0
				//--> if == 0, check if we have for the navigation property any object, and retrieve it's PK value - will be done when adding the related
				//    entities.
				//--> --> if navigation property is empty or null, throw some exception (meaningful please)
				//else throw some meaningful exception

			EntityKeysInfo entityKeysInfo = _fakeContext.EntityStructureProvider.EntityKeys.Single(x => x.Entity.Name == typeof(T).Name);
			foreach (var keyInfo in entityKeysInfo.Keys)
			{
				//TODO: check if comparing with 0 is enough for al types (decimal for instance)
				if (!keyInfo.PK.GetValue(item).Equals(0)) continue;

				if (keyInfo.IsIdentity)
				{
					//TODO: calculate the next value, again, figure out how adapt it to any integer type
					throw new NotImplementedException();
				}
				else
				{
					//the last option is the NavigationProperties, the local PK must be FK to another Entity
					//TODO: try to cache the retrieval of navigation properties, extract to some method?

					//get the navigation property if any
					NavigationPropertyInfo navigationProperty = _fakeContext.EntityStructureProvider.NavigationProperties
						.SingleOrDefault(x => x.From.Name == typeof(T).Name && x.FK.Any(y=>y.Name == keyInfo.PK.Name));

					//check if the related entity is set, if not then throw an error, because we need a primary key value
					if (navigationProperty.FromNavigation.GetValue(item).Equals(null))
					{
						//the navigation property is not set, so throw some meaningful error
						throw new InvalidOperationException("The primary key value could not be identified/calculated" +
							" for the added entity, or one of the entites from the added graph.");
					}
				}
			}

			//foreach navigation property that has any data
			//order from navigations first, because they might setup the primary key on this item
			//if the navigation is from this type of entity
			//--> AddAndAttachTestData(), then verify the PK value and assign to our objects FK property
			//if the navigation is to this type of entity
			//--> set the FK values on the related objects and AddAndAttachTestData()

			//identify navigation properties that are originating from this entity to another entity
			var fromNavigationProperties = _fakeContext.EntityStructureProvider.NavigationProperties
						.Where(x => x.From.Name == typeof(T).Name && x.FromNavigation != null);
			foreach (NavigationPropertyInfo navProperty in fromNavigationProperties)
			{
				object navigationPropertyValue = navProperty.FromNavigation.GetValue(item);
				if (navigationPropertyValue == null) continue;

				var enumerableNavigation = navigationPropertyValue as System.Collections.IEnumerable;
				if (enumerableNavigation != null)
				{
					//TODO: implement logic for collections
				}
				else
				{
					_fakeContext.AddAndAttachEntity(navigationPropertyValue);
					//check the added entity PKs and assign to our FKs
					for (int i = 0; i < navProperty.PK.Count; i++)
					{
						navProperty.FK[i].SetValue(item, navProperty.PK[i].GetValue(navigationPropertyValue));
					}
				}
			}

			

			//create proxy for out object
			//map item to the proxy object
			//add proxy object to the _data
		}

		#endregion internal methods

		#region interface implementation

		public virtual T Find(params object[] keyValues)
		{
			if (keyValues.Length != _keyProperties.Count)
				throw new ArgumentException("Incorrect number of keys passed to find method");

			IQueryable<T> keyQuery = this.AsQueryable<T>();
			for (int i = 0; i < keyValues.Length; i++)
			{
				var x = i; // nested linq
				keyQuery = keyQuery.Where(entity => _keyProperties[x].GetValue(entity, null).Equals(keyValues[x]));
			}

			return keyQuery.SingleOrDefault();
		}

		public T Add(T item)
		{
			//GenerateId(item);
			//_data.Add(item);
			_fakeContext.AddTestData(item);
			return item;
		}

		public T Remove(T item)
		{
			_data.Remove(item);
			return item;
		}

		//public class Interceptor : IInterceptor
		//{
		//	public void Intercept(IInvocation invocation)
		//	{
		//		if (invocation.Method.Name.StartsWith("get_"))
		//		{
		//			object currentValue = invocation.ReturnValue;
		//			//object currentValue = invocation.MethodInvocationTarget.Invoke(invocation.InvocationTarget, new object[] {});
		//		}
		//	}
		//}

		public T Attach(T item)
		{
			//we don't want this method to be used for now
			throw new NotImplementedException();

			//I'm not sure if this is the best place to do it, but for now will use this method for attaching entities
			//in the test methods

			//T proxyItem = new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy<T>(new Interceptor());
			//Mapper.Map<T, T>(item, proxyItem);

			//_data.Add(proxyItem);
			//return proxyItem;

			_data.Add(item);
			return item;
		}

		//public void Detach(T item)
		//{
		//	_data.Remove(item);
		//}

		Type IQueryable.ElementType
		{
			get { return _query.ElementType; }
		}

		Expression IQueryable.Expression
		{
			get { return _query.Expression; }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return _query.Provider; }
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public T Create()
		{
			throw new NotImplementedException("This method is not fully implemented and using it UT will cause unexpected results.");
			return Activator.CreateInstance<T>();
		}

		public ObservableCollection<T> Local
		{
			get
			{
				return new ObservableCollection<T>(_data);
			}
		}

		public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
		{
			return Activator.CreateInstance<TDerivedEntity>();
		}

		#endregion interface implementation

		#region private methods

		private void GetKeyProperties()
		{
			//TODO: this needs re implementing

			//_keyProperties = new List<PropertyInfo>();
			//PropertyInfo[] properties = typeof(T).GetProperties();
			//foreach (PropertyInfo property in properties)
			//{
			//	foreach (Attribute attribute in property.GetCustomAttributes(true))
			//	{
			//		if (attribute is KeyAttribute)
			//		{
			//			_keyProperties.Add(property);
			//		}
			//	}
			//}
		}

		private void GenerateId(T entity)
		{
			// If non-composite integer key
			if (_keyProperties.Count == 1 && _keyProperties[0].PropertyType == typeof(Int32))
				_keyProperties[0].SetValue(entity, _identity++, null);
		}

		#endregion private methods
	}
}
