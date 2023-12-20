using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
    public class ProcessProviderTest
    {
        private Process? fakedProcess;

        [TestInitialize]
        public void Init() {
            this.fakedProcess = new Process(new Mock<IProcessProvider>().Object);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessProvider_NullAnaconda()
        {
            new ProcessProvider(null);
        }


        [TestMethod()]
        public void ProcessProvider_EmptyAnaconda()
        {
            var proc = new ProcessProvider(new DocumentAnalysisAnaconda()
            {
            });
            Assert.AreEqual("", proc.Court);
            Assert.AreEqual("", proc.Judgement);
            Assert.AreEqual("", proc.Procedure);
            Assert.AreEqual("", proc.Topic);
            Assert.AreEqual(0, proc.GetActivePart(fakedProcess).Count());
            Assert.AreEqual(0, proc.GetPassivePart(fakedProcess).Count());
        }


        [DataTestMethod]
        [DataRow(ProcessProvider.ENTITY_ORGANO, nameof(ProcessProvider.Court), DisplayName = nameof(ProcessProvider.Court))]
        [DataRow(ProcessProvider.ENTITY_AUTO, nameof(ProcessProvider.Judgement), DisplayName = nameof(ProcessProvider.Judgement))]
        [DataRow(ProcessProvider.ENTITY_PROCEDURE, nameof(ProcessProvider.Procedure), DisplayName = nameof(ProcessProvider.Procedure))]
        [DataRow(ProcessProvider.ENTITY_NIG, nameof(ProcessProvider.NIG), DisplayName = nameof(ProcessProvider.NIG))]
        [DataRow(ProcessProvider.ENTITY_MATERIA, nameof(ProcessProvider.Topic), DisplayName = nameof(ProcessProvider.Topic))]
		public void ProcessProvider_EntityProperties(string valorEntidad, string theProperty)
        {
            var theValue = Guid.NewGuid().ToString();
            var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
            {
                Entities = new List<EntityAnaconda>()
                    {
                        new EntityAnaconda()
                        {
                            Type = valorEntidad,
                            Value = theValue
                        }
                    }
            });
            var valorXReflexion = theProcess.GetType()
                .GetProperties()
                .Where(x => x.Name == theProperty)
                .Select(y => y.GetValue(theProcess))
                .FirstOrDefault();
            Assert.AreEqual(theValue, valorXReflexion);
        }

		[DataTestMethod]
		[DataRow(ProcessProvider.CATEGORY_PROCEDURE_TYPE, nameof(ProcessProvider.Procedure), DisplayName = nameof(ProcessProvider.Procedure))]
		[DataRow(ProcessProvider.CATEGORY_MATERIA, nameof(ProcessProvider.Topic), DisplayName = nameof(ProcessProvider.Topic))]
		public void ProcessProvider_CategoryProperties(string valorCategoria, string theProperty)
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
                Categories = new List<CategoryAnaconda>()
					{
						new CategoryAnaconda()
						{
							Type = valorCategoria,
                            Label = theValue
						}
					}
			});
			var valorXReflexion = theProcess.GetType()
				.GetProperties()
				.Where(x => x.Name == theProperty)
				.Select(y => y.GetValue(theProcess))
				.FirstOrDefault();
			Assert.AreEqual(theValue, valorXReflexion);
		}

		[DataTestMethod]		
		[DataRow(ProcessProvider.ENTITY_ACTIVEPART_ACTOR, DisplayName = "Actor")]
		[DataRow(ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO, DisplayName = "Demandado")]
		public void ProcessProvider_EntityPropertiesNotInProcessProviderObject(string valorEntidad)
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
					{
						new EntityAnaconda()
						{
							Type = valorEntidad,
							Value = theValue
						}
					}
			});

			var property = theProcess.GetType()
                .GetProperties()
                .FirstOrDefault(o => o.CustomAttributes.Any(p => p.AttributeType == typeof(DocumentAnalysisEntityAttribute) && p.ConstructorArguments.Any(s => s.Value != null && s.Value.ToString() == valorEntidad)));

			
			Assert.IsNull(property, $"El valor de la entity '{valorEntidad}' no debe crearse en el objeto ProcessProvider, ya que se introduce como PersonsProvider en Parte activa o Parte pasiva");
		}

		[TestMethod()]
        public void ActivePart_ParteActiva()
        {
            var involved = new List<string[]>()
                {
                    new string[]{"N1","DNI1" },
                    new string[]{"N2","  " },
                    new string[]{"  ","DNI3" },
                    new string[]{"N4",null },
                    new string[]{null,"DNI5" }
                };

            var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
            {
                Entities = new List<EntityAnaconda>()
                    {
                        GetEntityInvolved(involved[0][0], involved[0][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[1][0], involved[1][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[2][0], involved[2][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[3][0], involved[3][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[4][0], involved[4][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR)
                    }
            });

            Assert.That.AreEquivalent(involved, theProcess.GetActivePart(fakedProcess),
                (stringsInvolved, thePersonInvolved) =>
                {
                    var theExpectedName = stringsInvolved[0];
                    var theExpectedDNI = stringsInvolved[1];
                    if (string.IsNullOrWhiteSpace(theExpectedName))
                    {
                        theExpectedName = string.Empty;
                    }
                    if (string.IsNullOrWhiteSpace(theExpectedDNI))
                    {
                        theExpectedDNI = string.Empty;
                    }

                    return theExpectedName.Equals(thePersonInvolved.Name) &&
                           theExpectedDNI.Equals(thePersonInvolved.DNI);
                });

        }

        [TestMethod()]
        public void ActivePart_IgnoreEmptyParts()
        {
            var involved = new List<string[]>()
                {
                    new string[]{"N1","DNI1" },
                    new string[]{"N2","  " },
                    new string[]{"  ","DNI3" },
                    new string[]{"N4",null },
                    new string[]{null,"DNI5" }
                };

            var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
            {
                Entities = new List<EntityAnaconda>()
                    {
                        GetEntityInvolved(involved[0][0], involved[0][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[1][0], involved[1][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[2][0], involved[2][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[3][0], involved[3][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR),
                        GetEntityInvolved(involved[4][0], involved[4][1], ProcessProvider.ENTITY_ACTIVEPART_ACTOR)
                    }
            });

            Assert.That.AreEquivalent(involved, theProcess.GetActivePart(fakedProcess),
                (stringsInvolved, thePersonInvolved) =>
                {
                    var theExpectedName = stringsInvolved[0];
                    var theExpectedDNI = stringsInvolved[1];
                    if (string.IsNullOrWhiteSpace(theExpectedName))
                    {
                        theExpectedName = string.Empty;
                    }
                    if (string.IsNullOrWhiteSpace(theExpectedDNI))
                    {
                        theExpectedDNI = string.Empty;
                    }

                    return theExpectedName.Equals(thePersonInvolved.Name) &&
                           theExpectedDNI.Equals(thePersonInvolved.DNI);
                });

        }

		[TestMethod()]
		public void PassivePart_PartePasiva()
		{
			var involved = new List<string[]>()
				{
					new string[]{"N1","DNI1" },
					new string[]{"N2","  " },
					new string[]{"  ","DNI3" },
					new string[]{"N4",null },
					new string[]{null,"DNI5" }
				};

			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
					{
						GetEntityInvolved(involved[0][0], involved[0][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[1][0], involved[1][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[2][0], involved[2][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[3][0], involved[3][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[4][0], involved[4][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO)
					}
			});

			Assert.That.AreEquivalent(involved, theProcess.GetPassivePart(fakedProcess),
				(stringsInvolved, thePersonInvolved) =>
				{
					var theExpectedName = stringsInvolved[0];
					var theExpectedDNI = stringsInvolved[1];
					if (string.IsNullOrWhiteSpace(theExpectedName))
					{
						theExpectedName = string.Empty;
					}
					if (string.IsNullOrWhiteSpace(theExpectedDNI))
					{
						theExpectedDNI = string.Empty;
					}

					return theExpectedName.Equals(thePersonInvolved.Name) &&
						   theExpectedDNI.Equals(thePersonInvolved.DNI);
				});

		}

		[TestMethod()]
		public void PassivePart_IgnoreEmptyParts()
		{
			var involved = new List<string[]>()
				{
					new string[]{"N1","DNI1" },
					new string[]{"N2","  " },
					new string[]{"  ","DNI3" },
					new string[]{"N4",null },
					new string[]{null,"DNI5" }
				};

			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
					{
						GetEntityInvolved(involved[0][0], involved[0][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[1][0], involved[1][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[2][0], involved[2][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[3][0], involved[3][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO),
						GetEntityInvolved(involved[4][0], involved[4][1], ProcessProvider.ENTITY_PASSIVEPART_DEMANDADO)
					}
			});

			Assert.That.AreEquivalent(involved, theProcess.GetPassivePart(fakedProcess),
				(stringsInvolved, thePersonInvolved) =>
				{
					var theExpectedName = stringsInvolved[0];
					var theExpectedDNI = stringsInvolved[1];
					if (string.IsNullOrWhiteSpace(theExpectedName))
					{
						theExpectedName = string.Empty;
					}
					if (string.IsNullOrWhiteSpace(theExpectedDNI))
					{
						theExpectedDNI = string.Empty;
					}

					return theExpectedName.Equals(thePersonInvolved.Name) &&
						   theExpectedDNI.Equals(thePersonInvolved.DNI);
				});

		}

		[DataTestMethod]
		public void ProcessProvider_CategoryProcedurePropertyMap()
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
                 Categories = new List<CategoryAnaconda>()
                 {
                     new CategoryAnaconda() {Type = ProcessProvider.CATEGORY_PROCEDURE_TYPE, Label = theValue }
                 }
			});
            Assert.AreEqual(theProcess.Procedure, theValue);
		}

		[DataTestMethod]
		public void ProcessProvider_CategoryMateriaPropertyMap()
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
				 {
					 new EntityAnaconda() {Type = ProcessProvider.ENTITY_MATERIA, Value = theValue }
				 },
				Categories = new List<CategoryAnaconda>()
				 {
					 new CategoryAnaconda() {Type = ProcessProvider.CATEGORY_MATERIA, Label = "NO SOBREESCRIBE" }
				 }
			});
			Assert.AreEqual(theProcess.Topic, theValue);
		}

		[DataTestMethod]
		public void ProcessProvider_EntityOrganoJudicialPropertyMap()
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
				 {
					 new EntityAnaconda() {Type = ProcessProvider.ENTITY_ORGANO_JUDICIAL, Value = theValue }
				 }
			});
			Assert.AreEqual(theProcess.Court, theValue);
		}

		[DataTestMethod]
		public void ProcessProvider_EntityProcedurePropertyMap()
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
				 {
					 new EntityAnaconda() {Type = ProcessProvider.ENTITY_PROCEDURE, Value = theValue }
				 }
			});
			Assert.AreEqual(theProcess.Procedure, theValue);
		}

		[DataTestMethod]
		public void ProcessProvider_EntityMateriaPropertyMap()
		{
			var theValue = Guid.NewGuid().ToString();
			var theProcess = new ProcessProvider(new DocumentAnalysisAnaconda()
			{
				Entities = new List<EntityAnaconda>()
				 {
					 new EntityAnaconda() {Type = ProcessProvider.ENTITY_MATERIA, Value = theValue }
				 }
			});
			Assert.AreEqual(theProcess.Topic, theValue);
		}

		private static EntityAnaconda GetEntityInvolved(string nombre, string dni, string type)
        {
            return new EntityAnaconda()
            {
                Type = type,
                Value = String.Empty,
                Children = new List<EntityAnaconda>()
                            {
                                new EntityAnaconda()
                                {
                                    Type = PersonsProvider.ENTITY_NAME,
                                    Value = nombre
                                },
                                new EntityAnaconda()
                                {
                                    Type = PersonsProvider.ENTITY_DNI,
                                    Value = dni
                                }
                            }
            };
        }
    }
}