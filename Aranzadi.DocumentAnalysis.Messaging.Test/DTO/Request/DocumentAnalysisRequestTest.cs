using Aranzadi.DocumentAnalysis.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
			re.Name = "DocumentName.pdf";
			re.Path = "http://urlToken.com";
			re.Guid = Guid.NewGuid().ToString();

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
			request.Name = value;
			Assert.IsFalse(request.Validate());
		}

		[TestMethod()]
		[DataRow(null, DisplayName = "null value")]
		[DataRow("", DisplayName = "empty value")]
		[DataRow("   ", DisplayName = "white space value")]
		public void Validate_DocumentPath_Error(string value)
		{
			var request = ValidRequest();
			request.Path = value;
			Assert.IsFalse(request.Validate());
		}

		[TestMethod()]
		[DataRow(null, DisplayName = "null value")]
		[DataRow("", DisplayName = "empty value")]
		[DataRow("   ", DisplayName = "white space value")]
		public void Validate_DocumentHash_Error(string value)
		{
			var request = ValidRequest();
			request.Guid = value;
			Assert.IsFalse(request.Validate());
		}

	}
}
