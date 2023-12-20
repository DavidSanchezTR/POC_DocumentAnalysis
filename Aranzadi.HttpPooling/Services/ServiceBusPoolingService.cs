using Aranzadi.DocumentAnalysis.Messaging;
using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.HttpPooling.Interfaces;
using Aranzadi.HttpPooling.Models;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Context;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;

namespace Aranzadi.HttpPooling.Services
{
	public abstract class ServiceBusPoolingService : IHttpPoolingServices
	{
		private const int RETRYCOUNTS = 3;
		private const string ORIGIN = "Pooling";

		private readonly PoolingConfiguration _poolingConfiguration;
		private IHttpClientFactory _httpClientFactory;
		private ServiceBusSender _sender;
		private ServiceBusProcessor _processor;
		private ServiceBusClient _client;
		private ServiceBusReceiver _dlqReceiver;

		public ServiceBusPoolingService(
			PoolingConfiguration poolingConfiguration,
			IHttpClientFactory factory)
		{
			_poolingConfiguration = poolingConfiguration;
			_httpClientFactory = factory;
		}

		public async Task Start()
		{
			await ConfigureServicesBusAccess();
		}

		private async Task ConfigureServicesBusAccess()
		{
			string connectionString = _poolingConfiguration.Messaging.Endpoint;
			string queueName = _poolingConfiguration.Messaging.Queue;

			_client = new ServiceBusClient(connectionString);

			var options = new ServiceBusProcessorOptions
			{
				AutoCompleteMessages = false,
				MaxConcurrentCalls = 100,
			};

			_processor = _client.CreateProcessor(queueName, options);
			_processor.ProcessMessageAsync += MessageHandler;
			_processor.ProcessErrorAsync += ErrorHandler;
			await _processor.StartProcessingAsync();

			_sender = _client.CreateSender(queueName);
		}

		public async Task AddRequest(HttpPoolingRequest request)
		{
			await AddRequest(request, 0);
		}

		private async Task AddRequest(HttpPoolingRequest request, int intento)
		{
			var m = new PooledRequestDTO() { Request = request, NIntento = intento };
			ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject(m));
			TimeSpan delayTime = GetDelayTimeFromLastTry(intento);
			message.ScheduledEnqueueTime = DateTime.UtcNow.AddTicks(delayTime.Ticks);
			if (_sender != null)
			{
				await _sender.SendMessageAsync(message);
				Log.Warning($"Intento {intento} en {delayTime.ToString("c")} para el Id {request.ExternalIdentificator}");
			}
			else
			{
				throw new NullReferenceException("_sender is null, not created ServiceBusSender");
			}
		}

		private async Task ErrorHandler(ProcessErrorEventArgs arg)
		{
			LogContext.PushProperty("Origin", ORIGIN);
			Log.Error(arg.Exception, arg.Exception.Message);
		}

		private async Task MessageHandler(ProcessMessageEventArgs args)
		{
			LogContext.PushProperty("Origin", ORIGIN);

			var polledRequest = TryGetMessageDTO(args);			

			if (polledRequest != null)
			{				
				LogContext.PushProperty("DocumentId", polledRequest.Request?.ExternalIdentificator);
				LogContext.PushProperty("RetryCount", polledRequest.NIntento);
				LogContext.PushProperty("UrlAnalysisProvider", polledRequest.Request?.Url);

				HttpRequestMessage httpReq = new HttpRequestMessage();
				PrepareRequest(polledRequest.Request, httpReq);

				using HttpClient client = _httpClientFactory.CreateClient(GetClientId());
				var httpResponse = await HttpPolicyExtensions.HandleTransientHttpError()
				.WaitAndRetryAsync(retryCount: RETRYCOUNTS
				, attempt => TimeSpan.FromSeconds(1 * attempt)
				, onRetry: (exception, counter, context) =>
				{
					Log.Warning($"Retry call in {counter}", exception?.Exception);
				}).ExecuteAsync(async () =>
				{
					return await client.SendAsync(httpReq);
				});
				
				if (await Retry(httpResponse, polledRequest))
				{					
					var totalDelayTime = GetDelayTimeFromFirstTry(polledRequest.NIntento + 1);
					if (totalDelayTime < _poolingConfiguration.AvailableTime)
					{
						await SetRetry(polledRequest);						
					}
					else
					{
						//Se ha superado el plazo de reintentos. El analisis se cancela
						Log.Warning($"El analisis con Id {polledRequest.Request.ExternalIdentificator} se cancela " +
							$"despues de {polledRequest.NIntento + 1} intentos y " +
							$"de superar el plazo de tiempo limite de {_poolingConfiguration.AvailableTime.ToString("c")}");
						await CancelAnalysis(httpResponse, polledRequest);						
					}
					await args.CompleteMessageAsync(args.Message);
				}
				else
				{
					await ProcessResponseAnalysis(httpResponse, polledRequest);
					await args.CompleteMessageAsync(args.Message);
				}
			}
		}

		private TimeSpan GetDelayTimeFromLastTry(int retryNumber)
		{
			var ts = retryNumber * TimeSpan.FromSeconds(_poolingConfiguration.RetryTimeInSeconds);
			return ts;
		}

		private TimeSpan GetDelayTimeFromFirstTry(int retryNumber)
		{
			TimeSpan ts = new TimeSpan(0);
			for (int i = 0; i <= retryNumber; i++)
			{
				ts = ts + GetDelayTimeFromLastTry(i);
			}
			return ts;
		}

		protected async Task SetRetry(PooledRequestDTO polledRequest)
		{
			await AddRequest(new HttpPoolingRequest()
			{
				Url = polledRequest.Request.Url,
				ExternalIdentificator = polledRequest.Request.ExternalIdentificator
			}, polledRequest.NIntento + 1);
		}

		private PooledRequestDTO TryGetMessageDTO(ProcessMessageEventArgs args)
		{
			try
			{
				return JsonConvert.DeserializeObject<PooledRequestDTO>(args.Message.Body.ToString());
			}
			catch (Exception ex)
			{
				Log.Error("Error getting PolledRequestDTO from Services Bus Qe, ", ex);
				return null;
			}
		}

		protected virtual string GetClientId()
		{
			return "";
		}

		protected virtual void PrepareRequest(HttpPoolingRequest polledRequest, HttpRequestMessage req)
		{
			req.RequestUri = new Uri(polledRequest.Url);
		}

		protected virtual string GetUrlAnalysisJob()
		{
			return "";
		}

		public abstract Task<bool> Retry(HttpResponseMessage responseService, PooledRequestDTO polledRequest);

		public abstract Task<bool> ProcessResponseAnalysis(HttpResponseMessage responseService, PooledRequestDTO polledRequest);

		public abstract Task<bool> CancelAnalysis(HttpResponseMessage responseService, PooledRequestDTO polledRequest);

		public async Task Stop()
		{
			await _processor.CloseAsync().ConfigureAwait(false);
			_processor = null;
			await _sender.CloseAsync().ConfigureAwait(false);
			_sender = null;
			if (_dlqReceiver != null)
			{
				await _dlqReceiver.CloseAsync().ConfigureAwait(false);
				_dlqReceiver = null;
			}
			await _client.DisposeAsync().ConfigureAwait(false);
			_client = null;


		}

	}
}

