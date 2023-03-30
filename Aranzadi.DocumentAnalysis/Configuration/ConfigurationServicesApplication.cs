﻿using Aranzadi.DocumentAnalysis.Data;
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

            builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            //string connString = "mongodb://uksouth-das-cosmos-dev:SlA98NPnfxekWsecVxydj3J3BTGtcfzWljltyNyaRAIRmsJqjIPLZfItRGZ9rsmT0nx9qcrwZVTCpeBaU12CKw==@uksouth-das-cosmos-dev.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@uksouth-das-cosmos-dev@";
            string connString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<DocumentAnalysisDbContext>(dbOptions =>
            {
                dbOptions.UseCosmos(connString, builder.Configuration.GetValue<string>(nameof(DocumentAnalysisOptions.CosmosDatabaseName)) ?? "documentAnalisis", (cosmosOptions) =>
                {
                    cosmosOptions.RequestTimeout(TimeSpan.FromMinutes(5));
                });
            });

			builder.Services.AddSingleton(sp =>
			{
				DocumentAnalysisOptions applicationSettings = new DocumentAnalysisOptions();
				builder.Configuration.Bind(applicationSettings);
				return applicationSettings;
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
