using AutoMapper;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EF.Fake
{
	public class FakeDbSet<T> : FakeDbSetBase, IDbSet<T> where T : class
	{
		#region constants

		/// <summary>
		/// Start index for the entity identity field (if any). 77000 looks like an unlikely choice for 
		/// manually setup ids, that's why we'll use it.
		/// </summary>
		private const int IdentityMagicNumber = 77000  + 1;

		#endregion constants

		#region private fields

		private readonly HashSet<T> _data;
		private readonly Dictionary<T, Dictionary<string, object>> _navigationCollectionsDict;
		private readonly Dictionary<T, Dictionary<string, object>> _navigationItemsDict;
		private readonly HashSet<int> _addedDataHashcodes;
		private readonly IQueryable _query;


		private int _identity = IdentityMagicNumber;
		private List<KeyPropertyInfo> _keyProperties;
		private List<NavigationPropertyInfo> _navigationProperties;
		private FakeContext _fakeContext;

		#endregion private fields

		#region .ctor

		public FakeDbSet(FakeContext context)
		{
			//GetKeyProperties();
			_data = new HashSet<T>();
			_addedDataHashcodes = new HashSet<int>();
			_navigationCollectionsDict = new Dictionary<T, Dictionary<string, object>>();
			_navigationItemsDict = new Dictionary<T, Dictionary<string, object>>();
			_query = _data.AsQueryable();
			_fakeContext = context;

			_keyProperties = _fakeContext.EntityStructureProvider.EntityKeys
				.Single(x => x.Entity.Name == typeof(T).Name).Keys;

			_navigationProperties = _fakeContext.EntityStructureProvider.NavigationProperties
				.Where(x => x.From.Name == typeof(T).Name || x.To.Name == typeof(T).Name).ToList();

			//we'll create the AutoMapper mapping and exclude all virtual properties
			List<string> virtualProperties = typeof(T).GetProperties( BindingFlags.Public | BindingFlags.Instance |
				BindingFlags.GetProperty | BindingFlags.SetProperty)
				.Where(x=>x.GetGetMethod().IsVirtual).Select(x=>x.Name).ToList();
			IMappingExpression<T,T> mappingExpression = Mapper.CreateMap<T, T>();
			virtualProperties.ForEach(x => mappingExpression.ForMember(x, y => y.Ignore()));
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
			//first need to test if the item hasn't been already added, that might happen when we have circular
			//dependencies
			if (_addedDataHashcodes.Contains(item.GetHashCode()) ||
				_data.Contains(item))
			{
				//the item has been found, so it's probably a circular reference. other cases might be the
				//code trying to add twice the same item, but that must be treated with an error in the FakeContext
				
				//exit this method
				return;
			}

			// if PK are set skip this block
				//if the primary key is an identity, calculate the value
				//if (each) PK is a FK check if are different from 0
				//--> if == 0, check if we have for the navigation property any object, 
				//    and retrieve it's PK value - will be done when adding the related
				//    entities.
				//--> --> if navigation property is empty or null, throw some exception (meaningful please)
				//else throw some meaningful exception

			foreach (var keyInfo in _keyProperties)
			{
				//TODO: check if comparing with 0 is enough for al types (decimal for instance)
				if (!keyInfo.PK.GetValue(item).Equals(0)) continue;

				if (keyInfo.IsIdentity)
				{
					GenerateId((T)item);
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

				if (navigationPropertyValue is System.Collections.IEnumerable)
				{
					//we can't have collections in this case (or am I wrong?)
					throw new InvalidOperationException("Unexpected collection in navigation property");
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

			//identify navigation properties that are origination from another entity to this one
			var toNavigationProperties = _fakeContext.EntityStructureProvider.NavigationProperties
				.Where(x=>x.To.Name == typeof(T).Name && x.ToNavigation != null);
			foreach (NavigationPropertyInfo navProperty in toNavigationProperties)
			{
				object navigationPropertyValue = navProperty.ToNavigation.GetValue(item);
				if (navigationPropertyValue == null) continue;

				
				if (navigationPropertyValue is System.Collections.IEnumerable)
				{
					var enumerableNavigation = navigationPropertyValue as System.Collections.IEnumerable;
					foreach (object relatedItem in enumerableNavigation)
					{
						for (int i = 0; i < navProperty.PK.Count; i++)
						{
							//set the current item PK to the FK of the navigation property objects
							navProperty.FK[i].SetValue(relatedItem, navProperty.PK[i].GetValue(item));
						}
						_fakeContext.AddAndAttachEntity(relatedItem);
					}
				}
				else
				{
					//in this case we expect collection type
					throw new InvalidOperationException("Unexpected navigation property type.");
				}
			}

			//create proxy for our object
			//map item to the proxy object
			//add proxy object to the _data
			T proxyItem = new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy<T>(
				new ProxyEntityInterceptor(this));
			Mapper.Map((T)item, proxyItem);
			_addedDataHashcodes.Add(item.GetHashCode());
			_data.Add(proxyItem);

			//also create the dictionary object in navigationCollections but don't add the collections
			//as they will be used in GetNavigtionValue to figure out if they have been initialized already or not
			Dictionary<string, object> navigationCollections = new Dictionary<string,object>();
			_navigationCollectionsDict.Add(proxyItem, navigationCollections);

			Dictionary<string, object> navigationItems = new Dictionary<string, object>();
			_navigationItemsDict.Add(proxyItem, navigationItems);
		}

		/// <summary>
		/// Gets the value of a navigation property for the object of current Type.
		/// </summary>
		internal override object GetNavigationValue(object proxyObject, string navigationPropertyName)
		{
			//first check if we got a relevant collections object in our dictionaries
			object navigationCollectionObj;
			if (_navigationCollectionsDict[(T)proxyObject].TryGetValue(navigationPropertyName,
				out navigationCollectionObj))
			{
				return navigationCollectionObj;
			}

			object navigationItemObj;
			if (_navigationItemsDict[(T)proxyObject].TryGetValue(navigationPropertyName, out navigationItemObj))
			{
				return navigationItemObj;
			}
			
			//oh well.. bad luck, try to figure it out the hard way
			NavigationPropertyInfo fromNavigation = _navigationProperties
				.SingleOrDefault(x => x.FromNavigation.Name == navigationPropertyName);
			NavigationPropertyInfo toNavigation = null;
			RelationshipMultiplicity multiplicity;
			Type relatedObjectType;

			Dictionary<PropertyInfo, object> keys = new Dictionary<PropertyInfo, object>();

			if (fromNavigation != null)
			{
				multiplicity = fromNavigation.FromMultiplicity;
				relatedObjectType = fromNavigation.To;

				for (int i = 0; i < fromNavigation.FK.Count; i++)
				{
					keys.Add(fromNavigation.PK[i], fromNavigation.FK[i].GetValue(proxyObject));
				}
			}
			else
			{
				toNavigation = _navigationProperties
					.SingleOrDefault(x => x.ToNavigation != null && x.ToNavigation.Name == navigationPropertyName);

				if (toNavigation != null)
				{
					multiplicity = toNavigation.ToMultiplicity;
					relatedObjectType = toNavigation.From;

					for (int i = 0; i < toNavigation.FK.Count; i++)
					{
						keys.Add(toNavigation.FK[i], toNavigation.PK[i].GetValue(proxyObject));
					}
				}
				else
				{
					throw new InvalidOperationException("Ups! Something went wrong.. A navigation property isn't treated properly.");
				}
			}

			//now that we have the keys we can get a FakeDbSet for the related object type
			//and ask for all objects with the found keys
			FakeDbSetBase fakeDbSet = _fakeContext.Get(relatedObjectType);
			List<object> foundObjects = fakeDbSet.GetObjectsByKeys(keys);

			//now that we have all the data we must figure out what to return
			if (multiplicity == RelationshipMultiplicity.Many)
			{
				//for multiplicity Many we need to return a collection

				Type navigationCollectionType = typeof(NavigationProxyCollection<>);
				Type newNavigationCollectionType = navigationCollectionType.MakeGenericType(relatedObjectType);
				navigationCollectionObj = Activator.CreateInstance(newNavigationCollectionType,
					this, proxyObject);
				newNavigationCollectionType.GetMethod("AddCollection")
					.Invoke(navigationCollectionObj, new object[] { foundObjects });

				_navigationCollectionsDict[(T)proxyObject].Add(navigationPropertyName, navigationCollectionObj);
				return navigationCollectionObj;
			}
			else if (multiplicity == RelationshipMultiplicity.One)
			{
				//for multiplicity One we need to find one item that matches the search criteria
				if (foundObjects.Count != 1)
				{
					throw new InvalidOperationException("Navigation property did not find exactly one itme.");
				}

				object foundObject = foundObjects.Single();
				_navigationItemsDict[(T)proxyObject].Add(navigationPropertyName, foundObject);
				return foundObject;
			}
			else
			{
				//for multiplicity ZeroOrOne we need to find zero or one item that matches the search criteria
				if (foundObjects.Count != 1)
				{
					throw new InvalidOperationException("Navigation property found more than one item.");
				}

				object foundObject = foundObjects.SingleOrDefault();
				_navigationItemsDict[(T)proxyObject].Add(navigationPropertyName, foundObject);
				return foundObject;
			}
		}

		internal override List<object> GetObjectsByKeys(Dictionary<PropertyInfo, object> keys)
		{
			IEnumerable<T> query = (IEnumerable<T>)_data;
			foreach (var pair in keys)
			{
				query = query.Where(x => pair.Key.GetValue(x).Equals(pair.Value));
			}
			return query.Select(x => (object)x).ToList();
		}

		#endregion internal methods

		#region IDbSet<T> interface implementation

		public virtual T Find(params object[] keyValues)
		{
			//TODO: implement
			throw new NotImplementedException();
			//if (keyValues.Length != _keyProperties.Count)
			//	throw new ArgumentException("Incorrect number of keys passed to find method");

			//IQueryable<T> keyQuery = this.AsQueryable<T>();
			//for (int i = 0; i < keyValues.Length; i++)
			//{
			//	var x = i; // nested linq
			//	keyQuery = keyQuery.Where(entity => _keyProperties[x].GetValue(entity, null).Equals(keyValues[x]));
			//}

			//return keyQuery.SingleOrDefault();
		}

		public T Add(T item)
		{
			//redirect to the _fakeContext so it can dirrect it can have a overall control
			_fakeContext.AddPendingData(item);
			return item;
		}

		public T Remove(T item)
		{
			throw new NotImplementedException();
			//TODO: more logic should be involved in removing an item..
			//_data.Remove(item);
			//return item;
		}

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
			throw new NotImplementedException("This method is not fully implemented and using in UT will cause unexpected results.");
			//return Activator.CreateInstance<T>();
		}

		public ObservableCollection<T> Local
		{
			get
			{
				throw new NotImplementedException();
				//return new ObservableCollection<T>(_data);
			}
		}

		public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
		{
			return Activator.CreateInstance<TDerivedEntity>();
		}

		#endregion interface implementation

		#region private methods

		private void GenerateId(T entity)
		{

			// If non-composite integer key
			if (_keyProperties.Count == 1 && _keyProperties[0].IsIdentity &&
				(_keyProperties[0].PK.PropertyType == typeof(int) ||
				_keyProperties[0].PK.PropertyType == typeof(decimal)))
			{
				//TODO: check if it indeed works with decimals, but implicit casting should kick in
				_keyProperties[0].PK.SetValue(entity, _identity++, null);
			}
		}

		#endregion private methods
	}
}
