using Aranzadi.DocumentAnalysis.Util;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	public class AssemblyApp
	{
		public static WebApplicationBuilder builder = null;
		public static DocumentAnalysisOptions? documentAnalysisOptions = null;
		public static WebApplication? app = null;

		public static string SasToken = "https://ukiflxcustomerqa1.blob.core.windows.net/dastest/TestDAS.zip?sp=r&st=2023-08-03T12:15:43Z&se=2028-08-03T20:15:43Z&spr=https&sv=2022-11-02&sr=b&sig=JyliI7l68vEwTtiCxig2FtX8rJo26UM%2BYFWSSwSs0qU%3D";
	}
}
