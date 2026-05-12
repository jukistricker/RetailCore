namespace RetailCore.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null) return NotFound();

        if (result.IsSuccess)
        {
            if (result.StatusCode == 204)
            {
                return NoContent();
            }
            if (result.Value != null)
                return StatusCode(result.StatusCode, result);
            
            return StatusCode(result.StatusCode);
        }

        return result.StatusCode switch
        {
            400 => BadRequest(result),
            401 => Unauthorized(result),
            403 => Forbid(),
            404 => NotFound(result),
            500 => StatusCode(500, result),
            _ => BadRequest(result)
        };
    }
}