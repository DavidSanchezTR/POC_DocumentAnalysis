using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO
{
    public class StatusRequest
    {
        public string Aplication { get; set; }

        public string Tenant { get; set; }

        public string Owner { get; set; }

        public string DocumentReference { get; set; }
    }
}
