using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Api.Common.Routing;

internal sealed class ApiRoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _prefix = new(new RouteAttribute("api/v1"));

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel?.IsAbsoluteTemplate == true ||
                    controller.Actions.SelectMany(x => x.Selectors).Any(x => x.AttributeRouteModel?.IsAbsoluteTemplate == true))
                    throw new InvalidOperationException("Controller and action routes must be relative to the global /api/v1 prefix.");
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_prefix, selector.AttributeRouteModel);
            }
        }
    }
}
