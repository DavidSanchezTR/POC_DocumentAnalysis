using Aranzadi.DocumentAnalysis.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.BackgroundOperations
{
    public class MessagingConfiguration : IValidable
    {
        public string ServicesBusConnectionString { get; set; }
        public string ServicesBusCola { get; set; }

        /// <summary>
        /// BackgroundOperationsFactory.MESSAGE_SOURCE_FUSION = "Fusion"
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// BackgroundOperationsFactory.MESSAGE_TYPE_ANALIS_DOC = "Analisis_Documento"
        /// </summary>
        public string Type { get; set; }

        public Uri URLOrquestador { get; set; }

        public Uri URLServicioAnalisisDoc { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(this.ServicesBusCola) ||
                string.IsNullOrWhiteSpace(this.ServicesBusConnectionString) ||
                string.IsNullOrWhiteSpace(this.Source) ||
                string.IsNullOrWhiteSpace(this.Type) ||
                this.URLOrquestador == null ||
                this.URLServicioAnalisisDoc == null
                )
            {
                return false;
            }
            return true;
        }
    }
}
