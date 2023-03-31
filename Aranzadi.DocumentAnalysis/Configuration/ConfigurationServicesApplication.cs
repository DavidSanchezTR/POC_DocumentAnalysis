using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using static Aranzadi.DocumentAnalysis.DocumentAnalysisOptions;

namespace Aranzadi.DocumentAnalysis.Configuration
{
    public class ConfigurationServicesApplication
    {
        public static void ConfigureServices(WebApplicationBuilder builder)
        {
            DocumentAnalysisOptions documentAnalysisOptions = new DocumentAnalysisOptions();
            documentAnalysisOptions = ApplicationSettings.GetDocumentAnalysisOptions(builder.Configuration);
            builder.Services.AddSingleton(sp =>
            {
                return documentAnalysisOptions;
            });

            builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            string connString = documentAnalysisOptions.ConnectionStrings.DefaultConnection;

            builder.Services.AddDbContext<DocumentAnalysisDbContext>(dbOptions =>
            {
                dbOptions.UseCosmos(connString, documentAnalysisOptions.CosmosDatabaseName ?? "documentAnalisis", (cosmosOptions) =>
                {
                    cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(5));
                });
            });

            if (!builder.Environment.IsDevelopment()) { }
               
            builder.Services.AddTransient((e) =>
            {
                return new DocumentAnalysisEnvironment(builder.Environment.ContentRootPath, builder.Environment);
            });

            #region SERVICES

            builder.Services.AddTransient<IDocumentAnalysisService, DocumentAnalysisService>();
            builder.Services.AddTransient<IDocumentAnalysisRepository, DocumentAnalysisRepository>();

            #endregion
        }

    }
}
