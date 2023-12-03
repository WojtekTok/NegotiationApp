using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Services;
using NegotiationsApi.Data;
using NegotiationsApi.Models;

namespace NegotiationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NegotiationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly INegotiationService _negotiationService;

        public NegotiationsController(AppDbContext context, INegotiationService negotiationService)
        {
            _context = context;
            _negotiationService = negotiationService;
        }

        // GET: api/Negotiations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NegotiationModel>>> GetAllNegotiations()
        {
          if (_context.NegotiationModel == null)
          {
              return NotFound();
          }
            return await _context.NegotiationModel.ToListAsync();
        }

        // GET: api/Negotiations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NegotiationModel>> GetNegotiation(int id)
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

        // DELETE: api/Negotiations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNegotiation(int id)
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

        // POST: api/Negotiations (Customer method)
        [HttpPost("{productId}/{customerId}/{proposedPrice}")]
        public async Task<ActionResult<NegotiationModel>> CustomerPostNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            return await _negotiationService.AddNegotiation(productId, customerId, proposedPrice);
        }

        // PUT: api/Negotiations (Customer method)
        [HttpPut("{productId}/{customerId}/{proposedPrice}")]
        public async Task<ActionResult<NegotiationModel>> CustomerPutNegotiation(int productId, int customerId, decimal proposedPrice)
        {
            return await _negotiationService.UpdateNegotiation(productId, customerId, proposedPrice);
        }

        // PUT: api/Negotiations (Employee method)
        [HttpPut("{id}")]
        public async Task<ActionResult<NegotiationModel>> EmployeePutNegotiation(int id, NegotiationModel.NegotiationStatus status)
        {
            if (_context.NegotiationModel == null)
                return NotFound();
            var negotiation = await _context.NegotiationModel.FindAsync(id);
            if (negotiation == null)
                return NotFound();
            negotiation.Status = status; // accept or decline
            await _context.SaveChangesAsync();

            return new OkObjectResult(negotiation);
        }
    }
}
