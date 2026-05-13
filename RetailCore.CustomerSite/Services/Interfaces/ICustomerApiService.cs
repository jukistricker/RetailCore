namespace RetailCore.CustomerSite.Services.Interfaces;

public interface ICustomerApiService
{
    Task<Result<bool>> UpdateCustomerAsync(Guid id, UpdateCustomerRequest request);
    
}