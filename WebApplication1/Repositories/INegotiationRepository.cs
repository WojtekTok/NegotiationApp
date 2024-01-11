using NegotiationsApi.Models;

namespace NegotiationsApi.Repositories
{
    public interface INegotiationRepository
    {
        Task<bool> NegotiationExists(int productId, int customerId);
        Task<IEnumerable<NegotiationModel>> GetAllNegotiationsAsync();
        Task<NegotiationModel> GetNegotiationAsync(int productId, int customerId);
        Task<NegotiationModel> GetNegotiationByIdAsync(int id);
        Task AddNegotiationAsync(NegotiationModel negotiation);
        Task UpdateNegotiationAsync(NegotiationModel negotiation);
    }
}
