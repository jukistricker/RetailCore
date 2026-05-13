namespace RetailCore.CustomerSite.Services;

public abstract class BaseApiService
{
    protected readonly string _clientName;
    protected readonly HttpClient _httpClient;

    protected BaseApiService(IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _clientName = configuration["ApiSettings:ClientName"] ?? "RetailApi";
        _httpClient = httpClientFactory.CreateClient(_clientName);
    }

    protected Dictionary<string, string?> GetBaseQuery(PagingRequest request)
    {
        return new Dictionary<string, string?>
        {
            ["PageNumber"] = request.PageNumber.ToString(),
            ["PageSize"] = request.PageSize.ToString(),
            ["SortBy"] = request.SortBy,
            ["IsDescending"] = request.IsDescending.ToString().ToLower(),
            ["Search"] = request.Search
        };
    }
}