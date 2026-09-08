using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace MyRecipe.Api.Extensions
{
    public static class OpenApiExtensions
    {
        private const string SwaggerProvider = "swagger";
        private const string ScalarProvider = "scalar";
        private const string BothProvider = "both";

        /// <summary>
        /// Configures native OpenAPI generation with JWT security requirements.
        /// </summary>
        public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    // Define the Security Scheme
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Please enter your JWT Bearer token directly into the input box below."
                    };

                    // 2. Fixed: Apply the scheme globally using the new OpenApiSecuritySchemeReference format
                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });

                    return Task.CompletedTask;
                });
            });

            return services;
        }

        /// <summary>
        /// Maps the OpenAPI endpoints and configures the selected interactive API documentation UI.
        /// </summary>
        public static IApplicationBuilder UseOpenApiDocumentation(this WebApplication app, IConfiguration configuration)
        {
            var provider = configuration["ApiDocumentation:Provider"]?.ToLowerInvariant().Trim() ?? SwaggerProvider;
            var defaultRoute = provider is ScalarProvider or BothProvider ? "/scalar" : "/swagger";

            app.MapOpenApi();
            app.MapGet("/docs", () => Results.Redirect(defaultRoute));

            if (provider is SwaggerProvider or BothProvider)
            {
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "MyRecipe API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            if (provider is ScalarProvider or BothProvider)
            {
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("MyRecipe API");
                    options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
                });
            }

            return app;
        }
    }
}
