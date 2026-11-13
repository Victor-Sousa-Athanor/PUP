using Pup.Application.Comum;
using Pup.Application.Features.Regioes;
using Pup.Contracts;
using Pup.Data.Coleta;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Coleta;

public sealed class ColetaService(ColetaRepo repo, ColetaQuery query, RegiaoService regioes, IRelogio relogio)
{
    /// <param name="somenteHoje">Atalho do portal: "quais coletas passam hoje?".</param>
    public async Task<IReadOnlyList<ColetaDto>> ListarAsync(Guid? regiaoId, ModalidadeColeta? modalidade,
        DiasSemana? dia, bool somenteHoje, CancellationToken ct = default)
    {
        var hoje = relogio.Hoje;
        if (somenteHoje) dia = hoje.DayOfWeek.ParaFlag();

        var itens = await query.ListarAsync(regiaoId, modalidade, dia, ct);
        var mapaRegioes = await regioes.ObterMapaAsync(ct);

        return itens
            .Select(c => Mapear(c, mapaRegioes, hoje))
            .OrderBy(c => c.RegiaoTipo)
            .ThenBy(c => c.RegiaoNome)
            .ThenBy(c => c.Modalidade)
            .ToList();
    }

    public async Task<ColetaDto> ObterAsync(Guid id, CancellationToken ct = default)
    {
        var coleta = await query.ObterAsync(id, ct) ?? throw NaoEncontrada();
        var regiao = await regioes.ObterAsync(coleta.RegiaoId, ct);
        return Mapear(coleta, regiao, relogio.Hoje);
    }

    public async Task<int> ContarAlteradasAsync(CancellationToken ct = default)
    {
        var lista = await ListarAsync(null, null, null, false, ct);
        return lista.Count(c => c.Situacao != SituacaoColeta.Regular);
    }

    public async Task<Guid> CriarAsync(SalvarColetaRequest r, CancellationToken ct = default)
    {
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        var coleta = ColetaResiduos.Criar(r.RegiaoId, r.Modalidade, r.Dias, r.HorarioInicio, r.HorarioFim, r.Observacao, r.FonteOficialUrl);
        repo.Adicionar(coleta);
        await repo.SalvarAsync(ct);
        return coleta.Id;
    }

    public async Task AtualizarAsync(Guid id, SalvarColetaRequest r, CancellationToken ct = default)
    {
        var coleta = await CarregarAsync(id, ct);
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        coleta.Atualizar(r.RegiaoId, r.Modalidade, r.Dias, r.HorarioInicio, r.HorarioFim, r.Observacao, r.FonteOficialUrl);
        await repo.SalvarAsync(ct);
    }

    public async Task AlterarTemporariamenteAsync(Guid id, AlterarColetaRequest r, CancellationToken ct = default)
    {
        var coleta = await CarregarAsync(id, ct);
        coleta.AlterarTemporariamente(r.Situacao, r.Aviso, r.Ate, relogio.Hoje);
        await repo.SalvarAsync(ct);
    }

    public async Task NormalizarAsync(Guid id, CancellationToken ct = default)
    {
        var coleta = await CarregarAsync(id, ct);
        coleta.Normalizar();
        await repo.SalvarAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private static ColetaDto Mapear(ColetaResiduos c, IReadOnlyDictionary<Guid, RegiaoDto> mapa, DateOnly hoje) =>
        Mapear(c, mapa.TryGetValue(c.RegiaoId, out var r) ? r : null, hoje);

    private static ColetaDto Mapear(ColetaResiduos c, RegiaoDto? regiao, DateOnly hoje)
    {
        var situacao = c.SituacaoEm(hoje);
        var aviso = situacao == SituacaoColeta.Regular ? null : c.AvisoAlteracao;
        return new ColetaDto(
            c.Id, c.RegiaoId, regiao?.Nome ?? "—", regiao?.Tipo ?? TipoRegiao.Urbana,
            c.Modalidade, c.Dias, c.HorarioInicio, c.HorarioFim, c.Observacao,
            situacao, aviso, c.AlteracaoAte, c.FonteOficialUrl, c.AtualizadoEm);
    }

    private async Task<ColetaResiduos> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrada();

    private static NaoEncontradoException NaoEncontrada() => new("Coleta não encontrada.");
}
