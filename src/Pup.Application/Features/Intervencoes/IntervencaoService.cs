using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Intervencoes;
using Pup.Data.Regioes;
using Pup.Domain.Intervencoes;

namespace Pup.Application.Features.Intervencoes;

/// <summary>Obras e serviços de manutenção (mesma entidade, separadas pelo campo Tipo).</summary>
public sealed class IntervencaoService(IntervencaoRepo repo, IntervencaoQuery query, RegiaoRepo regioes, IRelogio relogio)
{
    public Task<IReadOnlyList<IntervencaoResumoDto>> ListarAsync(FiltroIntervencoes filtro, CancellationToken ct) =>
        query.ListarAsync(filtro, relogio.Hoje, ct: ct);

    public async Task<IntervencaoDetalheDto> ObterAsync(Guid id, CancellationToken ct) =>
        await query.ObterAsync(id, relogio.Hoje, ct) ?? throw NaoEncontrada();

    public async Task<Guid> CriarAsync(CriarIntervencaoRequest r, CancellationToken ct)
    {
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        var i = Intervencao.Criar(r.Tipo, r.Titulo, r.Descricao, r.RegiaoId, r.Local, r.Status, r.DataInicio,
            r.PrevisaoConclusao, r.ImpactoNaVia, r.FonteOficialUrl);
        repo.Adicionar(i);
        await repo.SalvarAsync(ct);
        return i.Id;
    }

    public async Task AtualizarAsync(Guid id, AtualizarIntervencaoRequest r, CancellationToken ct)
    {
        var i = await CarregarAsync(id, ct);
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        i.AtualizarDados(r.Titulo, r.Descricao, r.RegiaoId, r.Local, r.DataInicio, r.PrevisaoConclusao, r.ImpactoNaVia, r.FonteOficialUrl);
        await repo.SalvarAsync(ct);
    }

    public async Task<Guid> MudarStatusAsync(Guid id, MudarStatusRequest r, CancellationToken ct)
    {
        var i = await CarregarAsync(id, ct);
        var atualizacao = i.MudarStatus(r.Status, r.Nota, relogio.Hoje);
        await repo.SalvarAsync(ct);
        return atualizacao.Id;
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private async Task<Intervencao> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrada();

    private static NaoEncontradoException NaoEncontrada() => new("Obra ou manutenção não encontrada.");
}
