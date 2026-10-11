using System.Net;
using Domain.Users;

namespace Application.Authentication.Models;

public sealed record LoginDevice(SessionDeviceType DeviceType, string? DeviceName, string? Os,
    string? Browser, IPAddress? IpAddress);
