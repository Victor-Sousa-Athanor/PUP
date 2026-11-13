using Pup.Application.Comum;
using Pup.Application.Features.Coleta;
using Pup.Application.Features.Comunicados;
using Pup.Application.Features.Intervencoes;
using Pup.Application.Features.PontosDescarte;
using Pup.Contracts;

namespace Pup.Application.Features.Portal;

/// <summary>Entrega as listas do portal público no formato esperado pelo front estático. Só leitura.</summary>
public sealed class PortalService(
    ColetaService coletas,
    PontoDescarteService pontos,
    IntervencaoService intervencoes,
    ComunicadoService comunicados,
    IRelogio relogio)
{
    public async Task<IReadOnlyList<PortalServicoDto>> ServicosAsync(CancellationToken ct = default)
    {
        // Consultas sequenciais: o mesmo AppDbContext não aceita operações em paralelo.
        var listaColetas = await coletas.ListarAsync(null, null, null, false, ct);
        var listaPontos = await pontos.ListarAsync(null, null, false, ct);
        var listaIntervencoes = await intervencoes.ListarAsync(new FiltroIntervencoes(), ct);

        return listaColetas.Select(c => MapeadorPortal.DeColeta(c, relogio.DataLocal(c.AtualizadoEm)))
            .Concat(listaPontos.Select(p => MapeadorPortal.DePonto(p, relogio.DataLocal(p.AtualizadoEm))))
            .Concat(listaIntervencoes.Select(i => MapeadorPortal.DeIntervencao(i, relogio.DataLocal(i.AtualizadoEm))))
            .OrderByDescending(s => s.AtualizadoEm)
            .ThenBy(s => s.Titulo)
            .ToList();
    }

    public async Task<IReadOnlyList<PortalComunicadoDto>> ComunicadosAsync(CancellationToken ct = default)
    {
        var lista = await comunicados.ListarAsync(null, null, true, false, ct);
        return lista.Select(c => MapeadorPortal.DeComunicado(c, relogio.DataLocal(c.PublicadoEm))).ToList();
    }
}
