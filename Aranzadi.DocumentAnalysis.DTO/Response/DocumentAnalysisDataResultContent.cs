using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultContent
    {
        public DocumentAnalysisDataResultJudgement juzgado { get; set; }
        public DocumentAnalysisDataResultProcedure procedimiento { get; set; }
        public DocumentAnalysisDataResolution resolucion { get; set; }
        public DocumentAnalysisDataResultReview review { get; set; }
        public string ocr { get; set; }

    }
}