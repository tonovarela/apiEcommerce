using ApiEcommerce.Constants;
using ApiEcommerce.Models.Dtos;
using ApiEcommerce.Models.Entities;
using ApiEcommerce.Models.Responses;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace ApiEcommerce.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]

[ApiController]
[ApiVersionNeutral]   

[EnableCors(PolicyNames.AllowSpecificOrigins)]
[Authorize(Roles = "User")]    
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
    [AllowAnonymous]
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



    [HttpGet("Paged", Name = "GetProductInPage")]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public IActionResult GetProductInPage([FromQuery] int pageNumber = 1,
                                          [FromQuery] int pageSize = 10)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Max(1, pageSize);
        try
        {
            var totalProducts = _productRepository.GetTotalProducts();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            if (pageNumber > totalPages && totalPages != 0)
            {
                return BadRequest(new { Message = "Page number exceeds total pages available." });
            }
            var products = _productRepository.GetProductsInPages(pageNumber, pageSize);
            var productsDto = new List<ProductDto>();
            foreach (var product in products)
            {
                productsDto.Add(_mapper.Map<ProductDto>(product));
            }
            return Ok(new PaginationResponse<ProductDto>
            {                
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Items = productsDto
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }


    [HttpGet(Name = "GetProducts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [AllowAnonymous]
    public IActionResult GetProducts()
    {
        var products = _productRepository.GetProducts();
        var productsDto = new List<ProductDto>();
        foreach (Product product in products)
        {
            var ProductDto = _mapper.Map<ProductDto>(product);
            productsDto.Add(ProductDto);
        }
        return Ok(productsDto);
    }
    

  

    



    [HttpPost(Name = "CreateProduct")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult CreateProduct([FromForm] CreateProductDto createProductDto)
    {
        if (!_categoryRepository.CategoryExists(createProductDto.CategoryId))
        {
            return NotFound(new { Message = "Category not found." });
        }
        if (_productRepository.ProductExist(createProductDto.Name))
        {
            ModelState.AddModelError("CustomError", "The product already exists.");
            return BadRequest(ModelState);
        }
        var product = _mapper.Map<Product>(createProductDto);
        UploadProductImage(createProductDto, product);    
        bool registro = _productRepository.CreateProduct(product);
        if (!registro)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the product.");
        }
        var createdProduct  =_productRepository.GetProduct(product.ProductId);
        if (createdProduct == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving created product.");
        }
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    [AllowAnonymous]
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
    public IActionResult UpdateProduct(int productId, [FromForm] UpdateProductDto updateProductDto)
    {
        var product = _productRepository.GetProduct(productId);
        if (product == null)
        {
            return NotFound();
        }
        if (!_categoryRepository.CategoryExists(updateProductDto.CategoryId))
        {
            return NotFound(new { Message = "Category not found." });
        }
        UploadProductImage(updateProductDto, product);
        product.UpdateDate = DateTime.Now;
        _mapper.Map(updateProductDto, product);
        bool actualizo = _productRepository.UpdateProduct(product);
        if (!actualizo)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the product.");
        }
        return Ok(_mapper.Map<ProductDto>(product));
    }

    private void UploadProductImage(dynamic productDto, Product product)
    {
        product.ImgUrl = productDto.Image != null
                                                ? $"{product.ProductId}-{Guid.NewGuid()}{Path.GetExtension(productDto.Image.FileName)}"
                                                : "https://placehold.co/300x300";

        if (productDto.Image != null)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages", product.ImgUrl);
            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProductImages");
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                productDto.Image.CopyTo(stream);
                var baseUrl = $"{Request.Scheme}://{Request.Host.Value}//{Request.PathBase.Value}";
                product.ImgUrl = $"{baseUrl}/ProductImages/{product.ImgUrl}";
                product.ImgUrlLocal = imagePath;
            }
        }
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
