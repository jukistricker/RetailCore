namespace RetailCore.API.Controllers;

[Route("api/products")]
public class ProductController : ApiControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productService.GetProductDetailAsync(id);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByPage([FromQuery] PagingRequest request, [FromQuery] Guid? categoryId,
        [FromQuery] string? slug)
    {
        var result = await _productService.GetProductsAsync(request, categoryId, slug);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] ProductCreateRequest request)
    {
        var result = await _productService.CreateProductAsync(request);
        return HandleResult(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(Guid id, [FromForm] ProductUpdateRequest request)
    {
        if (id != request.Id) return BadRequest("ID mismatch");

        var result = await _productService.UpdateProductAsync(id, request);
        return HandleResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteProductAsync(id);
        return HandleResult(result);
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeatured([FromQuery] int count = 4)
    {
        var result = await _productService.GetFeaturedProductsAsync(count);
        return HandleResult(result);
    }
}