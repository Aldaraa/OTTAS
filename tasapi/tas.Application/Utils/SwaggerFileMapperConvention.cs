using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tas.Application.Utils
{

    public class SwaggerFileMapperConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller?.ControllerType?.Namespace;
            if (controllerNamespace == null) return;
            var namespaceElements = controllerNamespace.Split('.');
            var nextToLastNamespace = namespaceElements.ElementAtOrDefault(namespaceElements.Length - 3)?.ToLowerInvariant();
            var isInClientNamespace = nextToLastNamespace == "auth";
            controller.ApiExplorer.GroupName = isInClientNamespace ? "tas" : "auth";
        }
    }
}
