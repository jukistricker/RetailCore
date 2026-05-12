namespace RetailCore.API.Controllers;

[Route("api/[controller]")]
public class ProductController: ApiControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("debug-claims")]
    [Authorize]
    public IActionResult DebugClaims()
    {
        return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Result<ProductDetailResponse> result = await _productService.GetProductDetailAsync(id);
        return HandleResult(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetByPage([FromQuery] PagingRequest request, [FromQuery] Guid? categoryId, [FromQuery] string? slug)
    {
        Result<PagingResponse<ProductSummaryResponse>> result = await _productService.GetProductsAsync(request, categoryId, slug);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
    {
        Result<bool> result = await _productService.CreateProductAsync(request);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] ProductUpdateRequest request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");
        
        Result<bool> result = await _productService.UpdateProductAsync(id, request);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Result<bool> result = await _productService.DeleteProductAsync(id);
        return HandleResult(result);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 4)
    {
        Result<IEnumerable<ProductSummaryResponse>> result = await _productService.GetFeaturedProductsAsync(count);
        return HandleResult(result);
    }
}

