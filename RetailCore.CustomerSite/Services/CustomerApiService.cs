using Microsoft.AspNetCore.WebUtilities;
using RetailCore.CustomerSite.Services.Interfaces;
using RetailCore.Shared.Constants;

namespace RetailCore.CustomerSite.Services;

public class CustomerApiService: BaseApiService,ICustomerApiService
{
    public CustomerApiService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
        : base(httpClientFactory, configuration)
    {
    }

    public async Task<Result<bool>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["id"] = id.ToString()
        };
        var url = QueryHelpers.AddQueryString(ApiRoutes.Customers.CustomerBase, queryParams);

        var response = await _httpClient.PutAsJsonAsync(url, request);

        if (response.IsSuccessStatusCode)
        {
            return Result<bool>.Success(true);
        }

        return Result<bool>.Failure(null, "Update failed", (int)response.StatusCode);
    }

}