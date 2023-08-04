using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Integration.Test.Moq
{
	public class HttpMessageHandlerMoq : HttpMessageHandler
	{
		private readonly Func<int, HttpRequestMessage, HttpResponseMessage> sendAsyncFun;

		private int nOfCalls = 0;
		private int expectedCalls;

		public HttpMessageHandlerMoq(int nExpectedCalls, Func<int, HttpRequestMessage, HttpResponseMessage> validation)
		{
			this.sendAsyncFun = validation;
			this.nOfCalls = 0;
			this.expectedCalls = nExpectedCalls;
		}
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			this.nOfCalls++;
			HttpResponseMessage r = this.sendAsyncFun(this.nOfCalls, request);
			return Task.FromResult<HttpResponseMessage>(r);
		}

		public void Verify()
		{
			Assert.AreEqual(this.expectedCalls, this.nOfCalls);
		}

	}
}
