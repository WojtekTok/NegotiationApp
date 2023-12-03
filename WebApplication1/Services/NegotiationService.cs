using Microsoft.AspNetCore.Mvc;
using NegotiationsApi.Services;
using NegotiationsApi.Data;
using NegotiationsApi.Models;

namespace NegotiationsApi.Services
{
    public class NegotiationService : INegotiationService
    {
        private readonly AppDbContext _context;

        public NegotiationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<NegotiationModel>> AddNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            if (_context.NegotiationModel.Any(n => n.ProductId == productId && n.CustomerId == customerId))
                return new BadRequestObjectResult("Negocjacja o ten produkt już istnieje");
            else
            {
                int wasRejected = ProcessNegotiation(productId, customerId, proposedPrice, out NegotiationModel.NegotiationStatus status);
                var negotiation = new NegotiationModel()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    ProposedPrice = proposedPrice,
                    AttemptsLeft = 3 - wasRejected,
                    Status = status
                };
                _context.NegotiationModel.Add(negotiation);
                await _context.SaveChangesAsync();

                return new CreatedAtActionResult("GetNegotiationModel", "Negotiations", new { id = negotiation.Id }, negotiation);
            }
        }

        public async Task<ActionResult<NegotiationModel>> UpdateNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            if (!_context.NegotiationModel.Any(n => n.ProductId == productId && n.CustomerId == customerId))
                return new BadRequestObjectResult("Negocjacja o ten produkt nie istnieje");
            else
            {
                int wasRejected = ProcessNegotiation(productId, customerId, proposedPrice, out NegotiationModel.NegotiationStatus status);
                var existingNegotiation = _context.NegotiationModel
                    .FirstOrDefault(n => n.ProductId == productId && n.CustomerId == customerId);

                if (existingNegotiation.Status == NegotiationModel.NegotiationStatus.Pending)
                    return new BadRequestObjectResult("Ostatnia oferta oczekuje nadal oczekuje na decyzję.");

                if (existingNegotiation.AttemptsLeft == 0)
                    return new BadRequestObjectResult("Przekroczono liczbę możliwych propozycji dla tego produktu.");

                existingNegotiation.AttemptsLeft -= wasRejected;

                if (existingNegotiation.AttemptsLeft == 0)
                    status = NegotiationModel.NegotiationStatus.Rejected;

                existingNegotiation.Status = status;
                existingNegotiation.ProposedPrice = proposedPrice;

                await _context.SaveChangesAsync();

                return new OkObjectResult(existingNegotiation);
            }
        }

        private int ProcessNegotiation(int productId, int customerId, decimal proposedPrice, out NegotiationModel.NegotiationStatus status)
        {
            int wasRejected = 0;
            status = NegotiationModel.NegotiationStatus.Pending;
            var basePrice = _context.ProductModel.Find(productId)?.BasePrice;

            if (basePrice.HasValue && proposedPrice > basePrice * 2)
            {
                wasRejected = 1;
                status = NegotiationModel.NegotiationStatus.Rejected;
            }
            return wasRejected;
        }
    }
}
