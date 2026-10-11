using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Authentication.Microsoft;

public sealed class MicrosoftOAuthOptions
{
    public const string SectionName = "Authentication:Microsoft";
    public const string PersonalTenantId = "9188040d-6c67-4c5b-b112-36a304b66dad";
    public const string IssuerTemplate = "https://login.microsoftonline.com/{tenantid}/v2.0";

    [Required] public string ClientId { get; init; } = "";
    [Required] public string ClientSecret { get; init; } = "";
    [Required] public string TenantId { get; init; } = "common";
    [Required, Url] public string RedirectUri { get; init; } = "";

    // TenantId is configuration, never supplied by a request or an unverified token.
    public string Authority => $"https://login.microsoftonline.com/{TenantId.ToLowerInvariant()}";

    public bool AllowsTenant(Guid tenant) => TenantId.ToLowerInvariant() switch
    {
        "common" => true,
        "organizations" => tenant != Guid.Parse(PersonalTenantId),
        "consumers" => tenant == Guid.Parse(PersonalTenantId),
        _ => Guid.TryParseExact(TenantId, "D", out var configured) && configured == tenant
    };
}
