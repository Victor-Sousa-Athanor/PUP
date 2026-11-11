using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pup.Application.Comum;
using Pup.Application.Features.Acesso;
using Pup.Data;

namespace Pup.Application.Features.Inicializacao;

/// <summary>Executado uma vez na subida da API: prepara o banco, cria o admin inicial e os dados de exemplo.</summary>
public sealed class InicializacaoService(
    BancoRepo banco,
    AdministradorService administradores,
    DadosExemplo exemplos,
    IOptions<PupOptions> options,
    ILogger<InicializacaoService> logger)
{
    public async Task ExecutarAsync(CancellationToken ct = default)
    {
        await banco.PrepararAsync(ct);

        var opcoes = options.Value;
        if (await administradores.GarantirAdminInicialAsync(opcoes.AdminInicial, ct))
            logger.LogInformation("Administrador inicial criado: {Email}", opcoes.AdminInicial.Email);
        else if (string.IsNullOrWhiteSpace(opcoes.AdminInicial.Senha))
            logger.LogWarning("Pup:AdminInicial não configurado. Defina e-mail e senha para conseguir entrar na área administrativa.");

        if (opcoes.SemearDadosExemplo && !await banco.TemRegioesAsync(ct))
        {
            await exemplos.SemearAsync(ct);
            logger.LogInformation("Dados de exemplo de São Francisco Xavier criados.");
        }
    }
}
