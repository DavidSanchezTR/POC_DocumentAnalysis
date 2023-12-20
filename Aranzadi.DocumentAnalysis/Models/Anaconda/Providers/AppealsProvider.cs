using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;
using Serilog;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
    public class AppealsProvider : JudicialNotificationProvider, IAppealsProvider
    {
		internal const string EVENT_ADMISION_A_TRAMITE = "Admisión a trámite";
		internal const string ENTITY_CUANTIA_INICIAL = "Cuantía inicial";
		internal const string ENTITY_CUANTIA_ADMITIDA = "Cuantía admitida";

		private Amount _initialAmount;
		public Amount InitialAmount
		{
			get { return _initialAmount; }
		}

		private Amount _recognizedAmount;
		public Amount RecognizedAmount
		{
			get { return _recognizedAmount; }
		}

		public AppealsProvider(DocumentAnalysisAnaconda? documentAnalysisAnaconda)
            : base(documentAnalysisAnaconda)
        {
			if (documentAnalysisAnaconda == null)
			{
				throw new ArgumentNullException(nameof(documentAnalysisAnaconda));
			}
			if (documentAnalysisAnaconda.Fragments != null)
			{
				foreach (var fragment in documentAnalysisAnaconda.Fragments)
				{
					if (fragment.Events != null)
					{
						foreach (var even in fragment.Events)
						{
							if (AppealsProvider.IsValidAdmisionTramite(even))
							{
								if (even.Entities != null)
								{
									foreach (var entity in even.Entities)
									{
										if (ENTITY_CUANTIA_INICIAL.Equals(entity.Type))
										{
											if (!Amount.TryParse(entity.Value, out _initialAmount))
											{
												Log.Warning($"Parse not realized of '{entity.Value}'");
											}
										}
										else if (ENTITY_CUANTIA_ADMITIDA.Equals(entity.Type))
										{
											if (!Amount.TryParse(entity.Value, out _recognizedAmount))
											{
												Log.Warning($"Parse not realized of '{entity.Value}'");
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}        

		internal static bool IsValidAdmisionTramite(EventAnaconda eventAnaconda)
		{
			if (eventAnaconda == null)
			{
				return false;
			}

			return EVENT_ADMISION_A_TRAMITE.Equals(eventAnaconda.Type);
		}

	}
}
