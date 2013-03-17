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
	public class FakeDbSet<T> : IDbSet<T> where T : class
	{
		#region private fields

		private readonly HashSet<T> _data;
		private readonly IQueryable _query;
		private int _identity = 1;
		private List<PropertyInfo> _keyProperties;

		#endregion private fields

		#region .ctor

		public FakeDbSet(IEnumerable<T> startData = null)
		{
			GetKeyProperties();
			_data = (startData != null ? new HashSet<T>(startData) : new HashSet<T>());
			_query = _data.AsQueryable();
			//Mapper.CreateMap<T, T>();
			//Mapper.CreateMap<UserDepotRole, UserDepotRole>();
		}

		#endregion .ctor

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
			GenerateId(item);
			_data.Add(item);
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
			_keyProperties = new List<PropertyInfo>();
			PropertyInfo[] properties = typeof(T).GetProperties();
			foreach (PropertyInfo property in properties)
			{
				foreach (Attribute attribute in property.GetCustomAttributes(true))
				{
					if (attribute is KeyAttribute)
					{
						_keyProperties.Add(property);
					}
				}
			}
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
