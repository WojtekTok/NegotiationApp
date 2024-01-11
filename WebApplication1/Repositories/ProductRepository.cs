using Microsoft.EntityFrameworkCore;
using NegotiationsApi.Data;
using NegotiationsApi.Models;

namespace NegotiationsApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductModel>> GetAllProductsAsync()
        {
            return await _context.ProductModel.ToListAsync();
        }

        public async Task<ProductModel> GetProductByIdAsync(int id)
        {
            return await _context.ProductModel.FindAsync(id);
        }

        public async Task AddProductAsync(ProductModel product)
        {
            _context.ProductModel.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.ProductModel.FindAsync(id);
            if (product != null)
            {
                _context.ProductModel.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
