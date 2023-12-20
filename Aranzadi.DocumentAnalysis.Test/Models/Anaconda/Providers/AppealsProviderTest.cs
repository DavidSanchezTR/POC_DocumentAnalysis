using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Newtonsoft.Json;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
    public class AppealsProviderTest
    {

        [TestMethod()]
        public void AppealsProvider_ExtendJudicialNotificationProvider()
        {
            var appel = new AppealsProvider(new DocumentAnalysisAnaconda());
            Assert.IsTrue(appel is JudicialNotificationProvider);
        }

		[TestMethod()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void AppealsProvider_DocumentAnalysisAnacondaIsNull_ThrowArgumentNullException()
		{
			DocumentAnalysisAnaconda documentAnalysisAnaconda = null;
			var appel = new AppealsProvider(documentAnalysisAnaconda);
		}

		[TestMethod()]
		public void AppealsProvider_DefaultValues_OK()
		{
			var appeal = new AppealsProvider(new DocumentAnalysisAnaconda());
			Assert.IsNotNull(appeal.InformationNotification);
			Assert.IsNotNull(appeal.Map);
			Assert.IsNotNull(appeal.Process);
			Assert.IsNull(appeal.InitialAmount);
			Assert.IsNull(appeal.RecognizedAmount);
		}

		[TestMethod()]
		public void AppealsProvider_NullFragment_IssuesEquals0()
		{
			var documentAnalysisAnaconda = new DocumentAnalysisAnaconda()
			{
				Fragments = null
			};
			var appealProvider = new AppealsProvider(documentAnalysisAnaconda);
			var appealsNotification = new AppealsNotification(new Mock<IAppealsProvider>().Object);
			Assert.AreEqual(0, appealProvider.GetIssues(appealsNotification).Count());
		}

		[TestMethod()]
		public void AppealsProvider_GetIssues()
		{
			List<Issue> theIssues = new List<Issue>();
			var b = new Mock<IAppealsProvider>();
			b.Setup(p => p.GetIssues(It.IsAny<AppealsNotification>())).Returns(theIssues);
			AppealsNotification appealsNotification = new AppealsNotification(b.Object);
			Assert.AreSame(theIssues, appealsNotification.Issues);
		}

		[TestMethod()]
		public void AppealsProvider_Process()
		{
			var pro = new Process(new ProcessProvider(new DocumentAnalysisAnaconda()));
			var b = new Mock<IAppealsProvider>();
			b.Setup(p => p.Process).Returns(pro);
			AppealsNotification appealsNotification = new AppealsNotification(b.Object);
			Assert.AreSame(pro, appealsNotification.ProceduralInformation);
		}

		[TestMethod()]
		public void AppealsProvider_InformationNotification()
		{
			var inf = new InformationNotification(new InformationProvider(new DocumentAnalysisAnaconda()));
			var b = new Mock<IAppealsProvider>();
			b.Setup(p => p.InformationNotification).Returns(inf);
			AppealsNotification appealsNotification = new AppealsNotification(b.Object);
			Assert.AreSame(inf, appealsNotification.Properties);

		}

		[TestMethod]
		public void Serialize()
		{
			DocumentAnalysisAnaconda ana = GetFullDocumentAnalysisAnaconda();
			var buil = new AppealsProvider(ana);
			var noti = new AppealsNotification(buil);


			var s = JsonConvert.SerializeObject(noti, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});


			var noti2 = JsonConvert.DeserializeObject<AppealsNotification>(s);

			var s2 = JsonConvert.SerializeObject(noti2, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});

			Assert.AreEqual(s, s2);
		}

		private static DocumentAnalysisAnaconda GetFullDocumentAnalysisAnaconda()
		{
			return new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>{
						new EntityAnaconda() {
							Type = ProcessProvider.ENTITY_MATERIA,
							Value = Guid.NewGuid().ToString() + "_Materia"
						},
						new EntityAnaconda() {
							Type = ProcessProvider.ENTITY_PROCEDURE,
							Value = Guid.NewGuid().ToString() + "_Procedure"
						},
						new EntityAnaconda() {
							Type = ProcessProvider.ENTITY_ORGANO,
							Value = Guid.NewGuid().ToString() + "_Organo"
						},
						new EntityAnaconda() {
							Type = ProcessProvider.ENTITY_NIG,
							Value = Guid.NewGuid().ToString() + "_NIG"
						},
						new EntityAnaconda()
						{
							Type = ProcessProvider.ENTITY_ACTIVEPART_ACTOR,
							Value = string.Empty,
							Children = new List<EntityAnaconda>
							{
								new EntityAnaconda()
								{
									Type= PersonsProvider.ENTITY_NAME,
									Value = Guid.NewGuid().ToString() + "_NAME"
								},
								new EntityAnaconda()
								{
									Type= PersonsProvider.ENTITY_DNI,
									Value = Guid.NewGuid().ToString() + "_DNI"
								}
							}
						},
						new EntityAnaconda()
						{
							Type = InformationProvider.ENTITY_FECHA_NOTIFICACION,
							Value = DateTime.UtcNow.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT)
						}
					},
				Fragments = new List<FragmentAnaconda>
					{
						new FragmentAnaconda()
						{
							Id = Guid.NewGuid().ToString() + "_IdFragmento",
							Type = Guid.NewGuid().ToString() + "_TipoFragmento",
							Events = new List<EventAnaconda>
							{
								new EventAnaconda()
								{
									IsMain = true,
									Type = Guid.NewGuid().ToString() + "_Event Tipe",
									Entities = new List<EntityAnaconda>
									{
										new EntityAnaconda()
										{
											Type = TermProvider.TERM_PLAZO,
											Value = "1 día"
										}
									}
								},
								new EventAnaconda()
								{
									IsMain = false,
									Type = AppealsProvider.EVENT_ADMISION_A_TRAMITE,
									Entities = new List<EntityAnaconda>
									{
										new EntityAnaconda()
										{
											Type = AppealsProvider.ENTITY_CUANTIA_INICIAL,
											Value = "945,54 Euros"
										},
										new EntityAnaconda()
										{
											Type = AppealsProvider.ENTITY_CUANTIA_ADMITIDA,
											Value = "Indeterminada"
										}
									}
								}
							}
						}
					}
			};
		}

	}
}