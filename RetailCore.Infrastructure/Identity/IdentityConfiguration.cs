using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace RetailCore.Infrastructure.Identity;

public static class IdentityConfiguration
{
    // 1. Thông tin định danh cơ bản (OIDC)
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    // 2. Định nghĩa các phạm vi truy cập API
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("retail_api", "Full Access to Retail Core API")
        };

    // 3. Cấu hình các ứng dụng Client
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // A. Client dành cho React Admin (Dùng Password Grant để bạn tự gói vào Cookie)
            new Client
            {
                ClientId = "retail_admin",
                ClientName = "Admin Dashboard (React)",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Phù hợp để gọi từ API Wrapper
                ClientSecrets = { new Secret("admin_secret".Sha256()) },
                
                AllowOfflineAccess = true, // Quan trọng: Để lấy Refresh Token
                AccessTokenLifetime = 900, // 15 phút (theo yêu cầu của Dương)
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AbsoluteRefreshTokenLifetime = 604800, // 7 ngày

                AllowedScopes = 
                { 
                    IdentityServerConstants.StandardScopes.OpenId, 
                    IdentityServerConstants.StandardScopes.Profile,
                    "retail_api" 
                }
            },

            // B. Client dành cho Customer Site (Dùng Authorization Code chuẩn cho MVC)
            new Client
            {
                ClientId = "retail_mvc_customer",
                ClientName = "Customer Portal (MVC)",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                ClientSecrets = { new Secret("mvc_secret".Sha256()) },

                RedirectUris = { "https://localhost:5002/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = 
                { 
                    IdentityServerConstants.StandardScopes.OpenId, 
                    IdentityServerConstants.StandardScopes.Profile,
                    "retail_api" 
                }
            }
        };
}