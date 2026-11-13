using Microsoft.OpenApi;

namespace Pup.Api.Infra;

public static class SwaggerExtensoes
{
    private const string EsquemaBearer = "Bearer";

    public static IServiceCollection AdicionarSwagger(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((doc, _, _) =>
            {
                doc.Info = new OpenApiInfo
                {
                    Title = "Portal de Utilidades Públicas (PUP) API",
                    Version = "v1",
                    Description = "API de coleta, pontos de descarte, obras, manutenções, comunicados e regiões de São Francisco Xavier."
                };

                doc.Components ??= new OpenApiComponents();
                doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                doc.Components.SecuritySchemes[EsquemaBearer] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Informe o token JWT obtido em POST /api/auth/login."
                };

                doc.Security ??= [];
                doc.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(EsquemaBearer, doc)] = []
                });

                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static WebApplication UsarSwagger(this WebApplication app)
    {
        app.MapOpenApi();
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/openapi/v1.json", "PUP API v1");
            o.RoutePrefix = "swagger";
        });
        return app;
    }
}
