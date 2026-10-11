using System.ComponentModel.DataAnnotations;
using Domain.Users;

namespace Api.Contracts.Authentication;

public sealed record LoginDeviceRequest
{
    [EnumDataType(typeof(SessionDeviceType))] public SessionDeviceType DeviceType { get; init; } = SessionDeviceType.Unknown;
    [StringLength(255)] public string? DeviceName { get; init; }
    [StringLength(255)] public string? Os { get; init; }
    [StringLength(255)] public string? Browser { get; init; }
}
