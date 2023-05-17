using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Aranzadi.DocumentAnalysis.Messaging.Model.Request
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
