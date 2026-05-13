using Microsoft.AspNetCore.Mvc;
using RetailCore.API.Controllers;
using RetailCore.Domain.Commons;

namespace RetailCore.Test.Helpers;

public class FakeController : ApiControllerBase
{
    public IActionResult TestHandleResult<T>(Result<T> result) => HandleResult(result);
}