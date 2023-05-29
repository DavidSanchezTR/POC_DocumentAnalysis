using Microsoft.ApplicationInsights.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Test
{

	/// <summary>
	/// A stub of <see cref="ITelemetryChannel"/>.
	/// </summary>
	public sealed class TelemetryChannelMock : ITelemetryChannel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StubTelemetryChannel"/> class.
		/// </summary>
		public TelemetryChannelMock()
		{
			this.OnSend = telemetry => { };
		}

		/// <summary>
		/// Gets or sets a value indicating whether this channel is in developer mode.
		/// </summary>
		public bool? DeveloperMode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating the channel's URI. To this URI the telemetry is expected to be sent.
		/// </summary>
		public string EndpointAddress { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to throw an error.
		/// </summary>
		public bool ThrowError { get; set; }

		/// <summary>
		/// Gets or sets the callback invoked by the <see cref="Send"/> method.
		/// </summary>
		public TelemetryAction OnSend { get; set; }

		/// <summary>
		/// Implements the <see cref="ITelemetryChannel.Send"/> method by invoking the <see cref="OnSend"/> callback.
		/// </summary>
		public void Send(ITelemetry item)
		{
			if (this.ThrowError)
			{
				throw new Exception("test error");
			}

			this.OnSend(item);
		}

		/// <summary>
		/// Implements the <see cref="IDisposable.Dispose"/> method.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Mock for the Flush method in <see cref="ITelemetryChannel"/>.
		/// </summary>
		public void Flush()
		{
		}
	}


	/// <summary>
	/// Required by Windows Runtime, which does not allow generics in public APIs.
	/// </summary>
	public delegate void TelemetryAction(ITelemetry telemetry);

}
