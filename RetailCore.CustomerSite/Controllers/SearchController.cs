using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Services.Interfaces;

namespace RetailCore.CustomerSite.Controllers;

public class SearchController : Controller
{
    private readonly IProductApiService _productApiService;

    public SearchController(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    [HttpGet("/Search")]
    public async Task<IActionResult> Index(string? q, Guid? categoryId, int pageNumber = 1)
    {
        var request = new PagingRequest 
        { 
            Search = q, 
            PageNumber = pageNumber, 
            PageSize = 12 
        };

        var result = await _productApiService.GetByPageAsync(request, categoryId, null);
        ViewBag.CurrentSearch = q;
        ViewBag.CurrentCategory = categoryId;

        return View(result.Value); 
    }
}