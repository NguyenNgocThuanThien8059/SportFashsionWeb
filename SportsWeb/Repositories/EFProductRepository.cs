using Microsoft.EntityFrameworkCore;
using SportsWeb.Models;

namespace SportsWeb.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _context;
        public EFProductRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product, int[] selectedCategoryIds)
        {
            var existingProduct = await _context.Products.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category).FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.Images = product.Images;
                var currentCategoryIds = existingProduct.ProductCategories.Select(pc => pc.CategoryId).ToList();

                // Remove categories that are no longer selected
                foreach (var categoryId in currentCategoryIds)
                {
                    if (!selectedCategoryIds.Contains(categoryId))
                    {
                        var categoryToRemove = existingProduct.ProductCategories.FirstOrDefault(pc => pc.CategoryId == categoryId);
                        if (categoryToRemove != null)
                        {
                            _context.ProductCategories.Remove(categoryToRemove);
                        }
                    }
                }

                // Add new categories that were selected
                foreach (var categoryId in selectedCategoryIds)
                {
                    if (!currentCategoryIds.Contains(categoryId))
                    {
                        existingProduct.ProductCategories.Add(new ProductCategory
                        {
                            ProductId = existingProduct.Id,
                            CategoryId = categoryId
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.Include(p => p.ProductCategories).FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                _context.ProductCategories.RemoveRange(product.ProductCategories);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
