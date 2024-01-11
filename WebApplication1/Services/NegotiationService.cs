using Microsoft.AspNetCore.Mvc;
using NegotiationsApi.Services;
using NegotiationsApi.Data;
using NegotiationsApi.Models;
using NegotiationsApi.Repositories;

namespace NegotiationsApi.Services
{
    public class NegotiationService : INegotiationService
    {
        private readonly INegotiationRepository _negotiationRepository;
        private readonly IProductRepository _productRepository;

        public NegotiationService(INegotiationRepository negotiationRepository, IProductRepository productRepository)
        {
            _negotiationRepository = negotiationRepository;
            _productRepository = productRepository;
        }

        public async Task<ActionResult<NegotiationModel>> AddNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            if (await _negotiationRepository.NegotiationExists(productId, customerId))
                return new BadRequestObjectResult("Negocjacja o ten produkt już istnieje");
            else
            {
                (int wasRejected, NegotiationModel.NegotiationStatus status) = await ProcessNegotiationAsync(productId, proposedPrice);
                var negotiation = new NegotiationModel()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    ProposedPrice = proposedPrice,
                    AttemptsLeft = 3 - wasRejected,
                    Status = status
                };
                await _negotiationRepository.AddNegotiationAsync(negotiation);

                return new CreatedAtActionResult("GetNegotiation", "Negotiations", new { id = negotiation.Id }, negotiation);
            }
        }

        public async Task<ActionResult<NegotiationModel>> UpdateNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            if (!await _negotiationRepository.NegotiationExists(productId, customerId))
                return new BadRequestObjectResult("Negocjacja o ten produkt nie istnieje");
            else
            {
                (int wasRejected, NegotiationModel.NegotiationStatus status) = await ProcessNegotiationAsync(productId, proposedPrice);
                var existingNegotiation = await _negotiationRepository.GetNegotiationAsync(productId, customerId);

                if (existingNegotiation.Status == NegotiationModel.NegotiationStatus.Pending)
                    return new BadRequestObjectResult("Ostatnia oferta oczekuje nadal oczekuje na decyzję.");

                if (existingNegotiation.AttemptsLeft == 0)
                    return new BadRequestObjectResult("Przekroczono liczbę możliwych propozycji dla tego produktu.");

                existingNegotiation.AttemptsLeft -= wasRejected;

                if (existingNegotiation.AttemptsLeft == 0)
                    status = NegotiationModel.NegotiationStatus.Rejected;

                existingNegotiation.Status = status;
                existingNegotiation.ProposedPrice = proposedPrice;

                await _negotiationRepository.UpdateNegotiationAsync(existingNegotiation);

                return new OkObjectResult(existingNegotiation);
            }
        }

        private async Task<(int, NegotiationModel.NegotiationStatus)> ProcessNegotiationAsync(int productId, decimal proposedPrice)
        {
            int wasRejected = 0;
            var status = NegotiationModel.NegotiationStatus.Pending;
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product != null && proposedPrice > product.BasePrice * 2)
            {
                wasRejected = 1;
                status = NegotiationModel.NegotiationStatus.Rejected;
            }
            return (wasRejected, status);
        }
        public async Task<ActionResult<NegotiationModel>> EmployeeUpdateNegotiation(int id, NegotiationModel.NegotiationStatus status)
        {
            var negotiation = await _negotiationRepository.GetNegotiationByIdAsync(id);

            if (negotiation == null)
            {
                return new NotFoundObjectResult("Negotiation not found");
            }

            negotiation.Status = status;
            await _negotiationRepository.UpdateNegotiationAsync(negotiation);

            return new OkObjectResult(negotiation);
        }

        public async Task<IEnumerable<NegotiationModel>> GetAllNegotiations()
        {
            return await _negotiationRepository.GetAllNegotiationsAsync();
        }

        public async Task<NegotiationModel> GetNegotiation(int id)
        {
            return await _negotiationRepository.GetNegotiationByIdAsync(id);
        }
    }
}

