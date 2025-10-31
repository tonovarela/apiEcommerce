using System;

namespace ApiEcommerce.Models.Dtos;

public class ProductDto
{
      
    public int ProductId { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public string ImgUrl { get; set; } = String.Empty;    
    public string SKU { get; set; } = String.Empty;    
    public int Stock { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
   public DateTime? UpdateDate { get; set; } = null;

    
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = String.Empty;
    
    
}
