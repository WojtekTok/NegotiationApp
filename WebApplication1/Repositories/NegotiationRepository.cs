using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Data;
using NegotiationsApi.Models;

namespace NegotiationsApi.Repositories
{
    public class NegotiationRepository : INegotiationRepository
    {
        private readonly AppDbContext _context;

        public NegotiationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> NegotiationExists(int productId, int customerId)
        {
            return await _context.NegotiationModel.AnyAsync(n => n.ProductId == productId && n.CustomerId == customerId);
        }

        public async Task<IEnumerable<NegotiationModel>> GetAllNegotiationsAsync()
        {
            return await _context.NegotiationModel.ToListAsync();
        }

        public async Task<NegotiationModel> GetNegotiationAsync(int productId, int customerId)
        {
            return await _context.NegotiationModel.FirstOrDefaultAsync(n => n.ProductId == productId && n.CustomerId == customerId);
        }

        public async Task<NegotiationModel> GetNegotiationByIdAsync(int id)
        {
            return await _context.NegotiationModel.FindAsync(id);
        }

        public async Task AddNegotiationAsync(NegotiationModel negotiation)
        {
            _context.NegotiationModel.Add(negotiation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNegotiationAsync(NegotiationModel negotiation)
        {
            _context.Entry(negotiation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
