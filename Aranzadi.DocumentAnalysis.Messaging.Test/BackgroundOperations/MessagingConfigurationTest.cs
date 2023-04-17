using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging.Test.BackgroundOperations
{
    [TestClass()]
    public class MessagingConfigurationTest
    {        

        [TestMethod()]
        public void Validate_OK()
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            Assert.IsTrue(config.Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]       
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_ServicesBusCola_Error(string value)
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            config.ServicesBusCola = value;
            Assert.IsFalse(config.Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_ServicesBusConnectionString_Error(string value)
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            config.ServicesBusConnectionString = value;
            Assert.IsFalse(config.Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_Source_Error(string value)
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            config.Source = value;
            Assert.IsFalse(config.Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_Type_Error(string value)
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            config.Source = value;
            Assert.IsFalse(config.Validate());
        }

        [TestMethod()]
        public void Validate_URLOrquestador_Error()
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            config.URLOrquestador = null;
            Assert.IsFalse(config.Validate());
        }

        [TestMethod()]
       public void Validate_URLServicioAnalisisDoc_Error()
        {
            var config = MessagingConfigurationTest.GetValidConfiguration();
            config.URLServicioAnalisisDoc = null;
            Assert.IsFalse(config.Validate());
        }



        public static MessagingConfiguration GetValidConfiguration()
        {
            var config = new MessagingConfiguration()
            {
                ServicesBusCola = "Nombre cola service bus",
                ServicesBusConnectionString = "Endpoint service bus",
                Source = "Fusion",
                Type = "Analisis documento",
                URLOrquestador = new Uri(@"https://URLOrquestador.es"),
                URLServicioAnalisisDoc = new Uri(@"https://URLServicioAnalisisDoc.es"),
            };
            return config;
        }

        public static MessagingConfiguration GetInvalidConfiguration()
        {
            var config = GetValidConfiguration();
            config.ServicesBusCola = "";
            Assert.IsFalse(config.Validate());
            return config;
        }

    }
}
