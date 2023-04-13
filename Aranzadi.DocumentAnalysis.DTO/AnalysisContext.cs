using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Aranzadi.DocumentAnalysis.DTO
{
    public class AnalysisContext : IEquatable<AnalysisContext>, IValidable
    {

        public string Owner { get; set; }

        public string Tenant { get; set; }

        public string Account { get; set; }

        public string App { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as AnalysisContext);
        }

        public bool Equals(AnalysisContext other)
        {

            return !(other is null) &&
                CompareStringToUpperInvariant(Owner, other.Owner) &&
                CompareStringToUpperInvariant(Tenant, other.Tenant) &&
                CompareStringToUpperInvariant(Account, other.Account) &&
                CompareStringToUpperInvariant(App, other.App);
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
            return HashCode.Combine(App?.ToUpperInvariant(), Tenant?.ToUpperInvariant(), Owner);
        }

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Owner) &&
                !string.IsNullOrWhiteSpace(Tenant) &&
                !string.IsNullOrWhiteSpace(App);
        }

    }
}
