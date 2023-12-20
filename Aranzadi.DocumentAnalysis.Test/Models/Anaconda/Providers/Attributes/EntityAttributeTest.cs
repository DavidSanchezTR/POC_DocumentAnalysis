using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers.Attributes
{
	[TestClass()]
    public class EntityAttributeTest
    {
        private class TestEntityAtribute : EntityAttribute
        {
            public TestEntityAtribute(string entityName) : base(entityName)
            {
            }

            public TestEntityAtribute(string entityName, Type theType) : base(entityName, theType)
            {
            }
        }


        [DataTestMethod]
        [DataRow(null, DisplayName = "null entity")]
        [DataRow("", DisplayName = "empty entity")]
        public void EntityAttribute_Constructor(string valor)
        {
            Assert.AreEqual("", new TestEntityAtribute(valor).ValueName);
            Assert.AreEqual(EntityAttribute.DEFAULT_TYPE, new TestEntityAtribute(valor).ValueType);
        }

        [DataTestMethod]
        [DataRow("v1", null, EntityAttribute.DEFAULT_TYPE, DisplayName = "si no hay tipo")]
        [DataRow("v2", EntityAttribute.Type.DateTimeMaybeUTC, EntityAttribute.Type.DateTimeMaybeUTC, DisplayName = "empty entity")]
        public void EntityAttribute_Constructor_Type(string valor, EntityAttribute.Type setType, EntityAttribute.Type expectedType)
        {
            var att = new TestEntityAtribute(valor, setType);
            Assert.AreEqual(valor, att.ValueName);
            Assert.AreEqual(expectedType, att.ValueType);
        }


        private const string WITHVALUE_TIPE_ENTITY = "TipoEntityConValor";
        private const string PRIVATE_WITHVALUE_TIPE_ENTITY = "TipoEntityConValor_Private";
        private const string PRIVATEGET_TIPE_ENTITY = "TipoEntityConValor_SinGet";
        private const string PRIVATE_VALUE = "Private_VAlue";


        private class FakeClassWithProperties
        {
            [DocumentAnalysisEntity(WITHVALUE_TIPE_ENTITY)]
            public string ConValor { get; set; }

            [DocumentAnalysisEntity(PRIVATE_WITHVALUE_TIPE_ENTITY)]
            internal string ConValorPrivate { get; set; }

            [DocumentAnalysisEntity(PRIVATEGET_TIPE_ENTITY)]
            internal string SinSet { get => PRIVATE_VALUE; }

        }


        [TestMethod()]
        public void SetPropertiesByAtribute_SetValue()
        {
            var elValor1 = Guid.NewGuid().ToString();
            var elValor2 = Guid.NewGuid().ToString();
            var theFakeClas = new FakeClassWithProperties();

            var entities = new List<EntityAnaconda>()
            {
                new EntityAnaconda()
                {
                    Value = elValor1,
                    Type = WITHVALUE_TIPE_ENTITY
                },
                new EntityAnaconda()
                {
                    Value = elValor2,
                    Type = PRIVATE_WITHVALUE_TIPE_ENTITY
                },
            };
            EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(entities, theFakeClas);

            Assert.AreEqual(elValor1, theFakeClas.ConValor);
            Assert.AreEqual(elValor2, theFakeClas.ConValorPrivate);
            Assert.AreEqual(PRIVATE_VALUE, theFakeClas.SinSet);
        }



        [TestMethod()]
        public void SetPropertiesByAtribute_SetValueNulosVacios()
        {
            var noSeToca = Guid.NewGuid().ToString();

            var theFakeClas = new FakeClassWithProperties()
            {
                ConValor = noSeToca,
                ConValorPrivate = noSeToca
            };

            var entities = new List<EntityAnaconda>()
            {
                new EntityAnaconda()
                {
                    Value = null,
                    Type = WITHVALUE_TIPE_ENTITY
                },
                new EntityAnaconda()
                {
                    Value = "   ",
                    Type = PRIVATE_WITHVALUE_TIPE_ENTITY
                },
            };
            EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(entities, theFakeClas);

            Assert.AreEqual(noSeToca, theFakeClas.ConValor);
            Assert.AreEqual(noSeToca, theFakeClas.ConValorPrivate);
        }



        [TestMethod()]
        public void SetPropertiesByAtribute_IgnoreNulls()
        {
            EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(null, null);
        }


        private const string DATE_EMPTY = "DATE_EMPTY";
        private const string DATE_OKUTC = "DATE_OK";
        private const string DATE_OKUNKNOWTIMEZONE = "DATE_OKUnknowTimeZone";
        private const string DATE_BAD_FORMAT = "DATE_BAT_FORMAT";
        private class FakeClassWithPropertiesDateTime
        {


            [DocumentAnalysisEntity(DATE_EMPTY, EntityAttribute.Type.DateTimeMaybeUTC)]
            public DateTime DateEmpty { get; set; }

            [DocumentAnalysisEntity(DATE_OKUTC, EntityAttribute.Type.DateTimeMaybeUTC)]
            public DateTime DateOKUtc { get; set; }

            [DocumentAnalysisEntity(DATE_OKUNKNOWTIMEZONE, EntityAttribute.Type.DateTimeMaybeUTC)]
            public DateTime DateOKUnknowTimeZone { get; set; }

            [DocumentAnalysisEntity(DATE_BAD_FORMAT, EntityAttribute.Type.DateTimeMaybeUTC)]
            public DateTime DateBad { get; set; }

            [DocumentAnalysisEntity(DATE_OKUTC, EntityAttribute.Type.DateTimeMaybeUTC)]
            public int NotADateType { get; set; }

        }


        [TestMethod()]
        public void SetPropertiesByAtribute_DateTimeTest()
        {
            var theDate = DateTime.UtcNow.AddDays(-1);
            var fixDate = DateTime.Now.AddDays(365);

            var theFakeClas = new FakeClassWithPropertiesDateTime()
            {
                DateOKUtc = fixDate,
                DateOKUnknowTimeZone = fixDate,
                DateBad = fixDate,
                DateEmpty = fixDate
            };

            var entities = new List<EntityAnaconda>()
            {
                new EntityAnaconda()
                {
                    Value = theDate.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT),
                    Type = DATE_OKUTC
                },
                new EntityAnaconda()
                {
                    Value = theDate.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE),
                    Type = DATE_OKUNKNOWTIMEZONE
                },
                new EntityAnaconda()
                {
                    Value = string.Empty,
                    Type = DATE_EMPTY
                },
                new EntityAnaconda()
                {
                    Value = "KK yo no soy una fecha",
                    Type = DATE_BAD_FORMAT
                }
            };
            EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(entities, theFakeClas);

            Assert.AreEqual(theDate.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT),
                theFakeClas.DateOKUtc.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT));
            Assert.IsTrue(theFakeClas.DateOKUtc.Kind == DateTimeKind.Utc);

            Assert.AreEqual(theDate.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE),
                theFakeClas.DateOKUnknowTimeZone.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE));
            Assert.IsTrue(theFakeClas.DateOKUnknowTimeZone.Kind == DateTimeKind.Unspecified);

            Assert.AreEqual(fixDate, theFakeClas.DateBad);
            Assert.AreEqual(fixDate, theFakeClas.DateEmpty);
        }

        [TestMethod()]
        public void SetPropertiesByAtribute_DateTime_CaptureException()
        {
            var theDate = DateTime.UtcNow.AddDays(-1);
            var fixDate = DateTime.Now.AddDays(365);
            var intValue = 33;
            var theFakeClas = new FakeClassWithPropertiesDateTime()
            {
                DateOKUtc = fixDate,
                DateOKUnknowTimeZone = fixDate,
                DateBad = fixDate,
                DateEmpty = fixDate,
                NotADateType = intValue
            };

            var entities = new List<EntityAnaconda>()
            {
                new EntityAnaconda()
                {
                    Value = theDate.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT),
                    Type = DATE_OKUTC
                },
                new EntityAnaconda()
                {
                    Value = theDate.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE),
                    Type = DATE_OKUNKNOWTIMEZONE
                },
                new EntityAnaconda()
                {
                    Value = string.Empty,
                    Type = DATE_EMPTY
                },
                new EntityAnaconda()
                {
                    Value = "KK yo no soy una fecha",
                    Type = DATE_BAD_FORMAT
                }
            };
            EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(entities, theFakeClas);

            Assert.AreEqual(theDate.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT),
                theFakeClas.DateOKUtc.ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT));
            Assert.IsTrue(theFakeClas.DateOKUtc.Kind == DateTimeKind.Utc);

            Assert.AreEqual(theDate.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE),
                theFakeClas.DateOKUnknowTimeZone.ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE));
            Assert.IsTrue(theFakeClas.DateOKUnknowTimeZone.Kind == DateTimeKind.Unspecified);

            Assert.AreEqual(fixDate, theFakeClas.DateBad);
            Assert.AreEqual(fixDate, theFakeClas.DateEmpty);
            Assert.AreEqual(intValue, theFakeClas.NotADateType);
        }
    }


}