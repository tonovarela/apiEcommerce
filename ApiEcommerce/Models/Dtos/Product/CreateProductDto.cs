using System;

namespace ApiEcommerce.Models.Dtos;

public class CreateProductDto
{    
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public string ImgUrl { get; set; } = String.Empty;    
    public string SKU { get; set; } = String.Empty;    
    public int Stock { get; set; }
    
    public int CategoryId { get; set; }
}
