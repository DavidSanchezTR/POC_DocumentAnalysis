using Aranzadi.DocumentAnalysis.Messaging.Model;
using Aranzadi.DocumentAnalysis.Messaging.Model.Request;
using System;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public interface IConsumer
    {
        void StartProcess(Func<AnalysisContext, DocumentAnalysisRequest, Task<bool>> theAction);
    }
}