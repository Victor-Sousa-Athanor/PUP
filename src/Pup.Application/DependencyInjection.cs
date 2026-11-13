using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pup.Application.Comum;
using Pup.Application.Features.Acesso;
using Pup.Application.Features.AtualizacoesIntervencao;
using Pup.Application.Features.Coleta;
using Pup.Application.Features.Comunicados;
using Pup.Application.Features.Inicializacao;
using Pup.Application.Features.Intervencoes;
using Pup.Application.Features.Paginas;
using Pup.Application.Features.Painel;
using Pup.Application.Features.PontosDescarte;
using Pup.Application.Features.Portal;
using Pup.Application.Features.Regioes;

namespace Pup.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPupApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PupOptions>(configuration.GetSection(PupOptions.Secao));

        services.AddSingleton<IRelogio, Relogio>();
        services.AddSingleton<SenhaHasher>();

        // Services (um por entidade persistida + serviços de agregação/inicialização)
        services.AddScoped<RegiaoService>();
        services.AddScoped<ColetaService>();
        services.AddScoped<PontoDescarteService>();
        services.AddScoped<IntervencaoService>();
        services.AddScoped<AtualizacaoIntervencaoService>();
        services.AddScoped<ComunicadoService>();
        services.AddScoped<AdministradorService>();
        services.AddScoped<PaginaService>();
        services.AddScoped<PainelService>();
        services.AddScoped<PortalService>();
        services.AddScoped<BancoService>();
        services.AddScoped<DadosExemplo>();
        services.AddScoped<InicializacaoService>();

        return services;
    }
}
