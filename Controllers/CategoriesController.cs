using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Category;
using PixelMart.API.Models.Identity;
using PixelMart.API.Repositories;

namespace PixelMart.API.Controllers;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IPixelMartRepository _pixelMartRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IMapper _mapper;
    private readonly RequestLogHelper _requestLogHelper;

    public CategoriesController(IPixelMartRepository pixelMartRepository, ICategoriesRepository categoriesRepository, IMapper mapper, RequestLogHelper requestLogHelper)
    {
        _pixelMartRepository = pixelMartRepository ?? throw new ArgumentNullException(nameof(pixelMartRepository));
        _categoriesRepository = categoriesRepository ?? throw new ArgumentNullException(nameof(categoriesRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _requestLogHelper = requestLogHelper ?? throw new ArgumentNullException(nameof(requestLogHelper));
    }

    [HttpGet(Name = "GetAllCategories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
    {
        _requestLogHelper.LogInfo("GET /api/categories CALLED TO RETRIEVE ALL PRODUCT CATEGORIES");

        var categories = await _categoriesRepository.GetCategoriesAsync();
        var categoriesResponse = _mapper.Map<IEnumerable<CategoryDto>>(categories);

        return Ok(categoriesResponse);
    }

    [HttpGet("{categoryId}", Name = "GetCategory")]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid categoryId)
    {
        _requestLogHelper.LogInfo($"GET /api/categories/{categoryId} CALLED TO RETRIEVE SINGLE CATEGORY");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var categoryFromRepo = await _categoriesRepository.GetCategoryAsync(categoryId);

        if (categoryFromRepo == null)
            return NotFound();

        var categoryResponse = _mapper.Map<CategoryDto>(categoryFromRepo);

        return Ok(categoryResponse);
    }

    [Authorize(Roles = $"{UserRoles.Admin}, {UserRoles.User}")]
    [HttpPost(Name = "CreateNewCategory")]
    public async Task<IActionResult> CreateCategory(CategoryForCreationDto category)
    {
        _requestLogHelper.LogInfo("POST /api/categories CALLED TO CREATE A NEW CATEGORY");

        var categoryDto = _mapper.Map<Category>(category);
        await _categoriesRepository.AddCategoryAsync(categoryDto);
        await _pixelMartRepository.SaveAsync();

        var categoryToReturn = _mapper.Map<CategoryDto>(categoryDto);

        return CreatedAtRoute("GetCategory", new { categoryId = categoryToReturn.Id }, categoryToReturn);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryForUpdateDto category)
    {
        _requestLogHelper.LogInfo($"PUT /api/categories/{categoryId} CALLED TO UPDATE A CATEGORY");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var categoryFromRepo = await _categoriesRepository.GetCategoryAsync(categoryId);
        _mapper.Map(category, categoryFromRepo); // apply new values 

        await _categoriesRepository.UpdateCategoryAsync(categoryFromRepo!);

        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        _requestLogHelper.LogInfo($"DELETE /api/categories/{categoryId} CALLED TO DELETE A CATEGORY");

        if (!await _categoriesRepository.CategoryExistsAsync(categoryId))
            return NotFound();

        var categoryFromRepo = await _categoriesRepository.GetCategoryAsync(categoryId);

        if (categoryFromRepo == null)
            return NotFound();

        await _categoriesRepository.DeleteCategoryAsync(categoryFromRepo);

        return NoContent();
    }
}
