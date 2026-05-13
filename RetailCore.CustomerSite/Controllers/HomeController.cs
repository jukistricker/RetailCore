using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Models;
using RetailCore.CustomerSite.Services.Interfaces;

namespace RetailCore.CustomerSite.Controllers;

public class HomeController : Controller
{
    private readonly IProductApiService _productApiService;

    public HomeController(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _productApiService.GetFeaturedAsync(4);

        var products = result.IsSuccess ? result.Value : new List<ProductSummaryResponse>();

        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}