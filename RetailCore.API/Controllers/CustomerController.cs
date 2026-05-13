namespace RetailCore.API.Controllers;

[Route("api/customers")]
public class CustomerController:ApiControllerBase
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync([FromQuery]PagingRequest request)
    {
        var result = await _customerService.GetAllUsersAsync(request);
        return HandleResult(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(Guid id)
    {
        var result = await _customerService.GetUserByIdAsync(id);
        return HandleResult(result);
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateUserAsync(Guid id, UpdateCustomerRequest request)
    {
        var result = await _customerService.UpdateUserAsync(id, request);
        return HandleResult(result);
    }
}