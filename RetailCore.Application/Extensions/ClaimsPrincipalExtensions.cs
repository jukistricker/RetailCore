using System.Security.Claims;

namespace RetailCore.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetCustomerId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst("customer_id")?.Value
                    ?? user.FindFirst("sub")?.Value;

        return Guid.TryParse(value, out var result) ? result : Guid.Empty;
    }
}