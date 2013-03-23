using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EF.Fake.UT.Entities;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Linq;

namespace EF.Fake.UT
{
	[TestClass]
	public class BasicOperationsTests
	{
		#region private fields

		private FakeContext _fakeContext;

		#endregion private fields

		#region test setup

		[TestInitialize]
		public void TestSetup()
		{
			_fakeContext = new FakeContext(new TestEntityStructureProvider());
		}

		#endregion test setup

		#region tests

		[TestMethod]
		public void AddEntites_SaveChanges_CheckAllEntitiesInRelevantDbSets()
		{
			Customer customer = CreateGraphOfDataWithCircularReferences();

			_fakeContext.Get<Customer>().Add(customer);
			_fakeContext.SaveChanges();

			//expectancies
			//the product was added and has a valid primary key
			CheckAllEntitiesInRelevantDbSets();
		}

		[TestMethod]
		public void AddTestData_ComplexData_CheckAllEntitiesInRelevantDbSets()
		{
			//create a graph of object with circular references
			Customer customer = CreateGraphOfDataWithCircularReferences();

			_fakeContext.AddTestData(customer);

			//check expectancies

			CheckAllEntitiesInRelevantDbSets();
		}

		[TestMethod]
		public void AddTestData_ExpectNavigationProperitesWorking()
		{
			//create a graph of object with circular references
			Customer customer = CreateGraphOfDataWithCircularReferences();

			_fakeContext.AddTestData(customer);

			Customer foundCustomer = _fakeContext.Get<Customer>().Single();

			Assert.IsNotNull(foundCustomer.Products);
			Assert.AreEqual(1, foundCustomer.Products.Count);

			Product foundProduct = _fakeContext.Get<Product>().Single();
			Assert.IsNotNull(foundProduct.Customer);
			//assert once again the same property to check that the caching works
			Assert.IsNotNull(foundProduct.Customer);
		}

		[TestMethod]
		public void AddTestData_ExpectZeroOrOneNavigationsPropertiesWork()
		{
			//TODO: need to write this test
			Assert.Inconclusive("Test is not implemented");
		}

		public void AddTestData_ExpectQueriesWork()
		{
			//create a graph of object with circular references
			Customer customer = CreateGraphOfDataWithCircularReferences();

			_fakeContext.AddTestData(customer);

			var result = from c in _fakeContext.Get<Customer>()
						 join p in _fakeContext.Get<Product>() on c.CustomerId equals p.CustomerId
						 select new
						 {
							 c,
							 p
						 };
			//assert that c and p are set in the result
			Assert.IsNotNull(result.Single().c);
			Assert.IsNotNull(result.Single().p);
		}

		#endregion tests

		#region helper methods

		/// <summary>
		/// Creates a graph of objects with circular references.
		/// </summary>
		private Customer CreateGraphOfDataWithCircularReferences()
		{
			Depot depot = new Depot
			{
				Name = "Depot 1"
			};

			Product product = new Product
			{
				ProductCode = 4324,
				Depot = depot
			};

			Customer customer = new Customer
			{
				Name = "Some Customer",
				Depot = depot
			};
			customer.Products.Add(product);

			return customer;
		}

		/// <summary>
		/// Validates that all entities created by CreateGraphOfDataWithCircularReferences() can be retrieved
		/// in the relevant DbSets.
		/// </summary>
		private void CheckAllEntitiesInRelevantDbSets()
		{
			//the product was added and has a valid primary key
			Product foundProduct = _fakeContext.Get<Product>().SingleOrDefault();

			Assert.IsNotNull(foundProduct);
			Assert.AreNotEqual(0, foundProduct.ProductId);
			Assert.AreEqual(4324, foundProduct.ProductCode);
			//also check the FK
			Assert.AreNotEqual(0, foundProduct.CustomerId);
			Assert.AreNotEqual(0, foundProduct.DepotId);

			//the depot was added and has a valid primary key
			Depot foundDepot = _fakeContext.Get<Depot>().SingleOrDefault();
			Assert.IsNotNull(foundDepot);
			Assert.AreNotEqual(0, foundDepot.DepotId);

			//the customer was added and has a valid primary key
			Customer foundCustomer = _fakeContext.Get<Customer>().SingleOrDefault();
			Assert.IsNotNull(foundCustomer);
			Assert.AreNotEqual(0, foundCustomer.CustomerId);
			Assert.AreNotEqual(0, foundCustomer.DepotId);
		}

		#endregion helper methods
	}
}
