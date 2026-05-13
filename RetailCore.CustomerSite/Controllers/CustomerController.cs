using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailCore.CustomerSite.Services.Interfaces;

namespace RetailCore.CustomerSite.Controllers;

public class CustomerController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly ICustomerApiService _customerApiService;

    public CustomerController(IAuthApiService authApiService, ICustomerApiService customerApiService)
    {
        _authApiService = authApiService;
        _customerApiService = customerApiService;
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var result = await _authApiService.GetCurrentDetailsAsync();
        
        if (!result.IsSuccess) return RedirectToAction("Login", "Auth");

        return View(result.Value);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(UpdateCustomerRequest request,Guid id)
    {
        if (!ModelState.IsValid) return View("Profile", request);

        var result = await _customerApiService.UpdateCustomerAsync(id, request);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        ModelState.AddModelError("", "Failed to update profile.");
        return View("Profile");
    }
}