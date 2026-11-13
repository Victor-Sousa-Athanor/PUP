using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pup.Data.Acesso;
using Pup.Data.AtualizacoesIntervencao;
using Pup.Data.Coleta;
using Pup.Data.Comunicados;
using Pup.Data.Intervencoes;
using Pup.Data.PontosDescarte;
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
        services.AddScoped<AtualizacaoIntervencaoRepo>();
        services.AddScoped<ComunicadoRepo>();
        services.AddScoped<AdministradorRepo>();
        services.AddScoped<BancoRepo>();

        // Queries (leitura simples por entidade)
        services.AddScoped<RegiaoQuery>();
        services.AddScoped<ColetaQuery>();
        services.AddScoped<PontoDescarteQuery>();
        services.AddScoped<IntervencaoQuery>();
        services.AddScoped<AtualizacaoIntervencaoQuery>();
        services.AddScoped<ComunicadoQuery>();
        services.AddScoped<AdministradorQuery>();

        return services;
    }
}
