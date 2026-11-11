using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Coleta;
using Pup.Data.Comunicados;
using Pup.Data.Intervencoes;
using Pup.Data.Painel;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Painel;

/// <summary>Página inicial (Figura 4 do relatório) e busca do topo. Só leitura.</summary>
public sealed class PainelService(
    PainelQuery painel,
    ColetaQuery coletas,
    IntervencaoQuery intervencoes,
    ComunicadoQuery comunicados,
    IRelogio relogio)
{
    public async Task<PainelDto> ObterAsync(int limite, CancellationToken ct)
    {
        var hoje = relogio.Hoje;
        limite = Math.Clamp(limite, 1, 50);

        // Sequencial de propósito: o mesmo DbContext não aceita consultas em paralelo.
        var ultimas = await painel.UltimasAtualizacoesAsync(limite, hoje, ct);
        var destaques = await comunicados.ListarAsync(null, null, somenteVigentes: true, somenteDestaques: true, hoje, 3, ct);
        var coletasAlteradas = await coletas.ContarAlteradasAsync(hoje, ct);
        var obras = await intervencoes.ContarAsync(TipoIntervencao.Obra, somenteEmAndamento: true, ct);
        var manutencoes = await intervencoes.ContarAsync(TipoIntervencao.Manutencao, somenteEmAndamento: false, ct);
        var vigentes = await comunicados.ContarVigentesAsync(hoje, ct);

        return new PainelDto(ultimas, destaques, coletasAlteradas, obras, manutencoes, vigentes);
    }

    public async Task<IReadOnlyList<ItemAtualizacaoDto>> BuscarAsync(string? termo, CancellationToken ct)
    {
        var t = termo?.Trim();
        if (string.IsNullOrEmpty(t) || t.Length < 2)
            throw new DomainException("Digite pelo menos 2 letras para buscar.");
        if (t.Length > 100) t = t[..100];
        return await painel.BuscarAsync(t, 30, relogio.Hoje, ct);
    }
}
