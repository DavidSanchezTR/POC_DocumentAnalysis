
using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using Aranzadi.DocumentAnalysis.Data.Repository;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Enums;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Runtime.CompilerServices;
using ThomsonReuters.BackgroundOperations.Messaging.Models;

namespace Aranzadi.DocumentAnalysis.Services
{
    public class QueuedHostedService : BackgroundService
    {
        public const string MESSAGE_SOURCE_FUSION = "Fusion";
        public const string MESSAGE_TYPE_DOCUMENT_ANALYSIS = "DocumentAnalysis";

        static string objLock = "";
        static IConsumer consumer = null;
        public static AnalysisContext Contexto = null;

        private readonly ILogger<QueuedHostedService> _logger;
        private readonly DocumentAnalysisOptions configuration;
        private readonly TelemetryClient telemetryClient;

        public IServiceProvider serviceProvider { get; }

        public QueuedHostedService(ILogger<QueuedHostedService> logger
            , IServiceProvider serviceProvider
            , DocumentAnalysisOptions configuration
            , TelemetryClient telemetryClient)
        {
            _logger = logger;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.telemetryClient = telemetryClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"{nameof(QueuedHostedService)} running at: {DateTimeOffset.Now}");

                if (consumer == null)
                {
                    lock (objLock)
                    {

                        var conf = new MessagingConfiguration();
                        conf.ServicesBusConnectionString = configuration.ServiceBus.ConnectionString;
                        conf.ServicesBusCola = configuration.ServiceBus.Queue;
                        conf.Source = MESSAGE_SOURCE_FUSION;
                        conf.Type = MESSAGE_TYPE_DOCUMENT_ANALYSIS;

                        var factory = new AnalisisDocumentosDefaultFactory(conf);

                        if (consumer == null)
                        {
                            consumer = factory.GetConsumer();
                            consumer.StartProcess(ProcessMessage);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
                await telemetryClient.FlushAsync(stoppingToken);
                throw;
            }
        }


        private async Task<bool> ProcessMessage(AnalysisContext context, DTO.Request.DocumentAnalysisRequest request)
        {
            try
            {             
                var data = new DocumentAnalysisData
                {
                    App = context.App,
                    TenantId = context.Tenant.ToString(),
                    UserId = context.Owner,
                    Analysis = null,
                    Status = StatusResult.Pendiente,
                    AnalysisDate = DateTimeOffset.Now,
                    CreateDate = DateTimeOffset.Now,
                    Source = Source.LaLey,
                    DocumentName = request.Name,
                    AccessUrl = request.Path,
                    Sha256 = "" // Calcular el Hash aqui en el paquete
                };
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    IDocumentAnalysisRepository documentAnalysisRepository =
                        scope.ServiceProvider.GetRequiredService<IDocumentAnalysisRepository>();

                    await documentAnalysisRepository.AddAnalysisDataAsync(data);
                }

                await Task.Delay(0);
                return true;
            }
            catch (Exception ex)
            {
                telemetryClient.TrackException(ex);
                telemetryClient.Flush();
                throw;
            }
        }

    }
}
