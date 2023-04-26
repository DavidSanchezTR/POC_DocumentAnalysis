using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO
{
    public class StatusRequest
    {
        public string App { get; set; }

        public string Tenant { get; set; }

        public string Owner { get; set; }

        public string DocumentId { get; set; }

    }
}
