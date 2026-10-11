using Application.Authentication.Models;
using Application.Common.Interfaces;

namespace Application.Authentication.Queries.GetFacebookAuthorizationUrl;

public sealed class GetFacebookAuthorizationUrlQueryHandler(IFacebookOAuthClient facebook)
{
    public FacebookAuthorization Handle(GetFacebookAuthorizationUrlQuery query) => facebook.CreateAuthorization();
}
