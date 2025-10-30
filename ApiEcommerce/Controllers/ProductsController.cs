
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;
using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{

    private readonly IMapper _mapper;
    private readonly IProductRepository _productRepository;

    private readonly ICategoryRepository _categoryRepository;

    public ProductsController(IMapper mapper, IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _mapper = mapper;
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }




    [HttpGet("{productId:int}", Name = "GetProduct")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProduct(int productId)
    {
        try
        {
            var product = _productRepository.GetProduct(productId);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDto>(product);
            return Ok(productDto);

        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [HttpGet(Name = "GetProducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProducts()
    {
        var products = _productRepository.GetProducts();
        var productsDto = new List<ProductDto>();
        foreach (Product product in products)
        {
            var ProductDto = _mapper.Map<ProductDto>(product);
            Console.WriteLine(ProductDto.CategoryName);
            productsDto.Add(ProductDto);
        }

        return Ok(productsDto);
    }


    [HttpPost(Name = "CreateProduct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
        {
            return NotFound(new { Message = "Category not found." });
        }
        var product = _mapper.Map<Product>(createProductDto);
        bool registro = _productRepository.CreateProduct(product);
        if (!registro)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the product.");
        }

        var createdProduct  =_productRepository.GetProduct(product.ProductId);
        var createdProductDto = _mapper.Map<ProductDto>(createdProduct);        
        return CreatedAtRoute("GetProduct", new { productId = createdProductDto.ProductId }, createdProductDto);
    }


    [HttpDelete("{productId:int}", Name = "DeleteProduct")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteProduct(int productId)
    {
        var product = _productRepository.GetProduct(productId);
        if (product == null)
        {
            return NotFound();
        }
        _productRepository.DeleteProduct(product);
        return NoContent();
    }


    [HttpGet("category/{categoryId:int}", Name = "GetProductsForCategory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsForCategory(int categoryId)
    {
        var products = _productRepository.GetProductsForCategory(categoryId);
        var productsDto = new List<ProductDto>();
        foreach (var product in products)
        {
            productsDto.Add(_mapper.Map<ProductDto>(product));
        }
        return Ok(productsDto);
    }



    [HttpGet("searchByDescription/{productDescription}", Name = "GetProductsByDescription")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsByDescription(string productDescription)
    {
        var products = _productRepository.SearchProductByDescription(productDescription);
        var productsDto = new List<ProductDto>();
        foreach (var product in products)
        {
            productsDto.Add(_mapper.Map<ProductDto>(product));
        }
        return Ok(productsDto);
    }



    [HttpGet("searchByname/{productName}", Name = "GetProductsByName")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetProductsByName(string productName)
    {
        var products = _productRepository.SearchProduct(productName);
        var productsDto = new List<ProductDto>();
        foreach (var product in products)
        {
            productsDto.Add(_mapper.Map<ProductDto>(product));
        }
        return Ok(productsDto);
    }



    [HttpPut("{productId:int}", Name = "UpdateProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateProduct(int productId, [FromBody] UpdateProductDto productDto)
    {
        var product = _productRepository.GetProduct(productId);

        if (product == null)
        {
            return NotFound();
        }
        if (!_categoryRepository.CategoryExists(productDto.CategoryId))
        {
            return NotFound(new { Message = "Category not found." });
        }
        product.UpdateDate = DateTime.Now;
        _mapper.Map(productDto, product);
        bool actualizo = _productRepository.UpdateProduct(product);
        if (!actualizo)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the product.");
        }
        return Ok(_mapper.Map<ProductDto>(product));
    }



     [HttpGet("buyProduct/{name}/quantity/{quantity:int}", Name = "BuyProduct")]
     [ProducesResponseType(StatusCodes.Status200OK)]
     [ProducesResponseType(StatusCodes.Status404NotFound)]
     [ProducesResponseType(StatusCodes.Status400BadRequest)]
     public IActionResult BuyProduct(string name, int quantity)
     {                         
         try
         {
             var result = _productRepository.BuyProduct(name, quantity);
             if (!result)
             {
                 return NotFound(new { Message = "Product not found or insufficient stock." });
             }
             return Ok(new { Message = "Product purchased successfully." });
         }
         catch (Exception ex)
         {
             return StatusCode(StatusCodes.Status400BadRequest, new { Message = ex.Message });
         }
    }


}
