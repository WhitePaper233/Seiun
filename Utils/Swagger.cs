using Swashbuckle.AspNetCore.SwaggerGen;

namespace Seiun.Utils;

public static class Swagger
{
	public static void ConfigureSwaggerGen(SwaggerGenOptions c)
	{
		c.AddSecurityDefinition("Bearer",
		new Microsoft.OpenApi.Models.OpenApiSecurityScheme
		{
			Name = "Authorization",
			Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
			Scheme = "Bearer",
			BearerFormat = "JWT",
			In = Microsoft.OpenApi.Models.ParameterLocation.Header
		});

		c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
		{
			{
				new Microsoft.OpenApi.Models.OpenApiSecurityScheme
				{
					Reference = new Microsoft.OpenApi.Models.OpenApiReference
					{
						Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				},
				[]
			}
		});
	}
}
