using Aranzadi.DocumentAnalysis.Models.CreditReservations;
using Aranzadi.DocumentAnalysis.Models;
using Aranzadi.DocumentAnalysis.Models.CreditConsumption;

namespace Aranzadi.DocumentAnalysis.Services.IServices
{
	public interface ICreditsConsumptionClient
	{
		Task<OperationResult<TenantCreditReservation>> CreateReservation(NewTenantCreditReservation reservation);
		Task<OperationResult> CompleteReservation(string tenantCreditId, string reservationId);
		Task<OperationResult<CreditResponse>> GetTenantCredits(string userAccountId, string creditTemplateID);


    }
}
