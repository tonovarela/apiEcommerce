using ApiEcommerce.Models.Entities;
using ApiEcommerce.Repository.IRepository;

namespace ApiEcommerce.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db)
    {
        this._db = db;
    }

    public bool CategoryExists(string name)
    {
        return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateCategory(Category category)
    {
        return _db.Categories.Add(category) != null;
    }

    public bool DeleteCategory(Category category)
    {
        return _db.Categories.Remove(category) != null;
    }

    public ICollection<Category> GetCategories()
    {
        return _db.Categories.ToList();
    }

    public Category GetCategory(int categoryId)
    {
        return _db.Categories.FirstOrDefault(c => c.Id == categoryId)!;
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public bool UpdateCategory(Category category)
    {
        _db.Categories.Update(category);
        return true;
    }
}

