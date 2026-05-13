namespace RetailCore.API.Controllers;

[Route("api/categories")]
public class CategoryController : ApiControllerBase
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
        var result = await _categoryService.CreateAsync(request);
        return HandleResult(result);
    }

    [HttpPut]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request)
    {
        var result = await _categoryService.UpdateAsync(request);
        return HandleResult(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery]PagingRequest request)
    {
        var result = await _categoryService.GetByPageAsync(request);
        return HandleResult(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        return HandleResult(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        return HandleResult(result);
    }
}