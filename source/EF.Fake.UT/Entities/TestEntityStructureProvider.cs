using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EF.Fake.UT.Entities
{
	class TestEntityStructureProvider : IEntityStructureProvider
	{
		List<NavigationPropertyInfo> _navigationProperties;
		List<EntityKeysInfo> _entityKeys;

		public TestEntityStructureProvider()
		{
			_navigationProperties = new List<NavigationPropertyInfo>
				{
					new NavigationPropertyInfo
					{
						FK = new List<PropertyInfo> { typeof(Customer).GetProperty("DepotId") },
						PK = new List<PropertyInfo> { typeof(Depot).GetProperty("DepotId") },
						From = typeof(Customer),
						To = typeof(Depot),
						FromNavigation = typeof(Customer).GetProperty("Depot"),
						ToNavigation = null,
						FromMultiplicity = RelationshipMultiplicity.One,
						ToMultiplicity = RelationshipMultiplicity.Many
					},
					new NavigationPropertyInfo
					{
						FK = new List<PropertyInfo> { typeof(Product).GetProperty("CustomerId") },
						PK = new List<PropertyInfo> { typeof(Customer).GetProperty("CustomerId") },
						From = typeof(Product),
						To = typeof(Customer),
						FromNavigation = typeof(Product).GetProperty("Customer"),
						ToNavigation = typeof(Customer).GetProperty("Products"),
						FromMultiplicity = RelationshipMultiplicity.One,
						ToMultiplicity = RelationshipMultiplicity.Many
					},
					new NavigationPropertyInfo
					{
						FK = new List<PropertyInfo> { typeof(Product).GetProperty("DepotId") },
						PK = new List<PropertyInfo> { typeof(Depot).GetProperty("DepotId") },
						From = typeof(Product),
						To = typeof(Depot),
						FromNavigation = typeof(Product).GetProperty("Depot"),
						ToNavigation = typeof(Depot).GetProperty("Products"),
						FromMultiplicity = RelationshipMultiplicity.One,
						ToMultiplicity = RelationshipMultiplicity.Many
					},
					new NavigationPropertyInfo
					{
						FK = new List<PropertyInfo> { typeof(Depot).GetProperty("LocationId") },
						PK = new List<PropertyInfo> { typeof(Location).GetProperty("LocationId") },
						From = typeof(Depot),
						To = typeof(Location),
						FromNavigation = typeof(Depot).GetProperty("Location"),
						ToNavigation = null,
						FromMultiplicity = RelationshipMultiplicity.One,
						ToMultiplicity = RelationshipMultiplicity.Many
					}
				};

			_entityKeys = new List<EntityKeysInfo>
				{
					new EntityKeysInfo
					{
						Entity = typeof(Customer),
						Keys = new List<KeyPropertyInfo>
						{
							new KeyPropertyInfo
							{
								IsIdentity = true,
								PK = typeof(Customer).GetProperty("CustomerId")
							}
						}
					},
					new EntityKeysInfo
					{
						Entity = typeof(Depot),
						Keys = new List<KeyPropertyInfo>
						{
							new KeyPropertyInfo
							{
								IsIdentity = true,
								PK = typeof(Depot).GetProperty("DepotId")
							}
						}
					},
					new EntityKeysInfo
					{
						Entity = typeof(Product),
						Keys = new List<KeyPropertyInfo>
						{
							new KeyPropertyInfo
							{
								IsIdentity = true,
								PK = typeof(Product).GetProperty("ProductId")
							}
						}
					},
					new EntityKeysInfo
					{
						Entity = typeof(Location),
						Keys = new List<KeyPropertyInfo>
						{
							new KeyPropertyInfo
							{
								IsIdentity = true,
								PK = typeof(Location).GetProperty("LocationId")
							}
						}
					}
				};
		}


		public List<NavigationPropertyInfo> NavigationProperties
		{
			get
			{
				return _navigationProperties;
			}
		}

		public List<EntityKeysInfo> EntityKeys
		{
			get { return _entityKeys; }
		}
	}
}
