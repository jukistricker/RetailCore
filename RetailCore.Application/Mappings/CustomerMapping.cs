namespace RetailCore.Application.Mappings;

public static class CustomerMapping
{
    public static CustomerResponse ToCustomerResponse(Customer customer)
    {
        return new CustomerResponse
            {
                Id =  customer.Id,
                UserId = customer.UserId,
                Phone = customer.Phone,
                Address =  customer.Address,
                City = customer.City,
                FullName = customer.FullName,
                Email = customer.Email,
                IsActive = customer.IsActive,
                CreatedDate = customer.CreatedDate


            };
    }

    public static void ToCustomerUpdate(Customer customer, UpdateCustomerRequest request,Guid customerId)
    {
        customer.Phone = request.Phone;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.FullName = request.FullName;
        customer.Email = request.Email;
        customer.IsActive = request.IsActive;
        customer.UpdatedDate = DateTime.Now;
        customer.UpdatedBy = customerId;
    }
    
    public static PagingResponse<CustomerResponse> ToPagingResponse(PagingResponse<Customer> pagingResponse)
    {
        return new PagingResponse<CustomerResponse>
        {
            Items = pagingResponse.Items.Select(x => ToCustomerResponse(x)).ToList(),
            TotalCount = pagingResponse.TotalCount,
            PageNumber = pagingResponse.PageNumber,
            PageSize = pagingResponse.PageSize
        };
    }
    
}