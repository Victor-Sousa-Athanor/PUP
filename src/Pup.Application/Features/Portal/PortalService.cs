using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Coleta;
using Pup.Data.Comunicados;
using Pup.Data.Intervencoes;

namespace Pup.Application.Features.Portal;

/// <summary>Entrega as listas do portal público no formato esperado pelo front estático. Só leitura.</summary>
public sealed class PortalService(
    ColetaQuery coletas,
    PontoDescarteQuery pontos,
    IntervencaoQuery intervencoes,
    ComunicadoQuery comunicados,
    IRelogio relogio)
{
    public async Task<IReadOnlyList<PortalServicoDto>> ServicosAsync(CancellationToken ct = default)
    {
        var hoje = relogio.Hoje;

        // Consultas sequenciais: o mesmo AppDbContext não aceita consultas em paralelo.
        var listaColetas = await coletas.ListarAsync(null, null, null, hoje, ct);
        var listaPontos = await pontos.ListarAsync(null, null, false, ct);
        var listaIntervencoes = await intervencoes.ListarAsync(new FiltroIntervencoes(), hoje, 500, ct);

        return listaColetas.Select(c => MapeadorPortal.DeColeta(c, relogio.DataLocal(c.AtualizadoEm)))
            .Concat(listaPontos.Select(p => MapeadorPortal.DePonto(p, relogio.DataLocal(p.AtualizadoEm))))
            .Concat(listaIntervencoes.Select(i => MapeadorPortal.DeIntervencao(i, relogio.DataLocal(i.AtualizadoEm))))
            .OrderByDescending(s => s.AtualizadoEm)
            .ThenBy(s => s.Titulo)
            .ToList();
    }

    public async Task<IReadOnlyList<PortalComunicadoDto>> ComunicadosAsync(CancellationToken ct = default)
    {
        var lista = await comunicados.ListarAsync(null, null, true, false, relogio.Hoje, 100, ct);
        return lista.Select(c => MapeadorPortal.DeComunicado(c, relogio.DataLocal(c.PublicadoEm))).ToList();
    }
}
