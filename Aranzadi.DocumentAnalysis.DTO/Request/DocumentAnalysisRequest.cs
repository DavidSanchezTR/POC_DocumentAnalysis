using System;
using System.Collections.Generic;
using System.Text;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Enums;

namespace Aranzadi.DocumentAnalysis.DTO.Request
{
    public class DocumentAnalysisRequest : IValidable
    {
        public string DocumentName { get; set; }

        public string Analysis { get; set; }

        public Source Source { get; set; }

        public string DocumentUniqueRefences { get; set; }

        public string AccessUrl { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(DocumentName) ||
                string.IsNullOrWhiteSpace(DocumentUniqueRefences) ||
                AccessUrl == null)
            {
                return false;
            }
            return true;
        }
    }
}
