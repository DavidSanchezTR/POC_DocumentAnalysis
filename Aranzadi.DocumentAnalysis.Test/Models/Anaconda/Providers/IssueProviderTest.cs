using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
    public class IssueProviderTest
    {
        
        private Issue thefakedIssue;
        private FragmentAnaconda validFrag;

        [TestInitialize]
        public void TestInitialize()
        {
            this.validFrag = GetValidFragment();

            this.thefakedIssue = new Issue(
                new JudicialNotification(new Mock<IJudicialNotificationProvider>().Object),
                new Mock<IIssueProvider>().Object);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IssueProvider_FragmentIsNull_ThrowArgumentNullException()
        {
			new IssueProvider(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void IssueProvider_FragmentIdIsEmpty_ThrowArgumentException()
        {            
            validFrag.Id = " ";
            new IssueProvider(validFrag);
        }

		[TestMethod()]
		[ExpectedException(typeof(ArgumentException))]
		public void IssueProvider_FragmentTypeIsEmpty_ThrowArgumentException()
		{
			this.validFrag.Type = " ";
			new IssueProvider(this.validFrag);
		}

		[TestMethod()]
        public void IssueProvider_FragmentId_SetIdInBuilderOK()
        {
            this.validFrag.Id = Guid.NewGuid().ToString();
            var isu = new IssueProvider(this.validFrag);
            Assert.AreEqual(this.validFrag.Id, isu.Id);
        }        

        [TestMethod()]
        public void IssueProvider_Fragment_FragmentType_SetTitleInBuilderOK()
        {
            this.validFrag.Type = Guid.NewGuid().ToString();

            var isu = new IssueProvider(this.validFrag);
            Assert.AreEqual(this.validFrag.Type, isu.Title);
        }

		[TestMethod()]
		public void IssueProvider_TheNotiEnumsAreNotNullButEmpty()
		{
			var theBuilder = new IssueProvider(GetValidFragment());

			Assert.IsNotNull(theBuilder.GetRequirements(thefakedIssue));
			Assert.AreEqual(theBuilder.GetRequirements(thefakedIssue).Count(), 0);
			Assert.IsNotNull(theBuilder.GetTerms(thefakedIssue));
			Assert.AreEqual(theBuilder.GetTerms(thefakedIssue).Count(), 0);
		}

		[DataTestMethod]
        [DataRow(TermProvider.TERM_PLAZO, "El evento" + TermProvider.TERM_PLAZO, 1, DisplayName = TermProvider.TERM_PLAZO)]
        public void IssueProvider_GetTerms_Valid(string termTypeEntity, string eventType, int nDias)
        {
            var eventWithTerm = new EventAnaconda()
            {
                Type = eventType,
                Entities = new List<EntityAnaconda>() {
                        new EntityAnaconda()
                        {
                            Type = termTypeEntity,
                             Value = nDias + " días"
                        },
                        new EntityAnaconda()
                        {
                            Type = "Ruido_kk"
                        },
                        new EntityAnaconda()
                        {
                            Type = termTypeEntity,
                             Value = (nDias +1) + " días"
                        }
                    }
            };
            this.validFrag.Events = new List<EventAnaconda>() {
               eventWithTerm,
                new EventAnaconda()
                {
                    Type ="Ruido_kk"
                }
            };

            var isu = new IssueProvider(this.validFrag);

            Assert.That.AreEquivalent(
                eventWithTerm.Entities.Where(x => x.Type == termTypeEntity), 
                isu.GetTerms(thefakedIssue),
                (theEntity, theTerm) =>
                {
                    return theEntity.Value.Equals(theTerm.Days + " días");
                });
        }

		internal static FragmentAnaconda GetValidFragment()
		{
			return new FragmentAnaconda()
			{
				Id = Guid.NewGuid().ToString(),
				Type = Guid.NewGuid().ToString()
			};
		}

	}
}