namespace Aranzadi.DocumentAnalysis.Models.Anaconda.Providers.Attributes
{
    internal class DocumentAnalysisEntityAttribute : EntityAttribute
    {
		internal DocumentAnalysisEntityAttribute(string entityName) : base(entityName)
        {
        }
		internal DocumentAnalysisEntityAttribute(string entityName, Type theType) : base(entityName, theType) { }

    }
}
