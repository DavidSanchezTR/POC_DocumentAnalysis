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

		public static string SasToken = "";
	}
}
