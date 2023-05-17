using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Request
{
    public class PackageRequestTrack
    {
        public string TrackingNumber { get; set; }
        public IEnumerable<DocumentAnalysisRequestTrack> DocumentAnalysysRequestTracks { get; set; }
    }
}
