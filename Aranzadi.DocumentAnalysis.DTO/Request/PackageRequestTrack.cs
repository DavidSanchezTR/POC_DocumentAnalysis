using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Request
{
    public class PackageRequestTrack
    {
        public string TrackingNumber { get; set; }
        public IEnumerable<DocumentAnalysisRequestTrack> DocumentAnalysysRequestTracks { get; set; }
    }
}
