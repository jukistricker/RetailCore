using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Services.Interfaces;

namespace RetailCore.CustomerSite.ViewComponents;


public class CategoryMenuViewComponent : ViewComponent
{
    private readonly ICategoryApiService _categoryApiService;

    public CategoryMenuViewComponent(ICategoryApiService categoryApiService)
    {
        _categoryApiService = categoryApiService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var request = new PagingRequest { PageSize = 10, PageNumber = 1 };
        
        var result = await _categoryApiService.GetByPageAsync(request);

        var categories = result.IsSuccess ? result.Value?.Items : new List<CategoryResponse>();
        
        return View(categories);
    }
}