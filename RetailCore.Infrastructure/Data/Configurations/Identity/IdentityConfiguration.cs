using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace RetailCore.Infrastructure.Data.Configurations.Identity;

// Tìm hiểu của em về cấu hình Identity Server
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
    public static IEnumerable<ApiScope> GetApiScopes(IdentityServerOptions options)
    {
        return new List<ApiScope>
        {
            new(options.ApiScopeName, "Full Access to Retail Core API")
        };
    }

    // Định nghĩa ApiResource để gắn Claim vào Access Token
    public static IEnumerable<ApiResource> GetApiResources(IdentityServerOptions options)
    {
        return new List<ApiResource>
        {
            new(options.ApiScopeName, "Retail Core API")
            {
                UserClaims = { "role", "customer_id" },
                Scopes = { options.ApiScopeName }
            }
        };
    }

    // 3. Cấu hình các ứng dụng Client
    public static IEnumerable<Client> GetClients(IdentityServerOptions options)
    {
        return new List<Client>
        {
            // Client dành cho React Admin (Dùng Password Grant để gói vào Cookie)
            new()
            {
                ClientId = options.AdminClientId, //tên của client
                ClientName = options.AdminClientName, //tên hiển thị trên giao diện lúc đăng nhập
                AllowedGrantTypes =
                    GrantTypes
                        .ResourceOwnerPassword, // Cho phép gửi trực tiếp username và password từ client lên server
                ClientSecrets = { new Secret(options.AdminSecret.Sha256()) }, //Mật khẩu của ứng dụng 
                AlwaysIncludeUserClaimsInIdToken = true,
                AllowOfflineAccess = true, // Quan trọng: Cho phép cấp refresh token 
                AccessTokenLifetime = 900, // 15 phút
                RefreshTokenUsage = TokenUsage.OneTimeOnly, //refresh token chỉ được sử dụng 1 lần rồi sẽ bị hủy 
                RefreshTokenExpiration =
                    TokenExpiration.Sliding, //nếu đang dùng app liên tục, refresh token sẽ tự động được gia hạn thêm
                AbsoluteRefreshTokenLifetime = 604800, // 7 ngày

                //Danh sách các quyền của client này
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    options.ApiScopeName
                }
            },

            // Client dành cho Customer Site 
            new()
            {
                ClientId = options.CustomerClientId,
                ClientName = options.CustomerClientName,
                AllowedGrantTypes = GrantTypes.Code, //dùng mã tạm thời để lấy token
                RequirePkce = true, //khóa bảo vệ đi kèm GrantTypes.Code để chống đánh chặn mã tạm thời
                ClientSecrets = { new Secret(options.CustomerSecret.Sha256()) },
                AlwaysIncludeUserClaimsInIdToken = true,
                RedirectUris = { $"{options.CustomerBaseUrl}/signin-oidc" },
                PostLogoutRedirectUris = { $"{options.CustomerBaseUrl}/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AccessTokenLifetime = 900,
                RefreshTokenUsage = TokenUsage.OneTimeOnly,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AbsoluteRefreshTokenLifetime = 604800,

                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    options.ApiScopeName
                }
            }
        };
    }
}