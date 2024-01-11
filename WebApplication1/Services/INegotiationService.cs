using Microsoft.AspNetCore.Mvc;
using NegotiationsApi.Models;

namespace NegotiationsApi.Services
{
    public interface INegotiationService
    {
        Task<ActionResult<NegotiationModel>> AddNegotiation(int productId, int customerId, decimal proposedPrice);
        Task<ActionResult<NegotiationModel>> UpdateNegotiation(int productId, int customerId, decimal proposedPrice);
        Task<ActionResult<NegotiationModel>> EmployeeUpdateNegotiation(int id, NegotiationModel.NegotiationStatus status);
        Task<IEnumerable<NegotiationModel>> GetAllNegotiations();
        Task<NegotiationModel> GetNegotiation(int id);
    }
}
