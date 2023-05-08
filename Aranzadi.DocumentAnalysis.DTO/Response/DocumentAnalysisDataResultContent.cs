using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{
    public class DocumentAnalysisDataResultContent
    {
        public DocumentAnalysisDataResultJudgement Judgement { get; set; }
        public DocumentAnalysisDataResultProcedure Procedure { get; set; }
        public DocumentAnalysisDataResolution Resolution { get; set; }
        public DocumentAnalysisDataResultReview Review { get; set; }
        public string Ocr { get; set; }

    }
}
