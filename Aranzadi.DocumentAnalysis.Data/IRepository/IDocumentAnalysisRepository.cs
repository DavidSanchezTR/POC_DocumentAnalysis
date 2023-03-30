using Aranzadi.DocumentAnalysis.Data;
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
        Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysisAsync(int LawfirmId);

        Task AddAnalysisDataAsync(DocumentAnalysisData data);
    }
}
