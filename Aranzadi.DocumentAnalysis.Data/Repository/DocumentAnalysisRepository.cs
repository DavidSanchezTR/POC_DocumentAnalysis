using Aranzadi.DocumentAnalysis.Data.Entities;
using Aranzadi.DocumentAnalysis.Data.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Data.Repository
{
    internal class DocumentAnalysisRepository : IDocumentAnalysisRepository
    {
        public Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysisAsync(int LawfirmId)
        {
            throw new NotImplementedException();
        }
    }
}
