using Aranzadi.DocumentAnalysis.Data;
using System.Text.Json.Serialization;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

namespace Aranzadi.DocumentAnalysis.Configuration
{
    public class ConfigurationServicesApplication
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {

            builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            string connString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddCosmos<DocumentAnalysisDbContext>(connString, builder.Configuration.GetValue<string>(nameof(DocumentAnalysisOptions.CosmosDatabaseName)) ?? "AnalysisService", (cosmosOptions) =>
            {
                cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(5));
            });

            builder.Services.AddSingleton(builder.Configuration);
            builder.Services.Configure<DocumentAnalysisOptions>(builder.Configuration);
            if (!builder.Environment.IsDevelopment()) { }
               
            builder.Services.AddTransient((e) =>
            {
                return new DocumentAnalysisEnvironment(builder.Environment.ContentRootPath, builder.Environment);
            });

            #region SERVICES

            builder.Services.AddTransient<IDocumentAnalysisService, DocumentAnalysisService>();

            #endregion
        }

    }
}
