using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Aranzadi.AnalisisDocumentosTestCore")]

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public interface IAnalisisDocumentosFactory
    {
        IClient GetClient();
        IConsumer GetConsumer();
    }
}