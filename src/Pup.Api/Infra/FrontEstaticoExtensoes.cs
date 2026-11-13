using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace Pup.Api.Infra;

/// <summary>Configura o serviço de arquivos estáticos do front na mesma origem da API.</summary>
public static class FrontEstaticoExtensoes
{
    public static WebApplication UsarFrontEstatico(this WebApplication app, IConfiguration cfg)
    {
        var pastaFront = cfg["Front:Pasta"];
        if (string.IsNullOrWhiteSpace(pastaFront)) return app;

        var caminhoFront = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, pastaFront));
        if (!Directory.Exists(caminhoFront))
        {
            app.Logger.LogWarning("Front:Pasta não encontrada ({Caminho}). A API sobe sem o front.", caminhoFront);
            return app;
        }

        var arquivosFront = new PhysicalFileProvider(caminhoFront);
        var tipos = CriarTiposPermitidos();
        var paginaDinamica = Path.Combine(caminhoFront, "pagina.html");

        app.Use(async (ctx, next) =>
        {
            await next();
            if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted &&
                EhRotaPaginaDinamica(ctx.Request) && File.Exists(paginaDinamica))
            {
                ctx.Response.StatusCode = StatusCodes.Status200OK;
                ctx.Response.ContentType = "text/html; charset=utf-8";
                await ctx.Response.SendFileAsync(paginaDinamica);
            }
        });

        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = arquivosFront });
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = arquivosFront,
            ContentTypeProvider = tipos,
            OnPrepareResponse = ctx =>
            {
                if (!app.Environment.IsDevelopment()) return;
                // Evita cache de navegador ao alterar HTML/CSS/JS em desenvolvimento.
                ctx.Context.Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
                ctx.Context.Response.Headers.Pragma = "no-cache";
                ctx.Context.Response.Headers.Expires = "0";
            }
        });

        app.Logger.LogInformation("Front estático servido a partir de {Caminho}", caminhoFront);
        return app;
    }

    private static FileExtensionContentTypeProvider CriarTiposPermitidos()
    {
        var tipos = new FileExtensionContentTypeProvider();
        tipos.Mappings.Clear();
        tipos.Mappings[".html"] = "text/html; charset=utf-8";
        tipos.Mappings[".css"] = "text/css; charset=utf-8";
        tipos.Mappings[".js"] = "text/javascript; charset=utf-8";
        tipos.Mappings[".svg"] = "image/svg+xml";
        tipos.Mappings[".png"] = "image/png";
        tipos.Mappings[".jpg"] = "image/jpeg";
        tipos.Mappings[".jpeg"] = "image/jpeg";
        tipos.Mappings[".webp"] = "image/webp";
        tipos.Mappings[".ico"] = "image/x-icon";
        tipos.Mappings[".woff2"] = "font/woff2";
        return tipos;
    }

    private static bool EhRotaPaginaDinamica(HttpRequest req)
    {
        if (!HttpMethods.IsGet(req.Method)) return false;
        var path = req.Path;
        if (path == "/" || Path.HasExtension(path.Value)) return false;
        return !path.StartsWithSegments("/api")
            && !path.StartsWithSegments("/swagger")
            && !path.StartsWithSegments("/openapi")
            && !path.StartsWithSegments("/health");
    }
}
