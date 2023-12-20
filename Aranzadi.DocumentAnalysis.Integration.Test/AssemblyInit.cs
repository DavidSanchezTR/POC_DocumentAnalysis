using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Aranzadi.DocumentAnalysis.Integration.Test
{
	[TestClass]
	public class AssemblyInit
	{

		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			AssemblyApp.builder = WebApplication.CreateBuilder();
			AssemblyApp.documentAnalysisOptions = ApplicationSettings.InitConfiguration(AssemblyApp.builder, "Settings/appsettings.test.json");
			AssemblyApp.TenantId = AssemblyApp.builder.Configuration.GetValue<string>("TenantIdForTest");
			AssemblyApp.UserId = AssemblyApp.builder.Configuration.GetValue<string>("UserIdForTest");
			AssemblyApp.SasToken = AssemblyApp.builder.Configuration.GetValue<string>("SasToken");

			ConfigurationServicesApplication.ConfigureServices(AssemblyApp.builder, AssemblyApp.documentAnalysisOptions);

			//Use Serilog
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(AssemblyApp.builder.Configuration)
				.WriteTo.AzureAnalytics(AssemblyApp.documentAnalysisOptions.LogAnalytics.WorkspaceId, AssemblyApp.documentAnalysisOptions.LogAnalytics.AuthenticationId, AssemblyApp.documentAnalysisOptions.LogAnalytics.LogName)
				.Enrich.FromLogContext()
				.CreateLogger();

			Log.Error("PRUEBA");

			AssemblyApp.app = AssemblyApp.builder.Build();

			#region CREATE DB IF NOT EXISTS
			using var scope = AssemblyApp.app.Services.CreateScope();
			using DocumentAnalysisDbContext dbContext = scope.ServiceProvider.GetRequiredService<DocumentAnalysisDbContext>();
			dbContext.Database.EnsureCreated();
			SeedDatabase.Seed(dbContext);
			#endregion CREATE DB IF NOT EXISTS

		}

	}

}
