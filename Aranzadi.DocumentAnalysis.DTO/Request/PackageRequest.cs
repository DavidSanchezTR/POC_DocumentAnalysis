using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using Aranzadi.DocumentAnalysis.DTO;

namespace Aranzadi.DocumentAnalysis.DTO.Request
{
    public class PackageRequest : IValidable
    {
        public AnalysisContext Context { get; set; }

        public string PackageUniqueRefences { get; set; }

        public IEnumerable<DocumentAnalysisRequest> Documents { get; set; }

        public bool Validate()
        {
            if (Context == null || !Context.Validate() ||
                string.IsNullOrWhiteSpace(PackageUniqueRefences) ||
                Documents == null || !Documents.Any() || Documents.Any((x) => !x.Validate()))
            {
                return false;
            }
            return true;
        }
    }
}
