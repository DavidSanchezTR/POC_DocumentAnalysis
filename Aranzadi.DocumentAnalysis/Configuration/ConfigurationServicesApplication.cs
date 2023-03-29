using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
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

            builder.Services.AddSingleton(builder.Configuration);
            builder.Services.Configure<DocumentAnalysisOptions>(builder.Configuration);
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
