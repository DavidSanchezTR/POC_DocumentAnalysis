using System;
using System.Collections.Generic;
using System.Text;

namespace Aranzadi.DocumentAnalysis.DTO
{
    public class AnalysisContext : IEquatable<AnalysisContext>, IValidable
    {
        /// <summary>
        /// Fusion
        /// </summary>
        public string Aplication { get; set; }

        /// <summary>
        /// El Despacho
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// El Usuario
        /// </summary>
        public string Owner { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnalysisContext);
        }

        public bool Equals(AnalysisContext other)
        {

            return !(other is null) &&
                   CompareStringToUpperInvariant(Aplication, other.Aplication) &&
                   CompareStringToUpperInvariant(Tenant, other.Tenant) &&
                   CompareStringToUpperInvariant(Owner, other.Owner);
        }

        public static bool CompareStringToUpperInvariant(string v, string v1)
        {
            if (v == null)
            {
                return v1 == null;
            }
            else if (v1 == null)
            {
                return false;
            }
            return v.ToUpperInvariant() == v1.ToUpperInvariant();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Aplication?.ToUpperInvariant(), Tenant?.ToUpperInvariant(), Owner?.ToUpperInvariant());
        }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Owner) &&
                !string.IsNullOrWhiteSpace(Aplication) &&
                !string.IsNullOrWhiteSpace(Tenant);
        }

    }
}
