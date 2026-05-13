using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RetailCore.Infrastructure.Data.Configurations;

namespace RetailCore.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<ProductRating> ProductRatings => Set<ProductRating>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new ProductImageConfiguration());
        builder.ApplyConfiguration(new CustomerConfiguration());
        builder.ApplyConfiguration(new OrderConfiguration());
        builder.ApplyConfiguration(new OrderItemConfiguration());
        builder.ApplyConfiguration(new CartItemConfiguration());
        builder.ApplyConfiguration(new ProductRatingConfiguration());
        builder.ApplyConfiguration(new ProductAttributeConfiguration());
    }

    public static async Task SeedRolesAndAdminAsync(IServiceProvider service, string adminEmail, string adminPassword)
    {
        var roleManager = service.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = service.GetRequiredService<UserManager<IdentityUser<Guid>>>();
        var dbContext = service.GetRequiredService<AppDbContext>(); 

        foreach (var roleName in Domain.Constants.Roles.AllRoles)
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>(roleName)
                {
                    Id = Guid.CreateVersion7()
                };
                await roleManager.CreateAsync(role);
            }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new IdentityUser<Guid>
            {
                Id = Guid.CreateVersion7(),
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                Customer adminCustomer = new Customer
                {
                    Id = Guid.CreateVersion7(),
                    UserId = admin.Id,
                    FullName = adminEmail,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                };
                await dbContext.Customers.AddAsync(adminCustomer);

                await userManager.AddToRoleAsync(admin, Domain.Constants.Roles.Admin);
                await dbContext.SaveChangesAsync();
            }

        }
    }
}