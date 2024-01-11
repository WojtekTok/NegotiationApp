using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Data;
using NegotiationsApi.Models;
using NegotiationsApi.Repositories;

namespace NegotiationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetAllProducts()
        {
             var products = await _productRepository.GetAllProductsAsync();
             if (products == null)
             {
                  return NotFound();
             }
             return Ok(products);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProduct(ProductModel productModel)
        {
            await _productRepository.AddProductAsync(productModel);

            return CreatedAtAction("GetProduct", new { id = productModel.Id }, productModel);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _productRepository.DeleteProductAsync(id);

            return NoContent();
        }
    }
}
