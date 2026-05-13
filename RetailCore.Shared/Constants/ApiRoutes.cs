namespace RetailCore.Shared.Constants;

public static class ApiRoutes
{
    public const string Base = "api";

    public static class Auth
    {
        public const string AuthBase = $"{Base}/auth";
        public const string Login = $"{AuthBase}/login";
        public const string Register = $"{AuthBase}/register";
        public const string Logout = $"{AuthBase}/logout";
        public const string RefreshToken = $"{AuthBase}/refresh-token";
    }

    public static class Products
    {
        public const string ProductBase = $"{Base}/products";
    }

    public static class Categories
    {
        public const string CategoryBase = $"{Base}/categories";
    }

    public static class CartItems
    {
        public const string CartItemBase = $"{Base}/cart-items";
    }
}