namespace RetailCore.CustomerSite.Middlewares;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Bắt trường hợp Response trả về 401 mà không qua Exception (do Authorize Filter)
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                HandleUnauthorized(context);
            }
        }
        catch (Exception ex)
        {
            // Bắt trường hợp HttpClient ném ra HttpRequestException khi gọi API
            if (ex is HttpRequestException httpEx && httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HandleUnauthorized(context);
                return;
            }

            throw; 
        }
    }
    private void HandleUnauthorized(HttpContext context)
    {
        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                      context.Request.Headers["Accept"].ToString().Contains("application/json");

        if (isAjax)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            var returnUrl = context.Request.Path + context.Request.QueryString;
            context.Response.Redirect($"/Auth/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}");
        }
    }
}