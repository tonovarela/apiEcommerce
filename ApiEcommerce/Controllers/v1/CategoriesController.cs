using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using ApiEcommerce.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using ApiEcommerce.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using ApiEcommerce.Constants;
using Asp.Versioning;

namespace ApiEcommerce.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]    
    [Authorize(Roles = "Admin")]    
    public class CategoriesController : ControllerBase
    {
         private readonly ICategoryRepository _categoryRepository;
         private readonly  IMapper _mapper;
        public CategoriesController(ICategoryRepository categoryRepository,IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetCategories")]

        //[ResponseCache(CacheProfileName = CacheProfiles.Default10)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Obsolete("Use GetCategoriesOrderById in v2")]
        
        [AllowAnonymous]
        public IActionResult GetCategories()
        {

            Console.WriteLine($"Version 1.0");
            var categories = _categoryRepository.GetCategories();
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            Console.WriteLine("Retrieved categories: " + categoriesDto.Count);
            return Ok(categoriesDto);
        }
        


       



        [HttpGet("{categoryId:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        
        [AllowAnonymous]
        public IActionResult GetCategory(int categoryId)
        {
            try
            {
                var category = _categoryRepository.GetCategory(categoryId);
                if (category == null)
                {
                    return NotFound();
                }
                var categoryDto = _mapper.Map<CategoryDto>(category);
                return Ok(categoryDto);

            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

       
       
       
       
        [HttpPost(Name = "CreateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto == null) return BadRequest();

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);                
            }

            var category = _mapper.Map<Category>(createCategoryDto);

            bool response = _categoryRepository.CreateCategory(category);
           if (!response)
            {
                ModelState.AddModelError("CustomError", "Error saving category!");
                return StatusCode(500, ModelState);
            }

            var createdCategoryDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtRoute("GetCategory", new { categoryId = createdCategoryDto.Id }, createdCategoryDto);
        }


        [HttpPut("{categoryId:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {            
            if (categoryDto == null || categoryId != categoryDto.Id)
            {
                return BadRequest();
            }
            if (_categoryRepository.CategoryExists(categoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }
            var category = _mapper.Map<Category>(categoryDto);
            bool registro = _categoryRepository.UpdateCategory(category);             
             if (!registro)
            {
                ModelState.AddModelError("CustomError", "Error updating category!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


        [HttpDelete("{categoryId:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteCategory(int categoryId)
        {
            var category = _categoryRepository.GetCategory(categoryId);
            if (category == null)
            {
                return NotFound();
            }
            bool seElimino = _categoryRepository.DeleteCategory(category);
            if (!seElimino)
            {
                ModelState.AddModelError("CustomError", "Error deleting category!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
        
        

    }
}
