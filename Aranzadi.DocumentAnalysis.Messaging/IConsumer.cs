using Aranzadi.DocumentAnalysis.DTO;
using Aranzadi.DocumentAnalysis.DTO.Request;

using System;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public interface IConsumer
    {
        void StartProcess(Func<AnalysisContext, DocumentAnalysisData, Task<bool>> theAction);
    }
}