using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
    public class PersonsProviderTest
    {
        [TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void PersonsProvider_EntityPersonNull_ThrowArgumentNullException()
        {
            EntityAnaconda personEntity = null;
            new PersonsProvider(personEntity);
		}

		[TestMethod]
		public void PersonsProvider_DefaultValues()
		{
			EntityAnaconda personEntity = new EntityAnaconda();
			var personProvider = new PersonsProvider(personEntity);
			Assert.AreEqual(personProvider.DNI, "");
			Assert.AreEqual(personProvider.Name, "");
			Assert.AreEqual(personProvider.Lawyer, "");
			Assert.AreEqual(personProvider.Attorney, "");
		}

		[TestMethod]
		public void PersonsProvider_WithChildrensNull()
		{
			string actorNameValue = Guid.NewGuid().ToString();
			EntityAnaconda personEntity = new EntityAnaconda()
			{
				Type = PersonsProvider.ENTITY_ACTOR,
				Value = actorNameValue
			};
			var personProvider = new PersonsProvider(personEntity);
			Assert.AreEqual(personProvider.DNI, "");
			Assert.AreEqual(personProvider.Name, actorNameValue);
			Assert.AreEqual(personProvider.Lawyer, "");
			Assert.AreEqual(personProvider.Attorney, "");
		}

		[TestMethod]
		public void PersonsProvider_WithChildrensActor()
		{
			string actorNameValue = Guid.NewGuid().ToString();
			string actorDNIValue = Guid.NewGuid().ToString();
			string actorLawyerValue = Guid.NewGuid().ToString();
			string actorAttorneyValue = Guid.NewGuid().ToString();
			EntityAnaconda personEntity = new EntityAnaconda();
			List<EntityAnaconda> childrenList = new List<EntityAnaconda>();
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_ACTOR, Value = actorNameValue });
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_DNI, Value = actorDNIValue });
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_LAWYER, Value = actorLawyerValue });
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_ATTORNEY, Value = actorAttorneyValue });
			personEntity.Children = childrenList;

			var personProvider = new PersonsProvider(personEntity);
			Assert.AreEqual(personProvider.DNI, actorDNIValue);
			Assert.AreEqual(personProvider.Name, actorNameValue);
			Assert.AreEqual(personProvider.Lawyer, actorLawyerValue);
			Assert.AreEqual(personProvider.Attorney, actorAttorneyValue);
		}

		[TestMethod]
		public void PersonsProvider_WithChildrensDemandado()
		{
			string demandadoNameValue = Guid.NewGuid().ToString();
			string demandadoDNIValue = Guid.NewGuid().ToString();
			string demandadoLawyerValue = Guid.NewGuid().ToString();
			string demandadoAttorneyValue = Guid.NewGuid().ToString();
			EntityAnaconda personEntity = new EntityAnaconda();
			List<EntityAnaconda> childrenList = new List<EntityAnaconda>();
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_DEMANDADO, Value = demandadoNameValue });
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_DNI, Value = demandadoDNIValue });
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_LAWYER, Value = demandadoLawyerValue });
			childrenList.Add(new EntityAnaconda() { Type = PersonsProvider.ENTITY_ATTORNEY, Value = demandadoAttorneyValue });
			personEntity.Children = childrenList;

			var personProvider = new PersonsProvider(personEntity);
			Assert.AreEqual(personProvider.DNI, demandadoDNIValue);
			Assert.AreEqual(personProvider.Name, demandadoNameValue);
			Assert.AreEqual(personProvider.Lawyer, demandadoLawyerValue);
			Assert.AreEqual(personProvider.Attorney, demandadoAttorneyValue);
		}

	}
}