using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Authentication.Facebook;

public sealed class FacebookOAuthOptions
{
    public const string SectionName = "Authentication:Facebook";

    [Required, RegularExpression("^[1-9][0-9]{0,31}$")] public string AppId { get; init; } = "";
    [Required] public string AppSecret { get; init; } = "";
    [Required, Url] public string RedirectUri { get; init; } = "";
    [Required, RegularExpression("^v[1-9][0-9]{0,2}\\.0$")] public string GraphApiVersion { get; init; } = "v26.0";

    public string GraphEndpoint => $"https://graph.facebook.com/{GraphApiVersion}";
    public string AuthorizationEndpoint => $"https://www.facebook.com/{GraphApiVersion}/dialog/oauth";
}
