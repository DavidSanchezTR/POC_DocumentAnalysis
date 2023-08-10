using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Aranzadi.DocumentAnalysis.System.Test
{
	[TestClass]
	public class AssemblyInit
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			AssemblyApp.builder = WebApplication.CreateBuilder();
			AssemblyApp.documentAnalysisOptions = ApplicationSettings.InitConfiguration(AssemblyApp.builder, "Settings/appsettings.systemtest.json");

			AssemblyApp.sasToken = AssemblyApp.builder.Configuration.GetValue<string>("SasToken");
			AssemblyApp.urlBaseDocumentAnalysisService = AssemblyApp.builder.Configuration.GetValue<string>("UrlDocumentAnalysisService");

		}
	}
}