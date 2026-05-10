namespace RetailCore.Infrastructure.Identity;

public class IdentityServerOptions
{
    public string Authority { get; set; }
    public string ApiScopeName { get; set; }
    public string AdminClientId { get; set; }
    public string AdminClientName { get; set; }
    public string AdminSecret { get; set; }
    public string CustomerClientId { get; set; }
    public string CustomerClientName { get; set; }
    public string CustomerSecret { get; set; } 
    public string CustomerBaseUrl { get; set; } 
}