using Aranzadi.DocumentAnalysis.Data;
using Aranzadi.DocumentAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aranzadi.DocumentAnalysis.Data.IRepository
{
    internal interface IDocumentAnalysisRepository
    {
        Task<IEnumerable<DocumentAnalysisResult>> GetAllAnalysis(int LawfirmId);
    }
}
