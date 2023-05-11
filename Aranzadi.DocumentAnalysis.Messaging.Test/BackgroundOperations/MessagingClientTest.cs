using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using ThomsonReuters.BackgroundOperations.Messaging;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.Messaging.Test.DTO.Request;
using ThomsonReuters.BackgroundOperations.Messaging.Models;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Response;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Aranzadi.DocumentAnalysis.DTO.Enums;

namespace Aranzadi.DocumentAnalysis.Messaging.Test.BackgroundOperations
{
    [TestClass()]
    public class MessagingClientTest
    {

        private Mock<IMessageSender> senderMoq = null!;
        private MessagingConfiguration conf = null!;
        private Mock<IHttpClientFactory> factMoq = null!;
        private MessagingClient theClient = null!;
        private AnalysisContext theContext = null!;

        private static readonly string REF_DOC = "Ref_doc";
        internal static readonly string DOC_REF = "33";


        [TestInitialize]
        public void TestInitialize()
        {
            this.senderMoq = new Mock<IMessageSender>();
            this.conf = MessagingConfigurationTest.GetValidConfiguration();
            this.factMoq = new Mock<IHttpClientFactory>();
            this.theContext = AnalysisContextTests.ValidContext();
        }

        [TestMethod()]
        public async Task SendRequestAsync_OK()
        {            
            senderMoq.Setup(o => o.Send(conf.ServicesBusCola, It.IsAny<Message<DocumentAnalysisRequest>>(), default)).Returns(Task.Run(() => { }));
            Mock<IHttpClientFactory> factMoq = new Mock<IHttpClientFactory>();

            MessagingClient client = new MessagingClient(senderMoq.Object, conf, factMoq.Object);
            PackageRequest request = PackageRequestTest.ValidPackage();
            PackageRequestTrack prt = await client.SendRequestAsync(request);

            Assert.AreEqual(prt.DocumentAnalysysRequestTracks.First().DocumentUniqueRefences, request.Documents.First().Guid);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MessagingClient_TodoNull_Exception()
        {
            new MessagingClient(null, null, null);
        }

        [TestMethod()]
        public void MessagingClient_EverythingValue_OK()
        {

            Assert.IsNotNull(new MessagingClient(this.senderMoq.Object, conf, factMoq.Object));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MessagingClient_IMessageSenderNull_Exception()
        {
            Assert.IsNotNull(new MessagingClient(null, conf, factMoq.Object));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MessagingClient_MessagingConfigurationNull_Exception()
        {

            Assert.IsNotNull(new MessagingClient(senderMoq.Object, null, factMoq.Object));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void MessagingClient_MessagingConfigurationInvalid_Exception()
        {

            Assert.IsNotNull(new MessagingClient(senderMoq.Object,
                MessagingConfigurationTest.GetInvalidConfiguration(), factMoq.Object));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MessagingClient_IHttpClientFactoryNull_Exception()
        {


            Assert.IsNotNull(new MessagingClient(senderMoq.Object, conf, null));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SendRequest_RequestNull_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.SendRequestAsync(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task SendRequest_RequestPackageInvalid_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.SendRequestAsync(PackageRequestTest.InvalidPackage());
        }

        [TestMethod()]
        public async Task SendRequest_RequestFormat_OK()
        {

            var t = Task.Run(() => { });

            PackageRequest paquete = PackageRequestTest.ValidPackage();
            this.senderMoq.Setup(
                e => e.Send(
                       this.conf.ServicesBusCola,
                       It.Is<Message<DocumentAnalysisRequest>>(u => ValidateMessageFormat(paquete, u)),
                       default))
            .Returns(t);

            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.SendRequestAsync(paquete);

            this.senderMoq.VerifyAll();
        }

        private bool ValidateMessageFormat(PackageRequest esperado, object u)
        {
            if (u == null || u is not Message<DocumentAnalysisRequest>)
            {
                return false;
            }
            var comparado = (Message<DocumentAnalysisRequest>)u;
            Assert.AreEqual(esperado.Context, comparado.AdditionalData);
            Assert.AreEqual(conf.Source, comparado.Source);
            Assert.AreEqual(conf.Type, comparado.Type);
            Assert.AreEqual(esperado.Context.Tenant, comparado.Tenant);
            return true;
        }

        [TestMethod()]
        public async Task SendRequest_RequestContent_OK()
        {

            PackageRequest paquete = PackageRequestTest.ValidPackage();
            paquete.Documents = new List<DocumentAnalysisRequest>() {
                DocumentAnalysisRequestTest.ValidRequest(),
                DocumentAnalysisRequestTest.ValidRequest(),
            };

            this.senderMoq.Setup(
                e => e.Send(
                       this.conf.ServicesBusCola,
                       It.Is<Message<DocumentAnalysisRequest>>(u => ValidateMessageContent(paquete, u)),
                       default))
            .Returns(Task.Run(() => { }));

            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.SendRequestAsync(paquete);

            this.senderMoq.VerifyAll();
        }

        private static bool ValidateMessageContent(PackageRequest esperado, object u)
        {
            if (u == null || u is not Message<DocumentAnalysisRequest>)
            {
                return false;
            }
            var chunks = ((Message<DocumentAnalysisRequest>)u).DataChunks;

            foreach (var chunck in chunks)
            {
                Assert.AreEqual(1, esperado.Documents.Where((doc) => doc == chunck.Data).Count());
            }
            foreach (var item in esperado.Documents)
            {
                Assert.AreEqual(1, chunks.Where((chun) => chun.Data == item).Count());
            }
            return true;
        }

        [TestMethod()]
        public async Task SendRequest_ValidateResponse_PackageTrackOK()
        {

            PackageRequest paquete = PackageRequestTest.ValidPackage();
            paquete.Documents = new List<DocumentAnalysisRequest>() {
                DocumentAnalysisRequestTest.ValidRequest(),
                DocumentAnalysisRequestTest.ValidRequest(),
            };

            this.senderMoq.Setup(
                e => e.Send(
                       this.conf.ServicesBusCola,
                       It.IsAny<Message<DocumentAnalysisRequest>>(),
                       default))
            .Returns(Task.FromResult(() => { }));

            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            PackageRequestTrack packageTrack = await theClient.SendRequestAsync(paquete);
            Assert.IsNotNull(packageTrack);
            Assert.IsFalse(string.IsNullOrWhiteSpace(packageTrack.TrackingNumber));
            VerifyPackageTrack(paquete, packageTrack);
            this.senderMoq.VerifyAll();
        }

        private static void VerifyPackageTrack(PackageRequest paquete, PackageRequestTrack packageTrack)
        {
            foreach (var doc in paquete.Documents)
            {
                Assert.AreEqual(1, packageTrack.DocumentAnalysysRequestTracks.
                    Where((x) => x.DocumentUniqueRefences.Equals(doc.Guid)).
                    Count());
            }
            foreach (var documentTranck in packageTrack.DocumentAnalysysRequestTracks)
            {
                Assert.AreEqual(1, paquete.Documents.
                    Where((x) => x.Guid.Equals(documentTranck.DocumentUniqueRefences)).
                    Count());
                Assert.IsFalse(string.IsNullOrWhiteSpace(documentTranck.TrackingNumber));
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(DocumentAnalysisException))]
        public async Task SendRequest_ComunicationException_Exception()
        {

            PackageRequest paquete = PackageRequestTest.ValidPackage();
            paquete.Documents = new List<DocumentAnalysisRequest>() {
                DocumentAnalysisRequestTest.ValidRequest(),
                DocumentAnalysisRequestTest.ValidRequest(),
            };

            this.senderMoq.Setup(
                e => e.Send(
                       this.conf.ServicesBusCola,
                       It.Is<Message<DocumentAnalysisRequest>>(u => ValidateMessageContent(paquete, u)),
                       default))
            .Returns(Task.Run(() => { throw new Exception("Error"); }));

            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.SendRequestAsync(paquete);

            this.senderMoq.VerifyAll();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetAnalysisAsync_NullContext_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.GetAnalysisAsync(null, REF_DOC);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public async Task GetAnalysisAsync_InvalidContext_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.GetAnalysisAsync(AnalysisContextTests.InvalidValidContext(), REF_DOC);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetAnalysisAsync_DocRefNull_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.GetAnalysisAsync(AnalysisContextTests.ValidContext(), null);

        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetAnalysisAsync_DocRefVacio_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.GetAnalysisAsync(AnalysisContextTests.ValidContext(), "");

        }
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task GetAnalysisAsync_DocRefSoloEspacio_Exception()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            await theClient.GetAnalysisAsync(AnalysisContextTests.ValidContext(), "   ");

        }

        [TestMethod()]
        public async Task GetAnalysisAsync_Valid_ValidateURL()
        {
			//https://urlservicioanalisisdoc.es/?api/DocumentAnalysis/GetAnalysis/T/O/33
			//https://urlservicioanalisisdoc.es:443/?App=A&Owner=O&Tenant=T&Hash=33

			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                queryString.Add(nameof(StatusRequest.App), theContext.App);
                queryString.Add(nameof(StatusRequest.Owner), theContext.Owner);
                queryString.Add(nameof(StatusRequest.Tenant), theContext.Tenant);
                queryString.Add(nameof(StatusRequest.DocumentId), DOC_REF);
                
				UriBuilder theUriBuilder = new(this.conf.URLServicioAnalisisDoc)
                {
                    Path= MessagingClient.GetAnalysisEndPoint,
					Query = queryString.ToString()
				};

				Assert.AreEqual(theUriBuilder.Uri, request!.RequestUri);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            });
            await GetAnalysisAsyncDocRefTestHttpRequest(handler);
        }

        private async Task<DocumentAnalysisResponse> GetAnalysisAsyncDocRefTestHttpRequest(HttpMessageHandlerMoq handler)
        {
            this.factMoq.Setup<HttpClient>(e => e.CreateClient(MessagingClient.CLIENT_ID))
                .Returns(new HttpClient(handler));

            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            try
            {
                return await theClient.GetAnalysisAsync(this.theContext, DOC_REF);
            }
            finally
            {
                this.factMoq.VerifyAll();
                handler.Verify();
            }
        }


        [TestMethod]
        public async Task GetAnalysisAsync_OneDocumentResponse_Return()
        {
            var doc = new DocumentAnalysisResponse()
            {
                Description = "Des",
                DocumentName = "DocName",
                DocumentUniqueRefences = "DocRef",
                Organo = "Organo",
                Status = AnalysisStatus.Pending,
                Type = AnalysisTypes.Demand
            };

            var r = new List<DocumentAnalysisResponse>() {
                doc
            };

            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(r))
                };
            });
            var document = await GetAnalysisAsyncDocRefTestHttpRequest(handler);
            Assert.AreEqual(document, doc);
        }


        [TestMethod]
        [ExpectedException(typeof(DocumentAnalysisException))]
        public async Task GetAnalysisAsync_MoreThanOneDocumentResponse_Exception()
        {
            var doc = new DocumentAnalysisResponse()
            {
                Description = "Des",
                DocumentName = "DocName",
                DocumentUniqueRefences = "DocRef",
                Organo = "Organo",
                Status = AnalysisStatus.Pending,
                Type = AnalysisTypes.Demand
            };

            var r = new List<DocumentAnalysisResponse>() {
                doc,
                doc
            };

            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(r))
                };
            });
            await GetAnalysisAsyncDocRefTestHttpRequest(handler);
        }

        [TestMethod]
        public async Task GetAnalysisAsync_EmptyDocumentResponse_Null()
        {

            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("")
                };
            });
            Assert.IsNull(await GetAnalysisAsyncDocRefTestHttpRequest(handler));
        }


        [TestMethod]
        [ExpectedException(typeof(DocumentAnalysisException))]
        public async Task GetAnalysisAsync_InvalidFormatOneDocumentResponse_Exception()
        {

            var r = new List<String>() {
                "I'm not a valid JSon"
            };

            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(r))
                };
            });
            await GetAnalysisAsyncDocRefTestHttpRequest(handler);
        }


        [TestMethod()]
        public async Task GetAnalysisAsync_ValidNotDocRef_ValidateURL()
        {
			//https://urlservicioanalisisdoc.es/?api/DocumentAnalysis/GetAnalysis/T/O/33
			//https://urlservicioanalisisdoc.es/?api/DocumentAnalysis/GetAnalysis/T/O

			var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
                queryString.Add(nameof(StatusRequest.App), theContext.App);
                queryString.Add(nameof(StatusRequest.Owner), theContext.Owner);
                queryString.Add(nameof(StatusRequest.Tenant), theContext.Tenant);
                queryString.Add(nameof(StatusRequest.DocumentId), string.Empty);

				UriBuilder theUriBuilder = new(this.conf.URLServicioAnalisisDoc)
				{
					Path = MessagingClient.GetAnalysisEndPoint,
					Query = queryString.ToString()
				};

				Assert.AreEqual(theUriBuilder.Uri, request!.RequestUri);
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            });

            await GetAnalysisAsyncTestHttpRequest(handler);
        }


        [TestMethod()]
        public async Task GetAnalysisAsync_ValidNotDocRef_SeveralDocs()
        {

            var doc1 = new DocumentAnalysisResponse()
            {
                Description = "Des1",
                DocumentName = "DocName1",
                DocumentUniqueRefences = "DocRef1",
                Organo = "Organo2",
                Status = AnalysisStatus.Pending,
                Type = AnalysisTypes.Demand
            };
            var doc2 = new DocumentAnalysisResponse()
            {
                Description = "Des2",
                DocumentName = "DocName2",
                DocumentUniqueRefences = "DocRef2",
                Organo = "Organo2",
                Status = AnalysisStatus.Done,
                Type = AnalysisTypes.Undefined
            };
            var r = new List<DocumentAnalysisResponse>() { doc1, doc2 };
            var handler = new HttpMessageHandlerMoq(1, (num, request) =>
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(r))
                };
            });

            IEnumerable<DocumentAnalysisResponse> result = await GetAnalysisAsyncTestHttpRequest(handler);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(1, result.Where((x) => x.Equals(doc1)).Count());
            Assert.AreEqual(1, result.Where((x) => x.Equals(doc2)).Count());
        }

        [TestMethod()]
        [ExpectedException(typeof(DocumentAnalysisException))]
        public async Task GetAnalysisAsync_InvalidContextNotDocRef_Error()
        {
            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);

            await theClient.GetAnalysisAsync(this.theContext);
        }

        private async Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsyncTestHttpRequest(HttpMessageHandlerMoq handler)
        {
            this.factMoq.Setup<HttpClient>(e => e.CreateClient(MessagingClient.CLIENT_ID))
               .Returns(new HttpClient(handler));

            theClient = new MessagingClient(this.senderMoq.Object, this.conf, this.factMoq.Object);
            try
            {
                return await theClient.GetAnalysisAsync(this.theContext);
            }
            finally
            {
                this.factMoq.VerifyAll();
                handler.Verify();
            }
        }


		

	}
}
