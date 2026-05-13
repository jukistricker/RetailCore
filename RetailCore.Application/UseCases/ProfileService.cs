using System.Security.Claims;
using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;

namespace RetailCore.Application.UseCases;

public class ProfileService : IProfileService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly UserManager<IdentityUser<Guid>> _userManager;

    public ProfileService(UserManager<IdentityUser<Guid>> userManager, ICustomerRepository customerRepository)
    {
        _userManager = userManager;
        _customerRepository = customerRepository;
    }

    // Triển khai logic thực tế vào hàm này
    public async Task GetProfileDataAsync(ProfileDataRequestContext context, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        if (user == null) return;

        var claims = new List<Claim>();

        // Truyền CancellationToken xuống Repository để tối ưu
        var customer = await _customerRepository.GetByUserIdAsync(user.Id);

        if (customer != null) claims.Add(new Claim("customer_id", customer.Id.ToString()));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles) claims.Add(new Claim(JwtClaimTypes.Role, role));

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context, CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = user != null;
    }

    // Đây là hàm chính mà IdentityServer sẽ gọi
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        // Chuyển logic vào đây, hoặc gọi hàm có CancellationToken với CancellationToken.None
        await GetProfileDataAsync(context, CancellationToken.None);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        await IsActiveAsync(context, CancellationToken.None);
    }
}