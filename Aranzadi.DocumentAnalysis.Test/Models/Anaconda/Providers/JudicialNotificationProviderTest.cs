using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
	public class JudicialNotificationProviderTest
	{
		[TestMethod()]
		[ExpectedException(typeof(ArgumentNullException))]
		public void JudicialNotificationBuilder_NullAnacondaDTO()
		{
			new JudicialNotificationProvider(null);
		}

		[TestMethod()]
		public void Issues_OneFragment()
		{
			var ana = new DocumentAnalysisAnaconda()
			{
				Fragments = new List<FragmentAnaconda> {
					IssueProviderTest.GetValidFragment()
				}
			};

			var not = new JudicialNotificationProvider(ana);
			var judNot = new JudicialNotification(new Mock<IJudicialNotificationProvider>().Object);
			var issues = not.GetIssues(judNot);
			Assert.AreEqual(1, issues.Count());

			Assert.That.AreEquivalent(not.GetIssues(judNot), ana.Fragments, (x, y) => { return x.Id.Equals(y.Id); });
		}

		[TestMethod()]
		public void Issues_NullFragment()
		{
			var ana = new DocumentAnalysisAnaconda()
			{
				Fragments = null
			};
			var not = new JudicialNotificationProvider(ana);
			var judNot = new JudicialNotification(new Mock<IJudicialNotificationProvider>().Object);
			Assert.AreEqual(0, not.GetIssues(judNot).Count());
		}

		[TestMethod]
		public void ProcessNotNull()
		{
			Assert.IsNotNull(new JudicialNotificationProvider(new DocumentAnalysisAnaconda()).Process);

		}

		[TestMethod]
		public void PropertiesNotNull()
		{
			Assert.IsNotNull(new JudicialNotificationProvider(new DocumentAnalysisAnaconda())
				.InformationNotification);
		}

		[TestMethod()]
		public void JudicialNotification_Builder_GetIssues()
		{
			List<Issue> theIssues = new List<Issue>();
			var b = new Mock<IJudicialNotificationProvider>();
			b.Setup(p => p.GetIssues(It.IsAny<JudicialNotification>())).Returns(theIssues);
			JudicialNotification not = new JudicialNotification(b.Object);
			Assert.AreSame(theIssues, not.Issues);
		}

		[TestMethod()]
		public void JudicialNotification_Builder_Process()
		{
			var pro = new Process(new ProcessProvider(new DocumentAnalysisAnaconda()));
			var b = new Mock<IJudicialNotificationProvider>();
			b.Setup(p => p.Process).Returns(pro);
			JudicialNotification not = new JudicialNotification(b.Object);
			Assert.AreSame(pro, not.ProceduralInformation);
		}

		[TestMethod()]
		public void JudicialNotification_Builder_InformationNotification()
		{
			var inf = new InformationNotification(new InformationProvider(new DocumentAnalysisAnaconda()));
			var b = new Mock<IJudicialNotificationProvider>();
			b.Setup(p => p.InformationNotification).Returns(inf);
			JudicialNotification not = new JudicialNotification(b.Object);
			Assert.AreSame(inf, not.Properties);

		}


		[TestMethod]
		public void Serialize()
		{
			DocumentAnalysisAnaconda ana = GetFullDocumentAnalysisAnaconda();
			var buil = new JudicialNotificationProvider(ana);
			var noti = new JudicialNotification(buil);


			var s = JsonConvert.SerializeObject(noti, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});


			var noti2 = JsonConvert.DeserializeObject<JudicialNotification>(s);

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
								}
							}
						}
					}
			};
		}

		[Ignore]
		[TestMethod]
		public void JudicialNotification_Builder_Sample0()
		{
			string test = @"{""$id"":""1"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.AppealsNotification, Aranzadi.DocumentAnalysis.Messaging"",""InitialAmount"":null,""RecognizedAmount"":null,""Properties"":{""$id"":""2"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.InformationNotification, Aranzadi.DocumentAnalysis.Messaging"",""NotificationDate"":""2023-11-20T13:30:00Z"",""NotificationDateDescription"":""""},""ProceduralInformation"":{""$id"":""3"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Process, Aranzadi.DocumentAnalysis.Messaging"",""Court"":""juzgado sample"",""Procedure"":""procedimiento sample"",""Judgement"":""numero autos sample"",""NIG"":""NIG sample"",""Topic"":""materia sample"",""ActivePart"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.PersonsInvolved, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""4"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.PersonsInvolved, Aranzadi.DocumentAnalysis.Messaging"",""Name"":""nombre2 sample"",""DNI"":""22222222R"",""Lawyer"":""abogado2 sample"",""Attorney"":""procurador2 sample""}]},""PassivePart"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.PersonsInvolved, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""5"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.PersonsInvolved, Aranzadi.DocumentAnalysis.Messaging"",""Name"":""nombre1 sample"",""DNI"":""11111111P"",""Lawyer"":""abogado1 sample"",""Attorney"":""procurador1 sample""}]}},""Issues"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Issue, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""6"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Issue, Aranzadi.DocumentAnalysis.Messaging"",""Notification"":{""$ref"":""1""},""Id"":""Tema1_Id"",""Title"":""Tema1"",""Terms"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""7"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""6""},""Days"":5,""PeriodDescription"":""dias"",""Text"":""Plazo1 sample""},{""$id"":""8"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""6""},""Days"":10,""PeriodDescription"":""dias"",""Text"":""Plazo2 sample""}]},""Requirements"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""9"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""6""},""RequirementDate"":""2023-11-21T13:30:00Z"",""RequirementDateDescription"":""Requerimiento1 sample""},{""$id"":""10"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""6""},""RequirementDate"":""2023-11-22T13:30:00Z"",""RequirementDateDescription"":""Requerimiento2 sample""}]}},{""$id"":""11"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Issue, Aranzadi.DocumentAnalysis.Messaging"",""Notification"":{""$ref"":""1""},""Id"":""Tema2_Id"",""Title"":""Tema2"",""Terms"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""12"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""11""},""Days"":6,""PeriodDescription"":""dias"",""Text"":""Plazo3 sample""},{""$id"":""13"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""11""},""Days"":20,""PeriodDescription"":""dias"",""Text"":""Plazo4 sample""}]},""Requirements"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[]}},{""$id"":""14"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Issue, Aranzadi.DocumentAnalysis.Messaging"",""Notification"":{""$ref"":""1""},""Id"":""Tema3_Id"",""Title"":""Tema3"",""Terms"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Term, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[]},""Requirements"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""15"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""14""},""RequirementDate"":""2023-11-21T13:30:00Z"",""RequirementDateDescription"":""Requerimiento3 sample""},{""$id"":""16"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.Requirement, Aranzadi.DocumentAnalysis.Messaging"",""TheIssue"":{""$ref"":""14""},""RequirementDate"":""2023-11-22T13:30:00Z"",""RequirementDateDescription"":""Requerimiento4 sample""}]}}]},""Exploration"":{""$id"":""17"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel, Aranzadi.DocumentAnalysis.Messaging"",""Title"":""Desconocido"",""Depth"":0,""Identificator"":""notificacion_prueba"",""SubLevels"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""18"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel, Aranzadi.DocumentAnalysis.Messaging"",""Title"":""Decreto"",""Depth"":1,""Identificator"":""notificacion_prueba_1"",""SubLevels"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""19"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel, Aranzadi.DocumentAnalysis.Messaging"",""Title"":""loquesea"",""Depth"":2,""Identificator"":null,""SubLevels"":null,""Properties"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""20"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging"",""Key"":""Plazo"",""Value"":""10 dias"",""PropertyType"":2}]},""IsMain"":false,""Container"":null}]},""Properties"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[]},""IsMain"":false,""Container"":""815 23 SIN NOMBRE DEL DEMANDANTE_CD.zip""}]},""Properties"":{""$type"":""System.Collections.Generic.List`1[[Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging]], System.Private.CoreLib"",""$values"":[{""$id"":""21"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging"",""Key"":""Tipo de procedimiento"",""Value"":""Juicio ordinario"",""PropertyType"":1},{""$id"":""22"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging"",""Key"":""Órgano judicial"",""Value"":""JDO.PRIMERA INSTANCIA N.5 DE PONEERRADA"",""PropertyType"":2},{""$id"":""23"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging"",""Key"":""Número de procedimiento"",""Value"":""0000815/2023"",""PropertyType"":2},{""$id"":""24"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging"",""Key"":""Actor"",""Value"":null,""PropertyType"":2},{""$id"":""26"",""$type"":""Aranzadi.DocumentAnalysis.Messaging.Model.Response.ExplorationLevel+Property, Aranzadi.DocumentAnalysis.Messaging"",""Key"":""Demandado"",""Value"":null,""PropertyType"":2}]},""IsMain"":false,""Container"":null}}";
			test = @"{""$id"":""1"",""InitialAmount"":null,""RecognizedAmount"":null,""Properties"":{""$id"":""2"",""NotificationDate"":""2023-11-20T13:30:00Z"",""NotificationDateDescription"":""""},""ProceduralInformation"":{""$id"":""3"",""Court"":""juzgado sample"",""Procedure"":""procedimiento sample"",""Judgement"":""numero autos sample"",""NIG"":""NIG sample"",""Topic"":""materia sample"",""ActivePart"":[{""$id"":""4"",""Name"":""nombre2 sample"",""DNI"":""22222222R"",""Lawyer"":""abogado2 sample"",""Attorney"":""procurador2 sample""}],""PassivePart"":[{""$id"":""5"",""Name"":""nombre1 sample"",""DNI"":""11111111P"",""Lawyer"":""abogado1 sample"",""Attorney"":""procurador1 sample""}]},""Issues"":[{""$id"":""6"",""Notification"":{""$ref"":""1""},""Id"":""Tema1_Id"",""Title"":""Tema1"",""Terms"":[{""$id"":""7"",""TheIssue"":{""$ref"":""6""},""Days"":5,""PeriodDescription"":""dias"",""Text"":""Plazo1 sample""},{""$id"":""8"",""TheIssue"":{""$ref"":""6""},""Days"":10,""PeriodDescription"":""dias"",""Text"":""Plazo2 sample""}],""Requirements"":[{""$id"":""9"",""TheIssue"":{""$ref"":""6""},""RequirementDate"":""2023-11-21T13:30:00Z"",""RequirementDateDescription"":""Requerimiento1 sample""},{""$id"":""10"",""TheIssue"":{""$ref"":""6""},""RequirementDate"":""2023-11-22T13:30:00Z"",""RequirementDateDescription"":""Requerimiento2 sample""}]},{""$id"":""11"",""Notification"":{""$ref"":""1""},""Id"":""Tema2_Id"",""Title"":""Tema2"",""Terms"":[{""$id"":""12"",""TheIssue"":{""$ref"":""11""},""Days"":6,""PeriodDescription"":""dias"",""Text"":""Plazo3 sample""},{""$id"":""13"",""TheIssue"":{""$ref"":""11""},""Days"":20,""PeriodDescription"":""dias"",""Text"":""Plazo4 sample""}],""Requirements"":[]},{""$id"":""14"",""Notification"":{""$ref"":""1""},""Id"":""Tema3_Id"",""Title"":""Tema3"",""Terms"":[],""Requirements"":[{""$id"":""15"",""TheIssue"":{""$ref"":""14""},""RequirementDate"":""2023-11-21T13:30:00Z"",""RequirementDateDescription"":""Requerimiento3 sample""},{""$id"":""16"",""TheIssue"":{""$ref"":""14""},""RequirementDate"":""2023-11-22T13:30:00Z"",""RequirementDateDescription"":""Requerimiento4 sample""}]}],""Exploration"":{""$id"":""17"",""Title"":""Desconocido"",""Depth"":0,""Identificator"":""notificacion_prueba"",""SubLevels"":[{""$id"":""18"",""Title"":""Decreto"",""Depth"":1,""Identificator"":""notificacion_prueba_1"",""SubLevels"":[{""$id"":""19"",""Title"":""loquesea"",""Depth"":2,""Identificator"":null,""SubLevels"":null,""Properties"":[{""$id"":""20"",""Key"":""Plazo"",""Value"":""10 dias"",""PropertyType"":2}],""IsMain"":false,""Container"":null}],""Properties"":[],""IsMain"":false,""Container"":""815 23 SIN NOMBRE DEL DEMANDANTE_CD.zip""}],""Properties"":[{""$id"":""21"",""Key"":""Tipo de procedimiento"",""Value"":""Juicio ordinario"",""PropertyType"":1},{""$id"":""22"",""Key"":""Órgano judicial"",""Value"":""JDO.PRIMERA INSTANCIA N.5 DE PONEERRADA"",""PropertyType"":2},{""$id"":""23"",""Key"":""Número de procedimiento"",""Value"":""0000815/2023"",""PropertyType"":2},{""$id"":""24"",""Key"":""Actor"",""Value"":null,""PropertyType"":2},{""$id"":""25"",""Key"":""Demandado"",""Value"":null,""PropertyType"":2}],""IsMain"":false,""Container"":null}}";

			JudicialNotification tesJudicial = JsonConvert.DeserializeObject<JudicialNotification>(test, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});

			string judi2 = JsonConvert.SerializeObject(tesJudicial, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});


			string a = @"{""id"":""notificacion_prueba"",""type"":""Desconocido"",""fragments"":[{""id"":""notificacion_prueba_1"",""htmlElementId"":null,""containerName"":""815 23 SIN NOMBRE DEL DEMANDANTE_CD.zip"",""type"":""Decreto"",""categories"":[],""entities"":[],""events"":[{""type"":""Admisión a trámite"",""categories"":[],""entities"":[{""type"":""Cuantía inicial"",""value"":""941,5 Euros"",""mentions"":[]},{""type"":""Cuantía admitida"",""value"":""Indeterminada"",""mentions"":[]},{""type"":""Plazo"",""value"":""39 dias"",""mentions"":[]},{""type"":""Fecha señalamiento"",""value"":""2022-11-28T13:30:00Z"",""mentions"":[]}],""isMain"":false},{""type"":""loquesea"",""categories"":[],""entities"":[{""type"":""Plazo"",""value"":""10 dias"",""mentions"":[]}],""isMain"":false}],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""Juicio ordinario""}],""entities"":[{""type"":""Órgano judicial"",""value"":""JDO.PRIMERA INSTANCIA N.5 DE PONEERRADA"",""mentions"":[]},{""type"":""Número de procedimiento"",""value"":""0000815/2023"",""mentions"":[]},{""type"":""Actor"",""value"":null,""children"":[{""type"":""Nombre"",""value"":""Pedro Perez Lopez"",""mentions"":[]},{""type"":""Nif"",""value"":""506776254-V"",""mentions"":[]}]},{""type"":""Actor"",""value"":null,""children"":[{""type"":""Nombre"",""value"":""Luisa Jimenez Lopez"",""mentions"":[]},{""type"":""Nif"",""value"":""545555678-E"",""mentions"":[]}]},{""type"":""Demandado"",""value"":null,""children"":[{""type"":""Nombre"",""value"":""Felipe el demandado"",""mentions"":[]},{""type"":""Nif"",""value"":""12345678-Z"",""mentions"":[]}]},{""type"":""Demandado"",""value"":null,""children"":[{""type"":""Nombre"",""value"":""Carlos el demandado"",""mentions"":[]},{""type"":""Nif"",""value"":""000555678-H"",""mentions"":[]}]},{""type"":""Actor"",""value"":""Actor sin children""},{""type"":""Demandado"",""value"":""Demandado sin children""}],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/notificacion_prueba_1/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
			DocumentAnalysisAnaconda analysis = JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(a);

			var judicialNotificationprovider = new JudicialNotificationProvider(analysis);
			var judicialNotification = new JudicialNotification(judicialNotificationprovider);

			string judi = JsonConvert.SerializeObject(judicialNotification, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});

			var appealNotificationprovider = new AppealsProvider(analysis);
			var appealNotification = new AppealsNotification(appealNotificationprovider);

			string appeal = JsonConvert.SerializeObject(appealNotification, new JsonSerializerSettings()
			{
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				Formatting = Formatting.Indented,
			});

			Assert.IsTrue(true);
		}

		[Ignore]
		[TestMethod]
		public void JudicialNotification_Builder_Sample1()
		{
			string a = @"{""id"":""839a5dc3-b20f-4e02-89a7-a6ee0a850e7c"",""type"":""Desconocido"",""fragments"":[{""id"":""839a5dc3-b20f-4e02-89a7-a6ee0a850e7c_1"",""htmlElementId"":null,""containerName"":""0202441001120220000036784(1).pdf"",""type"":""Decreto"",""categories"":[{""type"":""Tipo de procedimiento"",""label"":""VRB""}],""entities"":[{""type"":""Procedimiento"",""value"":""VRB - 0000122/2022"",""mentions"":[]}],""events"":[],""isMain"":true,""manualReviewRequired"":false,""analysisAllowed"":true},{""id"":""839a5dc3-b20f-4e02-89a7-a6ee0a850e7c_2"",""htmlElementId"":null,""containerName"":""2022_0000122_JVB_202210538178535_20221128132659(1).pdf"",""type"":""Cedula"",""categories"":[],""entities"":[{""type"":""Órgano"",""value"":""JDO. 1A INSTANCIA E INSTRUCCIÓN 1 de Casas-Ibáñez, Albacete"",""mentions"":[]},{""type"":""Nº de autos"",""value"":""0000122/2022"",""mentions"":[]},{""type"":""Fecha notificación"",""value"":""2022-11-28T13:30:00Z"",""mentions"":[]},{""type"":""Asunto"",""value"":""Comunicación del Acontecimiento 73: DECRETO APROBANDO TASACION  COSTAS ART 244.3 LEC"",""mentions"":[]},{""type"":""NIG"",""value"":""0202441120220000147"",""mentions"":[]},{""type"":""Nombre del profesional"",""value"":""SERNA ESPINOSA, MANUEL "",""mentions"":[]},{""type"":""Nº de colegiado"",""value"":""106"",""mentions"":[]},{""type"":""Colegio profesional"",""value"":""Ilustre Colegio de Procuradores de Albacete"",""mentions"":[]}],""events"":[],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":true}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""VRB""}],""entities"":[{""type"":""Órgano"",""value"":""JDO. 1A INSTANCIA E INSTRUCCIÓN 1 de Casas-Ibáñez, Albacete"",""mentions"":[]},{""type"":""Nº de autos"",""value"":""0000122/2022"",""mentions"":[]},{""type"":""Procedimiento"",""value"":""VRB - 0000122/2022"",""mentions"":[]},{""type"":""Fecha notificación"",""value"":""2022-11-28T13:30:00Z"",""mentions"":[]},{""type"":""Asunto"",""value"":""Comunicación del Acontecimiento 73: DECRETO APROBANDO TASACION  COSTAS ART 244.3 LEC"",""mentions"":[]},{""type"":""NIG"",""value"":""0202441120220000147"",""mentions"":[]},{""type"":""Nombre del profesional"",""value"":""SERNA ESPINOSA, MANUEL "",""mentions"":[]},{""type"":""Nº de colegiado"",""value"":""106"",""mentions"":[]},{""type"":""Colegio profesional"",""value"":""Ilustre Colegio de Procuradores de Albacete"",""mentions"":[]}],""links"":[{""rel"":""htmlOutput"",""href"":""{FE-DOCUMENTAPI-URL}/documents/839a5dc3-b20f-4e02-89a7-a6ee0a850e7c/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
			DocumentAnalysisAnaconda analysis = JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(a);

			var judicialNotificationprovider = new JudicialNotificationProvider(analysis);
			var judicialNotification = new JudicialNotification(judicialNotificationprovider);

			Assert.IsTrue(true);
		}

		[Ignore]
		[TestMethod]
		public void JudicialNotification_Builder_Sample()
		{
			string a = @"{""id"":""notificacion_prueba"",""type"":""Desconocido"",""fragments"":[{""id"":""notificacion_prueba_1"",""htmlElementId"":null,""containerName"":""815 23 SIN NOMBRE DEL DEMANDANTE_CD.zip"",""type"":""Decreto"",""categories"":[],""entities"":[],""events"":[{""type"":""Admisión a trámite"",""categories"":[],""entities"":[{""type"":""Cuantía inicial"",""value"":""Indeterminada"",""mentions"":[]},{""type"":""Cuantía admitida"",""value"":""Indeterminada"",""mentions"":[]}],""isMain"":false}],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""Juicio ordinario""}],""entities"":[{""type"":""Órgano judicial"",""value"":""JDO.PRIMERA INSTANCIA N.5 DE PONEERRADA"",""mentions"":[]},{""type"":""Número de procedimiento"",""value"":""0000815/2023"",""mentions"":[]},{""type"":""Actor"",""value"":null,""children"":[{""type"":""Nombre"",""value"":""Pedro Perez Lopez"",""mentions"":[]},{""type"":""Nif"",""value"":""506776254-V"",""mentions"":[]}]},{""type"":""Actor"",""value"":null,""children"":[{""type"":""Nombre"",""value"":""Luisa Jimenez Lopez"",""mentions"":[]},{""type"":""Nif"",""value"":""545555678-E"",""mentions"":[]}]}],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/notificacion_prueba_1/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
			DocumentAnalysisAnaconda analysis = JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(a);

			var judicialNotificationprovider = new JudicialNotificationProvider(analysis);
			var judicialNotification = new JudicialNotification(judicialNotificationprovider);

			Assert.IsTrue(true);
		}

		[Ignore]
		[TestMethod]
		public void JudicialNotification_Builder_SampleEmpty()
		{
			string a = @"{""id"":""85b8373c-929f-4add-b69c-266e48e9e574"",""type"":""Desconocido"",""fragments"":[{""id"":""85b8373c-929f-4add-b69c-266e48e9e574_1"",""htmlElementId"":null,""containerName"":""7435386430.pdf"",""type"":""Desconocido"",""categories"":[],""entities"":[],""events"":[],""isMain"":true,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[],""entities"":[],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/85b8373c-929f-4add-b69c-266e48e9e574/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
			DocumentAnalysisAnaconda analysis = JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(a);

			var judicialNotificationprovider = new JudicialNotificationProvider(analysis);
			var judicialNotification = new JudicialNotification(judicialNotificationprovider);

			Assert.IsTrue(true);
		}

		[Ignore]
		[TestMethod]
		public void JudicialNotification_Builder_Sample2()
		{
			string a = @"{""id"":""6b088558-6bea-468f-8124-acf5c5fc13d2"",""type"":""Desconocido"",""fragments"":[{""id"":""6b088558-6bea-468f-8124-acf5c5fc13d2_01"",""htmlElementId"":null,""containerName"":null,""type"":""Decreto"",""categories"":[],""entities"":[],""events"":[{""type"":""Admisión a trámite"",""categories"":[],""entities"":[{""type"":""Cuantía inicial"",""value"":""Indeterminada"",""mentions"":[]},{""type"":""Cuantía admitida"",""value"":""Indeterminada"",""mentions"":[]}],""isMain"":false}],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""Procedimiento de Madrid Category""}],""entities"":[{""type"":""Órgano judicial"",""value"":""ORGANO JUDICIAL DE MADRID"",""mentions"":[]},{""type"":""Órgano"",""value"":""ORGANO DE MADRID"",""mentions"":[]},{""type"":""Número de procedimiento"",""value"":""12253/2019"",""mentions"":[]},{""type"":""Procedimiento"",""value"":""Procedimiento de Madrid Entity"",""mentions"":[]}],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/6b088558-6bea-468f-8124-acf5c5fc13d2/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
			DocumentAnalysisAnaconda analysis = JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(a);

			var judicialNotificationprovider = new JudicialNotificationProvider(analysis);
			var judicialNotification = new JudicialNotification(judicialNotificationprovider);

			Assert.IsTrue(true);
		}

		[TestMethod]
		public void JudicialNotification_Builder_Sample4()
		{
            //string a = @"{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548"",""type"":""Desconocido"",""fragments"":[{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548_1"",""htmlElementId"":null,""containerName"":""2144 22.pdf"",""type"":""Decreto"",""categories"":[],""entities"":[],""events"":[{""type"":""Admisión a trámite"",""categories"":[],""entities"":[{""type"":""Cuantía inicial"",""value"":""Indeterminada"",""mentions"":[],""children"":[]},{""type"":""Cuantía admitida"",""value"":""Indeterminada"",""mentions"":[],""children"":[]}],""isMain"":false}],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false},{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548_2"",""htmlElementId"":null,""containerName"":""2144 22.pdf"",""type"":""Demanda"",""categories"":[],""entities"":[{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""JOSE MANUEL SANCHEZ"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""43456928-E"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]},{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""MARTA MORALES MARCOS"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""12356928-P"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]},{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""PEPE CERRO GRANDE"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""43454567-R"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]}],""events"":[],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""Juicio ordinario""}],""entities"":[{""type"":""Órgano judicial"",""value"":""JUZGADO DE PRIMERA INSTANCIA N” 13 DE LAS PALMAS DE GRAN CANARIA"",""mentions"":[],""children"":[]}],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/f8776753-ba6b-47b5-bbb0-f3b8a53c9548/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
            //string a = @"{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548"",""type"":""Desconocido"",""fragments"":[{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548_1"",""htmlElementId"":null,""containerName"":""2144 22.pdf"",""type"":""Decreto"",""categories"":[],""entities"":[{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""JOSE MANUEL SANCHEZ"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""43456928-E"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]},{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""MARTA MORALES MARCOS"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""12356928-P"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]},{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""PEPE CERRO GRANDE"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""43454567-R"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]}],""events"":[{""type"":""Admisión a trámite"",""categories"":[],""entities"":[{""type"":""Cuantía inicial"",""value"":""Indeterminada"",""mentions"":[],""children"":[]},{""type"":""Cuantía admitida"",""value"":""Indeterminada"",""mentions"":[],""children"":[]}],""isMain"":false}],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false},{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548_2"",""htmlElementId"":null,""containerName"":""2144 22.pdf"",""type"":""Demanda"",""categories"":[],""entities"":[],""events"":[],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""Juicio ordinario""}],""entities"":[{""type"":""Órgano judicial"",""value"":""JUZGADO DE PRIMERA INSTANCIA N” 13 DE LAS PALMAS DE GRAN CANARIA"",""mentions"":[],""children"":[]}],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/f8776753-ba6b-47b5-bbb0-f3b8a53c9548/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
            string a = @"{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548"",""type"":""Desconocido"",""fragments"":[{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548_1"",""htmlElementId"":null,""containerName"":""2144 22.pdf"",""type"":""Decreto"",""categories"":[],""entities"":[],""events"":[{""type"":""Admisión a trámite"",""categories"":[],""entities"":[{""type"":""Cuantía inicial"",""value"":""Indeterminada"",""mentions"":[],""children"":[]},{""type"":""Cuantía admitida"",""value"":""Indeterminada"",""mentions"":[],""children"":[]}],""isMain"":false}],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false},{""id"":""f8776753-ba6b-47b5-bbb0-f3b8a53c9548_2"",""htmlElementId"":null,""containerName"":""2144 22.pdf"",""type"":""Demanda"",""categories"":[],""entities"":[],""events"":[],""isMain"":false,""manualReviewRequired"":false,""analysisAllowed"":false}],""categories"":[{""type"":""Tipo de procedimiento"",""label"":""Juicio ordinario""}],""entities"":[{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[]},{""type"":null,""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""MARTA MORALES MARCOS"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""12356928-P"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]},{""type"":""Demandante"",""value"":null,""mentions"":[],""children"":[{""type"":""Nombre"",""value"":""PEPE CERRO GRANDE"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]},{""type"":""Número de identificación"",""value"":""43454567-R"",""isLinkedEntity"":null,""url"":null,""dataSource"":null,""mentions"":[],""acceptedBy"":null,""children"":[]}]},{""type"":""Órgano judicial"",""value"":""JUZGADO DE PRIMERA INSTANCIA N” 13 DE LAS PALMAS DE GRAN CANARIA"",""mentions"":[],""children"":[]}],""links"":[{""rel"":""htmlOutput"",""href"":""https://es-casfr-apimgt-test.azure-api.net/es-casfr-fadocument-test/v1/documents/f8776753-ba6b-47b5-bbb0-f3b8a53c9548/htmlOutput"",""action"":""Get"",""types"":[""text/html""]}]}";
            string demandante1 = "JOSE MANUEL SANCHEZ; 43456928-E";
			string demandante2 = "MARTA MORALES MARCOS; 12356928-P";
			string demandante3 = "PEPE CERRO GRANDE; 43454567-R";
			DocumentAnalysisAnaconda analysis = JsonConvert.DeserializeObject<DocumentAnalysisAnaconda>(a);

			var judicialNotificationprovider = new JudicialNotificationProvider(analysis);
			var judicialNotification = new JudicialNotification(judicialNotificationprovider);
			int finded = 0;
			if (judicialNotification.Exploration.Properties.Any(x => x.Value?.ToUpper() == demandante1 || x.Value?.ToUpper() == demandante2 ||
				x.Value?.ToUpper() == demandante3))
			{
				finded++;
			}
			foreach (var judic in judicialNotification.Exploration.SubLevels)
			{
				if (judic.Properties.Any(x => x.Value?.ToUpper() == demandante1 || x.Value?.ToUpper() == demandante2 ||
				x.Value?.ToUpper() == demandante3))
				{
					finded++;
				}
				foreach (var even in judic.SubLevels)
				{
					if (even.Properties.Any(x => x.Value?.ToUpper() == demandante1 || x.Value?.ToUpper() == demandante2 ||
									x.Value?.ToUpper() == demandante3))
					{
						finded++;
					}
				}
			}
			Assert.IsTrue(finded > 0);
		}

	}
}