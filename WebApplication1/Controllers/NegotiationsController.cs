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
        private readonly INegotiationService _negotiationService;

        public NegotiationsController(INegotiationService negotiationService)
        {
            _negotiationService = negotiationService;
        }

        // GET: api/Negotiations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NegotiationModel>>> GetAllNegotiationsAsync()
        {
            var negotiations = await _negotiationService.GetAllNegotiations();
            if (negotiations == null)
            {
                return NotFound();
            }
            return Ok(negotiations);
        }

        // GET: api/Negotiations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NegotiationModel>> GetNegotiation(int id)
        {
            return Ok(await _negotiationService.GetNegotiation(id));
        }

        // DELETE: api/Negotiations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNegotiation(int id)
        {
            // Implementacja może być dodana w przyszłości, jeśli będzie potrzebna
            return Ok();
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
        [HttpPut("{id}/{status}")]
        public async Task<ActionResult<NegotiationModel>> EmployeePutNegotiation(int id, NegotiationModel.NegotiationStatus status)
        {
            return await _negotiationService.EmployeeUpdateNegotiation(id, status);
        }
    }
}

