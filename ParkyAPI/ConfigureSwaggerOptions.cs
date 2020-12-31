using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ParkyAPI
{
    public class ConfigureSwaggerOptions :Microsoft.Extensions.Options.IConfigureOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>
    {
        readonly Microsoft.AspNetCore.Mvc.ApiExplorer.IApiVersionDescriptionProvider provider;
        
        public ConfigureSwaggerOptions(Microsoft.AspNetCore.Mvc.ApiExplorer.IApiVersionDescriptionProvider provider) => this.provider = provider;
        
        public void Configure(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            foreach (var desc in provider.ApiVersionDescriptions) {
                options.SwaggerDoc(
                    desc.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = $"Parky API {desc.ApiVersion}",
                        Version = desc.ApiVersion.ToString()
                    });

            }

            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description =
               "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
               "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
               "Example: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In =Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type =Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type =Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In =Microsoft.OpenApi.Models.ParameterLocation.Header,

                        },
                        new System.Collections.Generic.List<string>()
                    }
                });


            var xmlCommentFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var cmlCommentsFullPath =System.IO.Path.Combine(System.AppContext.BaseDirectory, xmlCommentFile);
            options.IncludeXmlComments(cmlCommentsFullPath);
        }
    }
}
