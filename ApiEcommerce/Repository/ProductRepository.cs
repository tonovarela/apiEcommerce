
using ApiEcommerce.Models.Entities;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;


namespace ApiEcommerce.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

     public ProductRepository(ApplicationDbContext context)
    {
        _db = context; 
    }
    public bool BuyProduct(string name, int quantity)
    {
        var product = _db.Products
                        .FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        if (product == null)
        {
            throw new Exception("Product not found");
        }
        if (product.Stock < quantity)
        {
            throw new Exception("Insufficient stock");
        }
        product.Stock -= quantity;
        _db.Products.Update(product);
        return Save();
    }

    public bool CreateProduct(Product product)
    {
        this._db.Products.Add(product);
        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        this._db.Products.Remove(product);
        return Save();
    }

    public Product? GetProduct(int productId)
    {
        return _db.Products
                   .Include(p => p.Category)
                   .FirstOrDefault(p => p.ProductId == productId);
    }

    public ICollection<Product> GetProducts()
    {
        return _db.Products.Include(p => p.Category).ToList();
    }

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        return _db.Products
        .Where(p => p.CategoryId == categoryId)        
        .ToList();
    }

    public ICollection<Product> GetProductsInPages(int pageNumber, int pageSize)
    {
        return _db.Products
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .OrderBy(p => p.ProductId)
                  .ToList();
    }

    public int GetTotalProducts() =>_db.Products.Count();
    

    public bool ProductExist(int productId)
    {
        return _db.Products.Any(p => p.ProductId == productId);
    }

    public bool ProductExist(string name)
    {
        return _db.Products
                   .Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0 ;
    }

    public ICollection<Product> SearchProduct(string name)
    {
        IQueryable<Product> query = _db.Products;
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(p => p.Name.Contains(name));
        }
        return query.ToList();
    }

    public ICollection<Product> SearchProductByDescription(string description)
    {
        IQueryable<Product> query = _db.Products;
        if (!string.IsNullOrEmpty(description))
        {
            query = query.Where(p => p.Description.Contains(description));
        }
        return query.ToList();
    }

    public bool UpdateProduct(Product product)
    {
        this._db.Products.Update(product);
        return Save();
    }
}
