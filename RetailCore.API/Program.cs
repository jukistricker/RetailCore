using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RetailCore.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

string? adminEmail = builder.Configuration["AdminEmail"];
string? adminPassword = builder.Configuration["AdminPassword"];

if (adminEmail == null || adminPassword == null)
{
    throw new Exception("AdminEmail or AdminPassword is not configured");
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser<Guid>, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole(RetailCore.Domain.Constants.Roles.Admin));
});

var app = builder.Build();

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();