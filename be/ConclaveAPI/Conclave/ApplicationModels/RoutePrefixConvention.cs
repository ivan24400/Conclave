using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;

namespace Conclave.ApplicationModels
{
    public class RoutePrefixConvention : IApplicationModelConvention
    {
        private readonly AttributeRouteModel _attributeRouteModel;

        public RoutePrefixConvention(IRouteTemplateProvider routeTemplate)
        {
            _attributeRouteModel = new AttributeRouteModel(routeTemplate);
        }

        public void Apply(ApplicationModel applicationModel)
        {
            foreach (var controller in applicationModel.Controllers.SelectMany(x => x.Selectors))
            {
                if (controller.AttributeRouteModel != null)
                {
                    controller.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_attributeRouteModel, controller.AttributeRouteModel);
                }
                else
                {
                    controller.AttributeRouteModel = _attributeRouteModel;
                }
            }
        }
    }
}
