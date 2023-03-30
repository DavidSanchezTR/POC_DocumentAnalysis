using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Response
{

    public enum AnalysisStatus
    {
        Unknown = 0,
        Pending = 1,
        Done = 2,
        DoneWithErrors = 3,
        FatalError = 4
    }

}
