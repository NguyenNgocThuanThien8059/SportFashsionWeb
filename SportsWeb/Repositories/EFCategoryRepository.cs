using Microsoft.EntityFrameworkCore;
using SportsWeb.Models;

namespace SportsWeb.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDBContext _context;
        public EFCategoryRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.Include(c => c.ProductCategories).ThenInclude(pc => pc.Product).ToListAsync();
        }
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.Include(c => c.ProductCategories).ThenInclude(pc => pc.Product).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            var existingCategory = await _context.Categories.Include(c => c.ProductCategories).FirstOrDefaultAsync(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.ProductCategories.Clear();
                foreach (var productId in category.ProductCategories.Select(pc => pc.ProductId))
                {
                    existingCategory.ProductCategories.Add(new ProductCategory { CategoryId = existingCategory.Id, ProductId = productId });
                }
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.Include(c => c.ProductCategories).FirstOrDefaultAsync(c => c.Id == id);

            if (category != null)
            {
                _context.ProductCategories.RemoveRange(category.ProductCategories);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
