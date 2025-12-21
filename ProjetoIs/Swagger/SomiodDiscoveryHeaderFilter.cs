using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

public class SomiodDiscoveryHeaderFilter : IOperationFilter
{
    public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
    {
        if (operation.parameters == null)
            operation.parameters = new List<Parameter>();

        operation.parameters.Add(new Parameter
        {
            name = "somiod-discovery",
            @in = "header",
            type = "string",
            required = false,
            description = "Discovery mode (application | container | content-instance | subscription)"
        });
    }
}
