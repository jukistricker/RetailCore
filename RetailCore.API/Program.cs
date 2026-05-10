using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using RetailCore.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

string? adminEmail = builder.Configuration["AdminEmail"];
string? adminPassword = builder.Configuration["AdminPassword"];

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Retail Core API";

        // 1. Định nghĩa Scheme
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-Access-Token",
            In = ParameterLocation.Cookie,
            Description = "Hệ thống sử dụng HttpOnly Cookie."
        };

        document.Components ??= new();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["CookieAuth"] = scheme
        };

        // 2. Định nghĩa Requirement (Cấu trúc mới của .NET 10)
        // Lưu ý: Key của Dictionary lúc này là OpenApiSecuritySchemeReference
        var requirement = new OpenApiSecurityRequirement();
        
        // Tạo reference trỏ đến "CookieAuth" đã định nghĩa ở trên
        var schemeReference = new OpenApiSecuritySchemeReference("CookieAuth", document);
        
        // Gán vào requirement với list các scope (để trống vì dùng Cookie)
        requirement.Add(schemeReference, new List<string>());

        document.Security = new List<OpenApiSecurityRequirement> { requirement };

        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var identityOptions = builder.Configuration
    .GetSection("IdentityConfig")
    .Get<IdentityServerOptions>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddIdentityServer()
    .AddInMemoryClients(IdentityConfiguration.GetClients(identityOptions))
    .AddInMemoryApiScopes(IdentityConfiguration.GetApiScopes(identityOptions)) 
    .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources);

builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = identityOptions.Authority;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => 
        policy.RequireRole(RetailCore.Domain.Constants.Roles.Admin));
    
    options.AddPolicy("CustomerPolicy", policy => 
        policy.RequireRole(RetailCore.Domain.Constants.Roles.Customer));
});

builder.Services.AddAntiforgery(options =>
{
    // Chứng minh không phải là CSRF
    options.HeaderName = "X-XSRF-TOKEN"; 
});
builder.Services.AddControllers();

var app = builder.Build();

app.Use((context, next) =>
{
    var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
    var tokens = antiforgery.GetAndStoreTokens(context);
    // Lưu CSRF Token vào một Cookie KHÔNG HttpOnly để JavaScript ở Frontend đọc được
    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, 
        new CookieOptions { HttpOnly = false, SameSite = SameSiteMode.Lax });
    
    return next(context);
});

app.UseAuthentication(); 
app.UseAuthorization();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("AppDbContext");
    try
    {
        await AppDbContext.SeedRolesAndAdminAsync(services, adminEmail, adminPassword);
        logger.LogInformation("Roles and admin seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding roles and admin.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Vẫn giữ dòng này để sinh file json
    
    app.UseSwaggerUI(options =>
    {
        // Trỏ vào file json mà Microsoft.AspNetCore.OpenApi sinh ra
        options.SwaggerEndpoint("/openapi/v1.json", "Retail Core API v1");
        
        // Đường dẫn mặc định sẽ là /swagger
        options.RoutePrefix = "swagger";
        
        // Quan trọng cho dân Cyber: Cho phép gửi Cookie đi kèm request test
        options.ConfigObject.AdditionalItems["withCredentials"] = true;
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();