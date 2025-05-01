using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMart.API.Entities;
using PixelMart.API.Models;
using PixelMart.API.Models.Identity;
using PixelMart.API.Repositories;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly IMapper _mapper;

    public CategoriesController(IPixelMartRepository pixelMartRepository, IMapper mapper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet(Name = "GetAllCategories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
    {
        var categories = await _pixelMartRepository.GetCategoriesAsync();
        return Ok(_mapper.Map<IEnumerable<CategoryDto>>(categories));
    }

    [HttpGet("{categoryId}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid categoryId)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var categoryFromRepo = await _pixelMartRepository.GetCategoryAsync(categoryId);

        if (categoryFromRepo == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CategoryDto>(categoryFromRepo));
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPost(Name = "CreateNewCategory")]
    public async Task<IActionResult> CreateCategory(CategoryForCreationDto category)
    {
        var categoryDto = _mapper.Map<Category>(category);

        _pixelMartRepository.AddCategory(categoryDto);

        await _pixelMartRepository.SaveAsync();

        var categoryToReturn = _mapper.Map<CategoryDto>(categoryDto);
        return CreatedAtRoute("GetCategory", new { categoryId = categoryToReturn.Id }, categoryToReturn);

    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryForUpdateDto category)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var categoryFromRepo = await _pixelMartRepository.GetCategoryAsync(categoryId);

        _mapper.Map<Category>(categoryFromRepo); // apply the updated field values to the entity

        _pixelMartRepository.UpdateCategory(categoryFromRepo);

        await _pixelMartRepository.SaveAsync();

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        if (!await _pixelMartRepository.CategoryExistsAsync(categoryId))
        {
            return NotFound();
        }

        var categoryFromRepo = await _pixelMartRepository.GetCategoryAsync(categoryId);

        if (categoryFromRepo == null)
        {
            return NotFound();
        }

        _pixelMartRepository.DeleteCategory(categoryFromRepo);
        await _pixelMartRepository.SaveAsync();

        return NoContent();
    }
}
