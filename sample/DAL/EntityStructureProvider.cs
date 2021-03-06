﻿
//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using EF.Fake;

namespace DAL
{
	public class EntityStructureProvider : IEntityStructureProvider
	{
		private List<NavigationPropertyInfo> _navigationProperties;
		private List<EntityKeysInfo> _entityKeys;

		public List<NavigationPropertyInfo> NavigationProperties { get { return _navigationProperties; } }
		public List<EntityKeysInfo> EntityKeys { get { return _entityKeys; } }
		public EntityStructureProvider()
		{
			_navigationProperties = new List<NavigationPropertyInfo>();
			_entityKeys = new List<EntityKeysInfo>();
			_navigationProperties.Add(new NavigationPropertyInfo
			{
				FullName = "DbModel.ProductCustomer",
				From = typeof(Product),
				To = typeof(Customer),
				FK = new List<PropertyInfo>
				{
					typeof(Product).GetProperty("CustomerId"),
				},
				PK = new List<PropertyInfo>
				{
					typeof(Customer).GetProperty("CustomerId"),
				},
				FromNavigation = typeof(Product).GetProperty("Customer"),
				ToNavigation = typeof(Customer).GetProperty("Products"),
				FromMultiplicity = RelationshipMultiplicity.One,
				ToMultiplicity = RelationshipMultiplicity.Many
			});
			_navigationProperties.Add(new NavigationPropertyInfo
			{
				FullName = "DbModel.CustomerDepot",
				From = typeof(Customer),
				To = typeof(Depot),
				FK = new List<PropertyInfo>
				{
					typeof(Customer).GetProperty("DepotId"),
				},
				PK = new List<PropertyInfo>
				{
					typeof(Depot).GetProperty("DepotId"),
				},
				FromNavigation = typeof(Customer).GetProperty("Depot"),
				ToNavigation = typeof(Depot).GetProperty("Customers"),
				FromMultiplicity = RelationshipMultiplicity.One,
				ToMultiplicity = RelationshipMultiplicity.Many
			});
			_navigationProperties.Add(new NavigationPropertyInfo
			{
				FullName = "DbModel.ProductDepot",
				From = typeof(Product),
				To = typeof(Depot),
				FK = new List<PropertyInfo>
				{
					typeof(Product).GetProperty("DepotId"),
				},
				PK = new List<PropertyInfo>
				{
					typeof(Depot).GetProperty("DepotId"),
				},
				FromNavigation = typeof(Product).GetProperty("Depot"),
				ToNavigation = typeof(Depot).GetProperty("Products"),
				FromMultiplicity = RelationshipMultiplicity.One,
				ToMultiplicity = RelationshipMultiplicity.Many
			});
			_navigationProperties.Add(new NavigationPropertyInfo
			{
				FullName = "DbModel.DepotLocation",
				From = typeof(Depot),
				To = typeof(Location),
				FK = new List<PropertyInfo>
				{
					typeof(Depot).GetProperty("LocationLocationId"),
				},
				PK = new List<PropertyInfo>
				{
					typeof(Location).GetProperty("LocationId"),
				},
				FromNavigation = typeof(Depot).GetProperty("Location"),
				ToNavigation = typeof(Location).GetProperty("Depots"),
				FromMultiplicity = RelationshipMultiplicity.One,
				ToMultiplicity = RelationshipMultiplicity.Many
			});
			_entityKeys.Add(new EntityKeysInfo
			{
				Entity = typeof(Customer),
				Keys = new List<KeyPropertyInfo>
				{
					new KeyPropertyInfo
					{
						PK = typeof(Customer).GetProperty("CustomerId"),
						IsIdentity = true
					},
				}
			});
			_entityKeys.Add(new EntityKeysInfo
			{
				Entity = typeof(Depot),
				Keys = new List<KeyPropertyInfo>
				{
					new KeyPropertyInfo
					{
						PK = typeof(Depot).GetProperty("DepotId"),
						IsIdentity = true
					},
				}
			});
			_entityKeys.Add(new EntityKeysInfo
			{
				Entity = typeof(Location),
				Keys = new List<KeyPropertyInfo>
				{
					new KeyPropertyInfo
					{
						PK = typeof(Location).GetProperty("LocationId"),
						IsIdentity = true
					},
				}
			});
			_entityKeys.Add(new EntityKeysInfo
			{
				Entity = typeof(Product),
				Keys = new List<KeyPropertyInfo>
				{
					new KeyPropertyInfo
					{
						PK = typeof(Product).GetProperty("ProductId"),
						IsIdentity = true
					},
				}
			});
		}
	}
}
