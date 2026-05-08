namespace RetailCore.Domain.Constants;

public static class Roles
{
    public const string Admin    = "Admin";
    public const string Customer = "Customer";
    
    public static readonly string[] AllRoles = [Admin, Customer];
}