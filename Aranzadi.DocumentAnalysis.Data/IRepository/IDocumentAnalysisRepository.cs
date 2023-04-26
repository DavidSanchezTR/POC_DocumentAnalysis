﻿using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Data.IRepository
{
    public interface IDocumentAnalysisRepository
    {
        Task<int> AddAnalysisDataAsync(DocumentAnalysisData data);

        Task<int> UpdateAnalysisDataAsync(DocumentAnalysisData data);

		Task<IEnumerable<DocumentAnalysisResult>> GetAnalysisAsync(string TenantId, string UserId, string documentId);

		Task<DocumentAnalysisResult?> GetAnalysisDoneAsync(string sha256);


	}
}
