using System;
using System.Collections.Generic;
using System.Text;
using Aranzadi.DocumentAnalysis.DTO;

namespace Aranzadi.DocumentAnalysis.DTO.Request
{
    public class DocumentAnalysisRequest : IValidable
    {
        public string DocumentName { get; set; }

        public string DocumentUniqueRefences { get; set; }

        public Uri DocumentAccesURI { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(DocumentName) ||
                string.IsNullOrWhiteSpace(DocumentUniqueRefences) ||
                DocumentAccesURI == null)
            {
                return false;
            }
            return true;
        }
    }
}
