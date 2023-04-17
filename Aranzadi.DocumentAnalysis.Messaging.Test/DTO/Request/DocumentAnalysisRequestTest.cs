using Aranzadi.DocumentAnalysis.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging.Test.DTO.Request
{
    [TestClass()]
    public class DocumentAnalysisRequestTest
    {
        public static DocumentAnalysisRequest ValidRequest()
        {
            var re = new DocumentAnalysisRequest();
            re.Analysis = "Analysis";
            re.Subject = "Subject";
            re.Source = DocumentAnalysis.DTO.Enums.Source.LaLey;
            re.FromEmail = "mail@mail.com";
            re.ConversationID = "Conversation Id";
            re.EmailId = "mail ID";
            re.Document = new DocumentAnalysisFile()
            {
                Name = "Document name",
                Path = "Document url path",
                Hash = "Hash"
            };
            re.UserAnalysis = new UserAnalysis()
            {
                LawFirmId = 100,
                UserId = 50
            };
            Assert.IsTrue(re.Validate(), "Request generica, reutizada en test");
            return re;
        }

        [TestMethod()]
        public void Validate_ValidRequest_OK()
        {
            Assert.IsTrue(ValidRequest().Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_DocumentName_Error(string value)
        {
            var request = ValidRequest();
            request.Document.Name = value;
            Assert.IsFalse(request.Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_DocumentPath_Error(string value)
        {
            var request = ValidRequest();
            request.Document.Path = value;
            Assert.IsFalse(request.Validate());
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null value")]
        [DataRow("", DisplayName = "empty value")]
        [DataRow("   ", DisplayName = "white space value")]
        public void Validate_DocumentHash_Error(string value)
        {
            var request = ValidRequest();
            request.Document.Hash = value;
            Assert.IsFalse(request.Validate());
        } 

    }
}
