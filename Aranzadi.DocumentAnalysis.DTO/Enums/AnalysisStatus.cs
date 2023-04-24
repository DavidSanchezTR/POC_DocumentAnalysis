using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO.Enums
{
    public enum AnalysisStatus
    {
		Unknown = 0,
		Pending = 1,
		Done = 2,
		DoneWithErrors = 3,
        Error = 4,
		NotFound = 5
	}
}
