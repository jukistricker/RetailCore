using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetailCore.CustomerSite.Middlewares;
using RetailCore.CustomerSite.Services;
using RetailCore.CustomerSite.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequest>();

builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddHttpClient("RetailApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<ICartItemApiService, CartItemApiService>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LoginPath = "/Auth/Login";

        options.Events = new CookieAuthenticationEvents
        {
            OnValidatePrincipal = async context =>
            {
                // Kiểm tra xem có X-Access-Token không
                var token = context.HttpContext.Request.Cookies["X-Access-Token"];
                if (string.IsNullOrEmpty(token)) context.RejectPrincipal();
            }
        };

        // Quan trọng: Tự động chuyển hướng khi bị Challenge
        options.Events.OnRedirectToLogin = context =>
        {
            if (IsAjaxRequest(context.Request))
                context.Response.StatusCode = 401;
            else
                context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });


bool IsAjaxRequest(HttpRequest request)
{
    return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
           request.Path.StartsWithSegments("/api");
}

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandler>();
app.MapStaticAssets();

app.MapControllerRoute(
        "default",
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();