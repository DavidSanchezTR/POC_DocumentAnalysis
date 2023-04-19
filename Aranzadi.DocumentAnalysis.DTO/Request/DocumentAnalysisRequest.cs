using System;
using System.Collections.Generic;
using System.Text;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Enums;

namespace Aranzadi.DocumentAnalysis.DTO.Request
{

    public class DocumentAnalysisRequest : IValidable
    {

		public string Guid { get; set; }
		public string Name { get; set; }
		public string Path { get; set; }
        	

        public bool Validate()
        {
            if (
                string.IsNullOrWhiteSpace(Guid) ||
                string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Path))
            {
                return false;
            }
            return true;
        }
    }
}
