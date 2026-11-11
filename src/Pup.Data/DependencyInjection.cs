using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pup.Data.Acesso;
using Pup.Data.Coleta;
using Pup.Data.Comunicados;
using Pup.Data.Intervencoes;
using Pup.Data.Painel;
using Pup.Data.Regioes;

namespace Pup.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddPupData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString, npg => npg.EnableRetryOnFailure(3)));

        // Repositórios (escrita)
        services.AddScoped<RegiaoRepo>();
        services.AddScoped<ColetaRepo>();
        services.AddScoped<PontoDescarteRepo>();
        services.AddScoped<IntervencaoRepo>();
        services.AddScoped<ComunicadoRepo>();
        services.AddScoped<AdministradorRepo>();
        services.AddScoped<BancoRepo>();

        // Queries (leitura)
        services.AddScoped<RegiaoQuery>();
        services.AddScoped<ColetaQuery>();
        services.AddScoped<PontoDescarteQuery>();
        services.AddScoped<IntervencaoQuery>();
        services.AddScoped<ComunicadoQuery>();
        services.AddScoped<PainelQuery>();

        return services;
    }
}
