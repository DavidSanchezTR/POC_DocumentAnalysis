using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using Aranzadi.DocumentAnalysis.Messaging.Model.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public interface IClient
    {
        Task<DocumentAnalysisResponse> GetAnalysisAsync(AnalysisContext context, string documentUniqueIdentifier);

        Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(AnalysisContext context);

        Task<PackageRequestTrack> SendRequestAsync(PackageRequest theRequest);
    }
}