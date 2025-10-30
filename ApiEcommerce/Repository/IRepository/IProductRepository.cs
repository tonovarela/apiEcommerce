

using ApiEcommerce.Models.Entities;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();

    ICollection<Product> GetProductsForCategory(int categoryId);

    ICollection<Product> SearchProduct(string name);
    ICollection<Product> SearchProductByDescription(string description);

    Product? GetProduct(int productId);

    bool BuyProduct(string name,int quantity);

    bool ProductExist(int productId);

    bool ProductExist(string name);

    bool CreateProduct(Product product);

    bool UpdateProduct(Product product);


    bool DeleteProduct(Product product);

    bool Save();



}
