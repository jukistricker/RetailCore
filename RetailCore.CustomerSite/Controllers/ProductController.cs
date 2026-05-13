using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Services.Interfaces;

namespace RetailCore.CustomerSite.Controllers;

public class ProductController : Controller
{
    private readonly IProductApiService _productApiService;

    public ProductController(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    // Trang danh sách: /product
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagingRequest request, [FromQuery] Guid? categoryId,
        [FromQuery] string? slug, [FromQuery] bool isPartial = false)
    {
        var result = await _productApiService.GetByPageAsync(request, categoryId, slug);

        if (!result.IsSuccess)
        {
            var emptyResponse = new PagingResponse<ProductSummaryResponse>();
            return isPartial ? PartialView("_ProductListOnlyPartial", emptyResponse.Items) : View(emptyResponse);
        }

        if (isPartial) return PartialView("_ProductListOnlyPartial", result.Value.Items);

        return View(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Detail(Guid id)
    {
        var result = await _productApiService.GetByIdAsync(id);

        if (!result.IsSuccess || result.Value == null) return NotFound();

        return View(result.Value);
    }

    // API nội bộ cho trang chủ gọi lấy sản phẩm nổi bật
    [HttpGet("featured")]
    public async Task<IActionResult> Featured(int count = 4)
    {
        var result = await _productApiService.GetFeaturedAsync(count);
        return PartialView("_ProductListPartial", result.Value);
    }
}