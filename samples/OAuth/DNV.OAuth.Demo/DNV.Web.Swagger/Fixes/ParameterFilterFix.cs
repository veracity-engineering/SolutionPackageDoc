using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DNV.Web.Swagger.Fixes;

public class ParameterFilterFix : IParameterFilter
{
	public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
	{
		FixRequired(parameter, context);
		FixArrayInQuery(parameter);

		// mark parameter as required in swagger
		static void FixRequired(OpenApiParameter parameter, ParameterFilterContext context)
		{
			if (context.ApiParameterDescription.ModelMetadata?.IsRequired == true)
			{
				parameter.Required = true;
			}

			if (context.ApiParameterDescription.ModelMetadata?.IsNullableValueType != true)
			{
				parameter.Schema.SetNullable(false);
				parameter.Extensions[SwaggerExtensions.NullableKey] = new OpenApiBoolean(false);
			}
		}

		// mark array parameter in query as form
		static void FixArrayInQuery(OpenApiParameter parameter)
		{
			if (parameter.In == ParameterLocation.Query && parameter.Schema.Type == "array")
			{
				parameter.Style = ParameterStyle.Form;
				parameter.Explode = true;
			}
		}
	}
}