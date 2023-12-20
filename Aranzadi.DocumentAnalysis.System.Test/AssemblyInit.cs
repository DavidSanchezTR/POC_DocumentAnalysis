using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;
using Aranzadi.DocumentAnalysis.Models.CreditReservations;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;

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
			ConfigurationServicesApplication.ConfigureServices(AssemblyApp.builder, AssemblyApp.documentAnalysisOptions);
			AssemblyApp.app = AssemblyApp.builder.Build();

			AssemblyApp.TenantId = AssemblyApp.builder.Configuration.GetValue<string>("TenantIdForTest");
			AssemblyApp.UserId = AssemblyApp.builder.Configuration.GetValue<string>("UserIdForTest");
			AssemblyApp.sasToken = AssemblyApp.builder.Configuration.GetValue<string>("SasToken");
			AssemblyApp.urlBaseDocumentAnalysisService = AssemblyApp.builder.Configuration.GetValue<string>("UrlDocumentAnalysisService");


			///Healthcheck
			var healthCheckUri = new Uri(new Uri(AssemblyApp.urlBaseDocumentAnalysisService), $"Healthcheck");
			using HttpClient client = new HttpClient();
			var response = client.GetAsync(healthCheckUri).Result;
			var stringResponse = response.Content.ReadAsStringAsync().Result;

			if (response.StatusCode != HttpStatusCode.OK
				|| stringResponse != "Healthy")
			{
				throw new HttpRequestException($"Check if url {healthCheckUri.AbsoluteUri} is OK");
			}

		}
	}
}