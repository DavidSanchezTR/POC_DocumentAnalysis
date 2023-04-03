namespace Aranzadi.DocumentAnalysis.Data.Entities
{
    public class DocumentAnalysisResult
    {
        public Guid DocumentId { get; set; }

        public string? Analysis { get; set; }

        public StatusResult Status { get; set; }

        public enum StatusResult
        {
            Pendiente = 1,
            EnCurso = 2,
            Disponible = 3, 
            NoConcluyente = 4,
            Erroneo = 5
        }
    }
}