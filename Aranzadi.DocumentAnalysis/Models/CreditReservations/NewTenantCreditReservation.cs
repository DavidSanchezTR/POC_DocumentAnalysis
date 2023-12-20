namespace Aranzadi.DocumentAnalysis.Models.CreditReservations
{
    public class NewTenantCreditReservation
    {
        public string CreditTemplateID { get; set; }
        public string TenantID { get; set; }
        public string UserID { get; set; }
    }

    public class TenantCreditReservation
    {
        public string ID { get; set; }

        public string UserID { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }

        public bool Completed { get; set; }

        public string TenantCreditID { get; set; }

        public string CreditTemplateID { get; set; }
    }

    public static class ReservationErrorDetailMessages
    {
        public const string NotFound = "Could not find the item";
        public const string NoTenantCredit = "Could not find the requested credit type for your tenant";
        public const string NoCreditTemplate = "The credit type does not exist";
        public const string NoLockAcquired = "Could not acquire lock for the resource. You may try again in a few seconds";
        public const string NoReservationsAvailable = "You have no credit reservations available";
    }
}
