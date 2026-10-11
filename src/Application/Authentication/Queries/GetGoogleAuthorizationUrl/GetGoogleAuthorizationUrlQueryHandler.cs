using Application.Authentication.Models;
using Application.Common.Interfaces;

namespace Application.Authentication.Queries.GetGoogleAuthorizationUrl;

public sealed class GetGoogleAuthorizationUrlQueryHandler(IGoogleOAuthClient google)
{
    public GoogleAuthorization Handle(GetGoogleAuthorizationUrlQuery query) => google.CreateAuthorization();
}
