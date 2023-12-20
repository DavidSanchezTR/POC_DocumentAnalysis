using Aranzadi.DocumentAnalysis.Messaging.Model.Enums;

namespace Aranzadi.HttpPooling.Models
{
    public sealed class HttpPoolingRequest
    {
		/// <summary>
		/// Identificador del analisis del DAS
		/// </summary>
		public string ExternalIdentificator { get; set; }
		/// <summary>
		/// Url para recuperar el estado del análisis
		/// </summary>
        public string Url { get; set; }
	}

}
