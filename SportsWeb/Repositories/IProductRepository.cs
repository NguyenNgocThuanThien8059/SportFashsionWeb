using SportsWeb.Models;

namespace SportsWeb.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product, int[] selectedCategoryIds);
        Task DeleteAsync(int id);
    }
}
