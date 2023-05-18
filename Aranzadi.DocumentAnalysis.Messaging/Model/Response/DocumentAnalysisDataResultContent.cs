using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging.Model.Response
{
    public class DocumentAnalysisDataResultContent
    {
        public DocumentAnalysisDataResultJudgement Juzgado { get; set; }
        public DocumentAnalysisDataResultProcedure Procedimiento { get; set; }
        public DocumentAnalysisDataResolution Resolucion { get; set; }
        public DocumentAnalysisDataResultReview Review { get; set; }
        public string Ocr { get; set; }

    }
}