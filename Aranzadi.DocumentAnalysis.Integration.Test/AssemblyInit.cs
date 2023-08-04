using Aranzadi.DocumentAnalysis.Configuration;
using Aranzadi.DocumentAnalysis.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

			ConfigurationServicesApplication.ConfigureServices(AssemblyApp.builder, AssemblyApp.documentAnalysisOptions);

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
