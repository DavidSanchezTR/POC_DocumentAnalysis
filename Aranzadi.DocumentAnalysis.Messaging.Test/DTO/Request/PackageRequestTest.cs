using Aranzadi.DocumentAnalysis.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging.Test.DTO.Request
{
    [TestClass()]
    public class PackageRequestTest
    {

        public static PackageRequest ValidPackage()
        {
            var validPackage = new PackageRequest();
            validPackage.Context = AnalysisContextTests.ValidContext();
            validPackage.PackageUniqueRefences = "R";
            var analysisRequest = new List<DocumentAnalysisRequest>();
            var documentRequest = DocumentAnalysisRequestTest.ValidRequest();
            analysisRequest.Add(documentRequest);
            validPackage.Documents = analysisRequest;
            Assert.IsTrue(validPackage.Validate());
            return validPackage;
        }

        public static PackageRequest InvalidPackage()
        {
            var invalidatedPackage = ValidPackage();
            invalidatedPackage.Documents = null;
            Assert.IsFalse(invalidatedPackage.Validate());
            return invalidatedPackage;
        }

        [TestMethod()]
        public void Validate_Valid_OK()
        {
            Assert.IsTrue(ValidPackage().Validate());
        }

        [TestMethod()]
        public void Validate_ContextNull_Invalid()
        {
            var invalidatedPackage = ValidPackage();
            invalidatedPackage.Context = null!;
            Assert.IsFalse(invalidatedPackage.Validate());
        }
        [TestMethod()]
        public void Validate_InvalidContext_Invalid()
        {
            var invalidatedPackage = ValidPackage();
            invalidatedPackage.Context = AnalysisContextTests.InvalidValidContext();
            Assert.IsFalse(invalidatedPackage.Validate());
        }
        [TestMethod()]
        public void Validate_NullEmptyAplication_Invalid()
        {
            AnalysisContextTests.ValidStringPropierty((x, y) => x.PackageUniqueRefences = y, ValidPackage());
        }

        [TestMethod()]
        public void Validate_NullDocument_Invalid()
        {
            var invalidatedPackage = ValidPackage();
            invalidatedPackage.Documents = null;
            Assert.IsFalse(invalidatedPackage.Validate());
        }

        [TestMethod()]
        public void Validate_EmptyDocument_Invalid()
        {
            var invalidatedPackage = ValidPackage();
            invalidatedPackage.Documents = new List<DocumentAnalysisRequest>();
            Assert.IsFalse(invalidatedPackage.Validate());
        }

        [TestMethod()]
        public void Validate_InvalidDocument_Invalid()
        {
            var invalidatedPackage = ValidPackage();
            var l = new List<DocumentAnalysisRequest>();
            var request = DocumentAnalysisRequestTest.ValidRequest();
            request.Document.Name = "";
            l.Add(request);
            invalidatedPackage.Documents = l;
            Assert.IsFalse(invalidatedPackage.Validate());
        }

    }
}
