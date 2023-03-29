using System.Collections.Generic;
using System.Threading.Tasks;
using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Request;
using Aranzadi.DocumentAnalysis.DTO.Response;

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public interface IClient
    {
        Task<DocumentResponse> GetAnalysisAsync(AnalysisContext context, string documentUniqueIdentifier);

        Task<IEnumerable<DocumentResponse>> GetAnalysisAsync(AnalysisContext context);

        Task<PackageRequestTrack> SendRequestAsync(PackageRequest theRequest);
    }
}