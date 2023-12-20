using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Aranzadi.DocumentAnalysis.Models.CreditConsumption
{
    public class TenantCredit
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "tenantID")]
        public string TenantId { get; set; }

        [JsonProperty(PropertyName = "tenantDisplayName")]
        public string TenantDisplayName { get; set; }

        [JsonProperty(PropertyName = "userID")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "userDisplayName")]
        public string UserDisplayName { get; set; }

        [JsonProperty(PropertyName = "freeReservations")]
        public int FreeReservations { get; set; }

        [JsonProperty(PropertyName = "creditTemplateID")]
        public string CreditTemplateId { get; set; }
    }

    public class CreditResponse
    {

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "items")]
        [DataMember]
        public ICollection<TenantCredit> TenantCredits { get; set; }
    }

    public class TenantCreditRechargeHistory
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "from")]
        public int From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public int To { get; set; }

        [JsonProperty(PropertyName = "byUser")]
        public string ByUser { get; set; }

        [JsonProperty(PropertyName = "rechargeDate")]
        public DateTimeOffset RechargeDate { get; set; }

        [JsonProperty(PropertyName = "tenantCreditID")]
        public string TenantCreditId { get; set; }
    }
}
