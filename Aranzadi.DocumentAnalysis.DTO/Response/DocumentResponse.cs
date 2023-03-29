using System;
using System.Collections.Generic;
using System.Text;
using Aranzadi.DocumentAnalysis.DTO;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentResponse : IEquatable<DocumentResponse>
    {


        public string DocumentName { get; set; }

        public string DocumentUniqueRefences { get; set; }

        public AnalysisStatus Status { get; set; }

        public AnalysisTypes Type { get; set; }

        public string Description { get; set; }

        public string Organo { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DocumentResponse);
        }
        public bool Equals(DocumentResponse other)
        {
            return !(other is null) &&
                AnalysisContext.CompareStringToUpperInvariant(DocumentName, other.DocumentName) &&
                AnalysisContext.CompareStringToUpperInvariant(DocumentUniqueRefences,
                    other.DocumentUniqueRefences) &&
                AnalysisContext.CompareStringToUpperInvariant(Description, other.Description) &&
                AnalysisContext.CompareStringToUpperInvariant(Organo, other.Organo) &&
                Status == other.Status &&
                Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DocumentName?.ToUpperInvariant(),
                DocumentUniqueRefences?.ToUpperInvariant(),
                Status,
                Type,
                Description?.ToUpperInvariant(),
                Organo?.ToUpperInvariant());
        }


    }

}
