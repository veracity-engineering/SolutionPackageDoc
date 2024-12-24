using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DNV.Web.Swagger.Fixes;

public class SchemaFilterFix : ISchemaFilter
{
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		FixAdditionalProperties(schema);
		FixNullableProperties(schema);

		// prevents generating 'additionalProperties: false' in swagger
		static void FixAdditionalProperties(OpenApiSchema schema)
		{
			schema.AdditionalPropertiesAllowed = true;
		}

		static void FixNullableProperties(OpenApiSchema schema)
		{
			schema.Properties?.ForEach(p => p.Value.SetNullable(p.Value.Nullable == true));
		}
	}
}
