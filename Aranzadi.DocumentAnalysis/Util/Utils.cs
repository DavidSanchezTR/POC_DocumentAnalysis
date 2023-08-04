using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Serilog;

namespace Aranzadi.DocumentAnalysis.Util
{
	public class Utils
	{
		private const int RETRYCOUNTS = 3;
		public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
		{
			return HttpPolicyExtensions.HandleTransientHttpError()
				.WaitAndRetryAsync(retryCount: RETRYCOUNTS
				, attempt => TimeSpan.FromSeconds(1 * attempt)
				, onRetry: (exception, counter, context) =>
				{
					Log.Warning($"Retry call in {counter}", exception?.Exception);
				});
		}

	}
}
