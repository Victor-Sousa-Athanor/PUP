using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pup.Application.Abstracoes;
using Pup.Application.Comum;
using Pup.Application.Features.Acesso;
using Pup.Application.Features.Coleta;
using Pup.Application.Features.Comunicados;
using Pup.Application.Features.Inicializacao;
using Pup.Application.Features.Intervencoes;
using Pup.Application.Features.Painel;
using Pup.Application.Features.Regioes;

namespace Pup.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddPupApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PupOptions>(configuration.GetSection(PupOptions.Secao));

        services.AddSingleton<IRelogio, Relogio>();
        services.AddSingleton<SenhaHasher>();
        services.AddScoped<IDispatcher, Dispatcher>();

        // Services (um por entidade persistida) — consomem Repo e Query
        services.AddScoped<RegiaoService>();
        services.AddScoped<ColetaService>();
        services.AddScoped<PontoDescarteService>();
        services.AddScoped<IntervencaoService>();
        services.AddScoped<ComunicadoService>();
        services.AddScoped<AdministradorService>();
        services.AddScoped<PainelService>();
        services.AddScoped<DadosExemplo>();
        services.AddScoped<InicializacaoService>();

        RegistrarHandlers(services);
        return services;
    }

    /// <summary>Registra todos os ICommandHandler/IQueryHandler do assembly (slices).</summary>
    private static void RegistrarHandlers(IServiceCollection services)
    {
        var abertos = new[] { typeof(ICommandHandler<,>), typeof(IQueryHandler<,>) };
        var tipos = typeof(DependencyInjection).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false });

        foreach (var tipo in tipos)
            foreach (var itf in tipo.GetInterfaces().Where(i => i.IsGenericType && abertos.Contains(i.GetGenericTypeDefinition())))
                services.AddScoped(itf, tipo);
    }
}
