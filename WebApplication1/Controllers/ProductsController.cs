using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Data;
using NegotiationsApi.Models;

namespace NegotiationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductModel()
        {
          if (_context.ProductModel == null)
          {
              return NotFound();
          }
            return await _context.ProductModel.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductModel(int id)
        {
          if (_context.ProductModel == null)
          {
              return NotFound();
          }
            var productModel = await _context.ProductModel.FindAsync(id);

            if (productModel == null)
            {
                return NotFound();
            }

            return productModel;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProductModel(ProductModel productModel)
        {
          if (_context.ProductModel == null)
          {
              return Problem("Entity set 'AppDbContext.ProductModel'  is null.");
          }
            _context.ProductModel.Add(productModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductModel", new { id = productModel.Id }, productModel);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductModel(int id)
        {
            if (_context.ProductModel == null)
            {
                return NotFound();
            }
            var productModel = await _context.ProductModel.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            _context.ProductModel.Remove(productModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductModelExists(int id)
        {
            return (_context.ProductModel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
