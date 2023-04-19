using System.Collections.Generic;
using System.Threading.Tasks;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.DTO.Response;

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public interface IClient
    {
        Task<DocumentAnalysisResponse> GetAnalysisAsync(AnalysisContext context, string documentUniqueIdentifier);

        Task<IEnumerable<DocumentAnalysisResponse>> GetAnalysisAsync(AnalysisContext context);

        Task<PackageRequestTrack> SendRequestAsync(PackageRequest theRequest);
    }
}