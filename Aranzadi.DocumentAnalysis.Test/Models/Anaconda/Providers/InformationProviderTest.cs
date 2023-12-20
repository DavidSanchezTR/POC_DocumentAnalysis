using Aranzadi.DocumentAnalysis.Models.Anaconda;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers;
using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Aranzadi.DocumentAnalysis.Test.Models.Anaconda.Providers
{
	[TestClass()]
    public class InformationProviderTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InformationProvider_NullDocAna()
        {         
            new InformationProvider(null);
        }




        [TestMethod()]
        public void InformationProvider_DefaultValues()
        {
            var emptyInformation = new InformationProvider(new DocumentAnalysisAnaconda());
            Assert.IsNull(emptyInformation.NotificationDate);
            Assert.AreEqual(string.Empty, emptyInformation.NotificationDateDescription);
        }

        [TestMethod()]
        public void InformationProvider_ValidDateUTF()
        {

            var dateStr = DateTime.UtcNow.AddDays(-3).ToString(EntityAttribute.DATETIME_UTC_STRING_FORMAT);
            DateTime date;
            DateTime.TryParse(dateStr, out date);

            date = date.ToUniversalTime();

            var emptyInformation = new InformationProvider(
                new DocumentAnalysisAnaconda()
                {
                    Entities = new List<EntityAnaconda>()
                    {
                     new EntityAnaconda
                     {
                         Value = dateStr.ToString(),
                         Type = InformationProvider.ENTITY_FECHA_NOTIFICACION
                     }
                 }
                });
            Assert.AreEqual(dateStr, emptyInformation.NotificationDateDescription, "DateTime, fecha");
            Assert.AreEqual(date, emptyInformation.NotificationDate, "DateTime Description");
        }

        [TestMethod()]
        public void InformationProvider_ValidDateLocal()
        {

            var dateStr = DateTime.UtcNow.AddDays(-3).ToString(EntityAttribute.DATETIME_UNKNOWN_TIME_ZONE);
            DateTime date;
            DateTime.TryParse(dateStr, out date);

            var emptyInformation = new InformationProvider(
                new DocumentAnalysisAnaconda()
                {
                    Entities = new List<EntityAnaconda>()
                    {
                     new EntityAnaconda
                     {
                         Value = dateStr.ToString(),
                         Type = InformationProvider.ENTITY_FECHA_NOTIFICACION
                     }
                 }
                });
            Assert.AreEqual(dateStr, emptyInformation.NotificationDateDescription, "DateTime, fecha");
            Assert.AreEqual(date, emptyInformation.NotificationDate, "DateTime Description");
        }

        [TestMethod()]
        public void InformationProvider_UnknownLocal()
        {

            var dateStr = "veinticinco de abril del 2019";
            DateTime date;
            DateTime.TryParse(dateStr, out date);

            var emptyInformation = new InformationProvider(
                new DocumentAnalysisAnaconda()
                {
                    Entities = new List<EntityAnaconda>()
                    {
                     new EntityAnaconda
                     {
                         Value = dateStr.ToString(),
                         Type = InformationProvider.ENTITY_FECHA_NOTIFICACION
                     }
                 }
                });
            Assert.IsNull(emptyInformation.NotificationDate, "DateTime, fecha");
            Assert.AreEqual(dateStr, emptyInformation.NotificationDateDescription, "DateTime Description");
        }

        [TestMethod()]
        public void InformationProvider_NullEntities()
        {


            var emptyInformation = new InformationProvider(
                new DocumentAnalysisAnaconda()
                {
                    Entities = null
                });
            Assert.IsNull(emptyInformation.NotificationDate);
            Assert.AreEqual(string.Empty, emptyInformation.NotificationDateDescription);
        }

    }
}