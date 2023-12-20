using Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response.Providers;

namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers
{
    public class InformationProvider : IInformationProvider
    {
		internal const string ENTITY_FECHA_NOTIFICACION = "Fecha notificación";

        public InformationProvider(DocumentAnalysisAnaconda documentAnalysisAnaconda)
        {
            if (documentAnalysisAnaconda == null)
            {
                throw new ArgumentNullException(nameof(documentAnalysisAnaconda));
            }
            this.NotificationDateDescription = string.Empty;
            EntityAttribute.SetPropertiesByAtribute<DocumentAnalysisEntityAttribute>(documentAnalysisAnaconda.Entities, this);
        }

        [DocumentAnalysisEntity(ENTITY_FECHA_NOTIFICACION, EntityAttribute.Type.DateTimeMaybeUTC)]
        public DateTime? NotificationDate { get; private set; }


        [DocumentAnalysisEntity(ENTITY_FECHA_NOTIFICACION, EntityAttribute.Type.String)]
        public string NotificationDateDescription { get; private set; }

    }
}
