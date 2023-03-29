using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.Messaging
{
    public class DocumentAnalysisException:Exception
    {
        public DocumentAnalysisException(string message):base(message) { }

        public DocumentAnalysisException(string message,Exception ex) : base(message,ex) { }
    }
}
