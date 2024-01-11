using NegotiationsApi.Models;

namespace NegotiationsApi.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductModel>> GetAllProductsAsync();
        Task<ProductModel> GetProductByIdAsync(int id);
        Task AddProductAsync(ProductModel product);
        Task DeleteProductAsync(int id);
    }
}
