using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Coleta;
using Pup.Data.Regioes;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Coleta;

public sealed class PontoDescarteService(PontoDescarteRepo repo, PontoDescarteQuery query, RegiaoRepo regioes)
{
    public Task<IReadOnlyList<PontoDescarteDto>> ListarAsync(Guid? regiaoId, TipoPontoDescarte? tipo, bool incluirInativos,
        CancellationToken ct) =>
        query.ListarAsync(regiaoId, tipo, incluirInativos, ct);

    public async Task<PontoDescarteDto> ObterAsync(Guid id, CancellationToken ct) =>
        await query.ObterAsync(id, ct) ?? throw NaoEncontrado();

    public async Task<Guid> CriarAsync(SalvarPontoDescarteRequest r, CancellationToken ct)
    {
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        var ponto = PontoDescarte.Criar(r.Nome, r.Tipo, r.RegiaoId, r.Referencia, r.Latitude, r.Longitude, r.Orientacoes, r.FonteOficialUrl);
        repo.Adicionar(ponto);
        await repo.SalvarAsync(ct);
        return ponto.Id;
    }

    public async Task AtualizarAsync(Guid id, SalvarPontoDescarteRequest r, CancellationToken ct)
    {
        var ponto = await CarregarAsync(id, ct);
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        ponto.Atualizar(r.Nome, r.Tipo, r.RegiaoId, r.Referencia, r.Latitude, r.Longitude, r.Orientacoes, r.FonteOficialUrl, r.Ativo);
        await repo.SalvarAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private async Task<PontoDescarte> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrado();

    private static NaoEncontradoException NaoEncontrado() => new("Ponto de descarte não encontrado.");
}
