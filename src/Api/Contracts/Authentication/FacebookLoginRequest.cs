using System.ComponentModel.DataAnnotations;

namespace Api.Contracts.Authentication;

public sealed record FacebookLoginRequest
{
    [Required, StringLength(4096)] public string Code { get; init; } = "";
    [Required, StringLength(128)] public string State { get; init; } = "";
    public LoginDeviceRequest? Device { get; init; }
}
