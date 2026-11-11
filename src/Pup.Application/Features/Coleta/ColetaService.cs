using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Coleta;
using Pup.Data.Regioes;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Coleta;

public sealed class ColetaService(ColetaRepo repo, ColetaQuery query, RegiaoRepo regioes, IRelogio relogio)
{
    /// <param name="somenteHoje">Atalho do portal: "quais coletas passam hoje?".</param>
    public Task<IReadOnlyList<ColetaDto>> ListarAsync(Guid? regiaoId, ModalidadeColeta? modalidade, DiasSemana? dia, bool somenteHoje,
        CancellationToken ct)
    {
        var hoje = relogio.Hoje;
        if (somenteHoje) dia = hoje.DayOfWeek.ParaFlag();
        return query.ListarAsync(regiaoId, modalidade, dia, hoje, ct);
    }

    public async Task<ColetaDto> ObterAsync(Guid id, CancellationToken ct) =>
        await query.ObterAsync(id, relogio.Hoje, ct) ?? throw NaoEncontrada();

    public async Task<Guid> CriarAsync(SalvarColetaRequest r, CancellationToken ct)
    {
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        var coleta = ColetaResiduos.Criar(r.RegiaoId, r.Modalidade, r.Dias, r.HorarioInicio, r.HorarioFim, r.Observacao, r.FonteOficialUrl);
        repo.Adicionar(coleta);
        await repo.SalvarAsync(ct);
        return coleta.Id;
    }

    public async Task AtualizarAsync(Guid id, SalvarColetaRequest r, CancellationToken ct)
    {
        var coleta = await CarregarAsync(id, ct);
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        coleta.Atualizar(r.RegiaoId, r.Modalidade, r.Dias, r.HorarioInicio, r.HorarioFim, r.Observacao, r.FonteOficialUrl);
        await repo.SalvarAsync(ct);
    }

    public async Task AlterarTemporariamenteAsync(Guid id, AlterarColetaRequest r, CancellationToken ct)
    {
        var coleta = await CarregarAsync(id, ct);
        coleta.AlterarTemporariamente(r.Situacao, r.Aviso, r.Ate, relogio.Hoje);
        await repo.SalvarAsync(ct);
    }

    public async Task NormalizarAsync(Guid id, CancellationToken ct)
    {
        var coleta = await CarregarAsync(id, ct);
        coleta.Normalizar();
        await repo.SalvarAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private async Task<ColetaResiduos> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrada();

    private static NaoEncontradoException NaoEncontrada() => new("Coleta não encontrada.");
}
