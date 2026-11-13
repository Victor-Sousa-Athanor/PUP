using Pup.Application.Comum;
using Pup.Application.Features.Regioes;
using Pup.Contracts;
using Pup.Data.PontosDescarte;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Application.Features.PontosDescarte;

public sealed class PontoDescarteService(PontoDescarteRepo repo, PontoDescarteQuery query, RegiaoService regioes)
{
    public async Task<IReadOnlyList<PontoDescarteDto>> ListarAsync(Guid? regiaoId, TipoPontoDescarte? tipo,
        bool incluirInativos, CancellationToken ct = default)
    {
        var itens = await query.ListarAsync(regiaoId, tipo, incluirInativos, ct);
        var mapaRegioes = await regioes.ObterMapaAsync(ct);

        return itens
            .Select(p => Mapear(p, mapaRegioes.TryGetValue(p.RegiaoId, out var r) ? r.Nome : "—"))
            .OrderBy(p => p.RegiaoNome)
            .ThenBy(p => p.Nome)
            .ToList();
    }

    public async Task<PontoDescarteDto> ObterAsync(Guid id, CancellationToken ct = default)
    {
        var ponto = await query.ObterAsync(id, ct) ?? throw NaoEncontrado();
        var regiao = await regioes.ObterAsync(ponto.RegiaoId, ct);
        return Mapear(ponto, regiao.Nome);
    }

    public async Task<Guid> CriarAsync(SalvarPontoDescarteRequest r, CancellationToken ct = default)
    {
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        var ponto = PontoDescarte.Criar(r.Nome, r.Tipo, r.RegiaoId, r.Referencia, r.Latitude, r.Longitude, r.Orientacoes, r.FonteOficialUrl);
        repo.Adicionar(ponto);
        await repo.SalvarAsync(ct);
        return ponto.Id;
    }

    public async Task AtualizarAsync(Guid id, SalvarPontoDescarteRequest r, CancellationToken ct = default)
    {
        var ponto = await CarregarAsync(id, ct);
        await regioes.GarantirExisteAsync(r.RegiaoId, ct);
        ponto.Atualizar(r.Nome, r.Tipo, r.RegiaoId, r.Referencia, r.Latitude, r.Longitude, r.Orientacoes, r.FonteOficialUrl, r.Ativo);
        await repo.SalvarAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private static PontoDescarteDto Mapear(PontoDescarte p, string regiaoNome) => new(
        p.Id, p.Nome, p.Tipo, p.RegiaoId, regiaoNome, p.Referencia, p.Latitude, p.Longitude,
        p.Orientacoes, p.Ativo, p.FonteOficialUrl, p.AtualizadoEm);

    private async Task<PontoDescarte> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrado();

    private static NaoEncontradoException NaoEncontrado() => new("Ponto de descarte não encontrado.");
}
