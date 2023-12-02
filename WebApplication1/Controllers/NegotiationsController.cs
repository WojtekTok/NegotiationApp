using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NegotiationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Negotiations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NegotiationModel>>> GetNegotiationModel()
        {
          if (_context.NegotiationModel == null)
          {
              return NotFound();
          }
            return await _context.NegotiationModel.ToListAsync();
        }

        // GET: api/Negotiations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NegotiationModel>> GetNegotiationModel(int id)
        {
          if (_context.NegotiationModel == null)
          {
              return NotFound();
          }
            var negotiationModel = await _context.NegotiationModel.FindAsync(id);

            if (negotiationModel == null)
            {
                return NotFound();
            }

            return negotiationModel;
        }

        // PUT: api/Negotiations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNegotiationModel(int id, NegotiationModel negotiationModel)
        {
            if (id != negotiationModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(negotiationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NegotiationModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Negotiations
        [HttpPost]
        public async Task<ActionResult<NegotiationModel>> PostNegotiationModel(NegotiationModel negotiationModel)
        {
          if (_context.NegotiationModel == null)
          {
              return Problem("Entity set 'AppDbContext.NegotiationModel'  is null.");
          }
            _context.NegotiationModel.Add(negotiationModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNegotiationModel", new { id = negotiationModel.Id }, negotiationModel);
        }

        // DELETE: api/Negotiations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNegotiationModel(int id)
        {
            if (_context.NegotiationModel == null)
            {
                return NotFound();
            }
            var negotiationModel = await _context.NegotiationModel.FindAsync(id);
            if (negotiationModel == null)
            {
                return NotFound();
            }

            _context.NegotiationModel.Remove(negotiationModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{productId}/{customerId}/{proposedPrice}")]
        public async Task<ActionResult<NegotiationModel>> PostNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            // Can't add a negotiation that already exist
            if (_context.NegotiationModel.Any(n => n.ProductId == productId && n.CustomerId == customerId))
                return BadRequest("Negocjacja o ten produkt już istnieje");
            // Check if price is valid
            if (proposedPrice <= 0)
                return BadRequest("Proszę podać cenę większą od zera.");          
            else
            {
                int wasRejected = 0;
                NegotiationModel.NegotiationStatus status = NegotiationModel.NegotiationStatus.Pending;
                var basePrice = _context.ProductModel.Find(productId)?.BasePrice;
                // check if price is twice bigger than base price, if so automatically reject proposition
                if (basePrice.HasValue && proposedPrice > basePrice * 2) //Here I think it was meant to be half of the price
                {
                    wasRejected = 1;
                    status = NegotiationModel.NegotiationStatus.Rejected;
                }

                NegotiationModel newNegotiation = new NegotiationModel()
                {
                    ProductId = productId,
                    CustomerId = customerId,
                    ProposedPrice = proposedPrice,
                    AttemptsLeft = 3-wasRejected,
                    Status = status
                };
                _context.NegotiationModel.Add(newNegotiation);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetNegotiationModel", new { id = newNegotiation.Id }, newNegotiation);
            }   
        }

        [HttpPut("{productId}/{customerId}/{proposedPrice}")]
        public async Task<ActionResult<NegotiationModel>> PutNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            // Can't add a negotiation that already exist
            if (!_context.NegotiationModel.Any(n => n.ProductId == productId && n.CustomerId == customerId))
                return BadRequest("Negocjacja o ten produkt nie istnieje");
            // Check if price is valid
            else if (proposedPrice <= 0)
                return BadRequest("Proszę podać cenę większą od zera.");
            else
            {
                int wasRejected = 0;
                NegotiationModel.NegotiationStatus status = NegotiationModel.NegotiationStatus.Pending;
                var basePrice = _context.ProductModel.Find(productId)?.BasePrice;
                // check if price is twice bigger than base price, if so automatically reject proposition
                if (basePrice.HasValue && proposedPrice > basePrice * 2) //Here I think it was meant to be half of the price
                {
                    wasRejected = 1;
                    status = NegotiationModel.NegotiationStatus.Rejected;
                }

                NegotiationModel? existingNegotiation = _context.NegotiationModel
                    .FirstOrDefault(n => n.ProductId == productId && n.CustomerId == customerId);

                if (existingNegotiation.Status == NegotiationModel.NegotiationStatus.Pending)
                    return BadRequest("Ostatnia oferta oczekuje nadal oczekuje na decyzję.");
                else if (existingNegotiation.AttemptsLeft == 0)
                    return BadRequest("Przekroczono liczbę możliwych propozycji dla tego produktu.");
                else
                {
                    existingNegotiation.AttemptsLeft -= wasRejected;
                    if (existingNegotiation.AttemptsLeft == 0)
                        status = NegotiationModel.NegotiationStatus.Rejected;

                    existingNegotiation.Status = status;
                    existingNegotiation.ProposedPrice = proposedPrice;

                    await _context.SaveChangesAsync();

                    return Ok(existingNegotiation);
                }
            }
        }

        

        private bool NegotiationModelExists(int id)
        {
            return (_context.NegotiationModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
