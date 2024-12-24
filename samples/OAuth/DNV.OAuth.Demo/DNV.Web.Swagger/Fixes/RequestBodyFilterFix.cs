using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace DNV.Web.Swagger.Fixes;

public class RequestBodyFilterFix : IRequestBodyFilter
{
    public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
    {
        FixRequired(requestBody, context);
        FixContentTypes(requestBody);

        // mark request body as required in swagger
        static void FixRequired(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
        {
            requestBody.Required = context.BodyParameterDescription.ModelMetadata.IsRequired;
        }

        // remove all content types except application/json
        static void FixContentTypes(OpenApiRequestBody requestBody)
        {
            var contentType = requestBody.Content.Values.FirstOrDefault();

            if (contentType is null) return;

            requestBody.Content.Clear();
            requestBody.Content.Add("application/json", contentType);
        }
    }
}
