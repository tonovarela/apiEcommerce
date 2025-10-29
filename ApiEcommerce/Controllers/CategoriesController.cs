using ApiEcommerce.Repository.IRepository;
using AutoMapper;
using ApiEcommerce.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using ApiEcommerce.Models.Entities;

namespace ApiEcommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
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
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest();
            }

            var category = _mapper.Map<Category>(categoryDto);
            _categoryRepository.CreateCategory(category);

            var createdCategoryDto = _mapper.Map<CategoryDto>(category);
            return CreatedAtRoute("GetCategory", new { categoryId = createdCategoryDto.Id }, createdCategoryDto);
        }


        [HttpPut("{categoryId:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || categoryId != categoryDto.Id)
            {
                return BadRequest();
            }

            var category = _mapper.Map<Category>(categoryDto);
            _categoryRepository.UpdateCategory(category);
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
            _categoryRepository.DeleteCategory(category);
            return NoContent();
        }
        
        

    }
}
