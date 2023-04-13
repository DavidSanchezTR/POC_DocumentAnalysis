using System;
using System.Collections.Generic;
using System.Text;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Enums;

namespace Aranzadi.DocumentAnalysis.DTO.Request
{

    public class DocumentAnalysisData : IValidable
    {

        //Provienen de la clase DocumentAnalysisData de Infolex
        
        public string EmailId { get; set; }

        public string Subject { get; set; }
        public string FromEmail { get; set; }
        public string ConversationID { get; set; }

        public DocumentAnalysisFile Document { get; set; }

        public UserAnalysis UserAnalysis { get; set; }


        public string Analysis { get; set; }

        public Source Source { get; set; }

        public bool Validate()
        {
            if (Document == null ||
                string.IsNullOrWhiteSpace(Document.Hash) ||
                string.IsNullOrWhiteSpace(Document.Path) ||
                string.IsNullOrWhiteSpace(Document.Name))
            {
                return false;
            }
            return true;
        }
    }
}
