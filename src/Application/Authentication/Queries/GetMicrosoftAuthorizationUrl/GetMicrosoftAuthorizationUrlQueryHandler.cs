using Application.Authentication.Models;
using Application.Common.Interfaces;

namespace Application.Authentication.Queries.GetMicrosoftAuthorizationUrl;

public sealed class GetMicrosoftAuthorizationUrlQueryHandler(IMicrosoftOAuthClient microsoft)
{
    public MicrosoftAuthorization Handle(GetMicrosoftAuthorizationUrlQuery query) => microsoft.CreateAuthorization();
}
