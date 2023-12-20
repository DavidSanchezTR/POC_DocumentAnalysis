using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;
using Aranzadi.DocumentAnalysis.Models.CreditReservations;
using Aranzadi.DocumentAnalysis.Services;
using Aranzadi.DocumentAnalysis.Test.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;
using static Aranzadi.DocumentAnalysis.Models.OperationResult;

namespace Aranzadi.DocumentAnalysis.Test
{
    [TestClass]
    public class CreditsConsumptionClientTest
    {
        [TestMethod]
        public async Task GetTenantCredits_ValidData_ReturnsOK()
        {
            //Arrange
            Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
            var credtisConfig = new CreditsConsumptionClass()
            {
                CreditsConsumptionService = "https://UrlApiCredits",
                AzureADCreditsConsumptionScope = $"api://{Guid.NewGuid()}/.default",
                AzureADCreditsConsumptionTenant = Guid.NewGuid().ToString()
            };
            documentAnalysisOptionsMock.Object.CreditsConsumption = credtisConfig;
            documentAnalysisOptionsMock.Object.Environment = "Test";
            documentAnalysisOptionsMock.Object.CheckIfActiveCreditsConsumption = true;

            var response = new CreditResponse();
            TenantCredit tenantCredit = new TenantCredit() {FreeReservations = 2 };
            List<TenantCredit> tenantCreditList = new List<TenantCredit>();
            tenantCreditList.Add(tenantCredit);
            response.Count = 1;
            response.TenantCredits = tenantCreditList;
            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(response))
                };
            });

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

           

            //Act
            var creditsConsumptionClient = new CreditsConsumptionClient(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
            var result = await creditsConsumptionClient.GetTenantCredits("1234", "analisisdocumental");

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(1,result.Result.Count);
            Assert.AreEqual(1,result.Result.TenantCredits.Count());
            Assert.AreEqual(2,result.Result.TenantCredits.Single().FreeReservations);
        }
        
        [TestMethod]
        public async Task CompleteReservation_ValidData_ReturnsOK()
        {
            //Arrange
            Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();
            var credtisConfig = new CreditsConsumptionClass()
            {
                CreditsConsumptionService = "https://UrlApiCredits",
                AzureADCreditsConsumptionScope = $"api://{Guid.NewGuid()}/.default",
                AzureADCreditsConsumptionTenant = Guid.NewGuid().ToString()
            };
            documentAnalysisOptionsMock.Object.CreditsConsumption = credtisConfig;
            documentAnalysisOptionsMock.Object.Environment = "Test";
            documentAnalysisOptionsMock.Object.CheckIfActiveCreditsConsumption = true;

            var response = new OperationResult() { Code = ResultCode.Success};
           
            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(response))
                };
            });

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

           

            //Act
            var creditsConsumptionClient = new CreditsConsumptionClient(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
            var result = await creditsConsumptionClient.CompleteReservation("1234", "analisisdocumental");

            //Assert
            Assert.AreEqual(ResultCode.Success, result.Code);
            Assert.IsNull(result.Detail);
        }
        
        [TestMethod]
        public async Task CreateReservation_ValidDataCreditsFalse_ReturnsOK()
        {
            //Arrange
            Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();

            var credtisConfig = new CreditsConsumptionClass()
            {
                CreditsConsumptionService = "https://UrlApiCredits",
                AzureADCreditsConsumptionScope = $"api://{Guid.NewGuid()}/.default",
                AzureADCreditsConsumptionTenant = Guid.NewGuid().ToString()
            };
            documentAnalysisOptionsMock.Object.CreditsConsumption = credtisConfig;
            documentAnalysisOptionsMock.Object.Environment = "Test";
            documentAnalysisOptionsMock.Object.CheckIfActiveCreditsConsumption = false;

            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(String.Empty))
                };
            });

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

            var reservation = new NewTenantCreditReservation() { CreditTemplateID = "analisisdocumental", TenantID = "1234", UserID = "88" };


            //Act
            var creditsConsumptionClient = new CreditsConsumptionClient(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
            var result = await creditsConsumptionClient.CreateReservation(reservation);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(String.Empty, result.Result.ID);
            Assert.AreEqual(String.Empty, result.Result.TenantCreditID);
            Assert.IsFalse(result.Result.Completed);
          
        }
        
        [TestMethod]
        public async Task CreateReservation_ValidDataCreditsTrue_ReturnsOK()
        {
            //Arrange
            Mock<DocumentAnalysisOptions> documentAnalysisOptionsMock = new Mock<DocumentAnalysisOptions>();

            var credtisConfig = new CreditsConsumptionClass()
            {
                CreditsConsumptionService = "https://UrlApiCredits",
                AzureADCreditsConsumptionScope = $"api://{Guid.NewGuid()}/.default",
                AzureADCreditsConsumptionTenant = Guid.NewGuid().ToString()
            };
            documentAnalysisOptionsMock.Object.CreditsConsumption = credtisConfig;
            documentAnalysisOptionsMock.Object.Environment = "Test";
            documentAnalysisOptionsMock.Object.CheckIfActiveCreditsConsumption = true;

            var response = new TenantCreditReservation() { 
                Completed = false,
                UserID = "88",
                CreditTemplateID = "analisisdocumental"
            };
            
             var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(response))
                };
            });

            Mock<IHttpClientFactory> httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(o => o.CreateClient(It.IsAny<string>())).Returns(new HttpClient(handler));

            var reservation = new NewTenantCreditReservation() { CreditTemplateID = "analisisdocumental", TenantID = "1234", UserID = "88" };


            //Act
            var creditsConsumptionClient = new CreditsConsumptionClient(documentAnalysisOptionsMock.Object, httpClientFactoryMock.Object);
            var result = await creditsConsumptionClient.CreateReservation(reservation);

            //Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(reservation.CreditTemplateID, result.Result.CreditTemplateID);
            Assert.AreEqual(reservation.UserID, result.Result.UserID);
            Assert.IsFalse(result.Result.Completed);
          
        }
    }
}
