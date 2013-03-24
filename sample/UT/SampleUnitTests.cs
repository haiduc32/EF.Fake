using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EF.Fake;
using DAL;
using BL;

namespace UT
{
	[TestClass]
	public class SampleUnitTests
	{
		private FakeContext _fakeContext;
		private FakeRepository _fakeRepository;
		private CustomerComponent _customerComponent;

		[TestInitialize]
		public void TestInitialize()
		{
			//initialize fakes
			_fakeContext = new FakeContext(new EntityStructureProvider());
			_fakeRepository = new FakeRepository(_fakeContext);

			//initialize the tested class
			_customerComponent = new CustomerComponent(_fakeRepository);
		}

		[TestMethod]
		public void TestMethod()
		{
			//setup test data
			Customer customer = new Customer()
			{
				Depot = new Depot()
			};

			_fakeContext.AddTestData(customer);

			//call the tested method
			int result = _customerComponent.FindDepotIdForCustoemr(customer.CustomerId);

			//verify expectancies
			Assert.AreNotEqual(0, result);
			Assert.AreEqual(customer.DepotId, result);
		}
	}
}
