using Pup.Application.Comum;
using Pup.Application.Features.Coleta;
using Pup.Application.Features.Comunicados;
using Pup.Application.Features.Intervencoes;
using Pup.Application.Features.PontosDescarte;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Painel;

/// <summary>Página inicial e busca do topo. Orquestra apenas os Services das entidades.</summary>
public sealed partial class PainelService(
    ColetaService coletas,
    PontoDescarteService pontos,
    IntervencaoService intervencoes,
    ComunicadoService comunicados)
{
    public async Task<PainelDto> ObterAsync(int limite, CancellationToken ct = default)
    {
        limite = Math.Clamp(limite, 1, 50);

        // Sequencial: o mesmo AppDbContext subjacente não aceita operações concorrentes.
        var itens = await MontarItensAsync(incluirVencidos: false, ct);
        var ultimas = itens.OrderByDescending(i => i.AtualizadoEm).Take(limite).ToList();
        var destaques = (await comunicados.ListarAsync(null, null, true, true, ct)).Take(3).ToList();
        var coletasAlteradas = await coletas.ContarAlteradasAsync(ct);
        var obras = await intervencoes.ContarAsync(TipoIntervencao.Obra, somenteEmAndamento: true, ct);
        var manutencoes = await intervencoes.ContarAsync(TipoIntervencao.Manutencao, somenteEmAndamento: false, ct);
        var vigentes = await comunicados.ContarVigentesAsync(ct);

        return new PainelDto(ultimas, destaques, coletasAlteradas, obras, manutencoes, vigentes);
    }

    public async Task<IReadOnlyList<ItemAtualizacaoDto>> BuscarAsync(string? termo, CancellationToken ct = default)
    {
        var t = termo?.Trim();
        if (string.IsNullOrEmpty(t) || t.Length < 2)
            throw new DomainException("Digite pelo menos 2 letras para buscar.");
        if (t.Length > 100) t = t[..100];

        var itens = await MontarItensAsync(incluirVencidos: true, ct);
        return itens
            .Where(i => Contem(i, t))
            .OrderByDescending(i => i.AtualizadoEm)
            .Take(30)
            .ToList();
    }
}
