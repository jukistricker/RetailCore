namespace RetailCore.API.Controllers;

[Route("api/[controller]")]
public class CategoryController: ApiControllerBase
{
    private readonly ICategoryService _categoryService;
    
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    
    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        Result<Guid> result=  await _categoryService.CreateAsync(request);
        return HandleResult(result);
    }
    
    [HttpPut]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request)
    {
        Result<bool> result = await _categoryService.UpdateAsync(request);
        return HandleResult(result);
    }
}