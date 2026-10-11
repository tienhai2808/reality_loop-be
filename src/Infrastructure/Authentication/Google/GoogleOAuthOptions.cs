using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Authentication.Google;

public sealed class GoogleOAuthOptions
{
    public const string SectionName = "Authentication:Google";
    [Required] public string ClientId { get; init; } = "";
    [Required] public string ClientSecret { get; init; } = "";
    [Required, Url] public string RedirectUri { get; init; } = "";
}
