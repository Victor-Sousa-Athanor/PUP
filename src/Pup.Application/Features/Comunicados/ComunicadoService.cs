using Pup.Application.Comum;
using Pup.Application.Features.Regioes;
using Pup.Contracts;
using Pup.Data.Comunicados;
using Pup.Domain.Comum;
using Pup.Domain.Comunicados;

namespace Pup.Application.Features.Comunicados;

public sealed class ComunicadoService(
    ComunicadoRepo repo,
    ComunicadoQuery query,
    RegiaoService regioes,
    IRelogio relogio)
{
    public async Task<IReadOnlyList<ComunicadoDto>> ListarAsync(CategoriaComunicado? categoria, Guid? regiaoId,
        bool somenteVigentes, bool somenteDestaques, CancellationToken ct = default)
    {
        var itens = await query.ListarAsync(categoria, somenteDestaques, ct);
        var mapaRegioes = await regioes.ObterMapaAsync(ct);
        var hoje = relogio.Hoje;

        return itens
            .Where(c => regiaoId is null || c.RegiaoId == null || c.RegiaoId == regiaoId)
            .Where(c => !somenteVigentes || c.Vigente(hoje))
            .OrderByDescending(c => c.Destaque && c.Vigente(hoje))
            .ThenByDescending(c => c.PublicadoEm)
            .Select(c => Mapear(c, mapaRegioes, hoje))
            .ToList();
    }

    public async Task<ComunicadoDto> ObterAsync(Guid id, CancellationToken ct = default)
    {
        var c = await query.ObterAsync(id, ct) ?? throw NaoEncontrado();
        var mapaRegioes = await regioes.ObterMapaAsync(ct);
        return Mapear(c, mapaRegioes, relogio.Hoje);
    }

    public async Task<int> ContarVigentesAsync(CancellationToken ct = default) =>
        (await ListarAsync(null, null, somenteVigentes: true, somenteDestaques: false, ct)).Count;

    public async Task<Guid> CriarAsync(SalvarComunicadoRequest r, CancellationToken ct = default)
    {
        await GarantirRegiaoAsync(r.RegiaoId, ct);
        var c = Comunicado.Criar(r.Titulo, r.Resumo, r.Conteudo, r.Categoria, r.RegiaoId, r.ValidoAte, r.Destaque,
            r.FonteOficialUrl, relogio.Hoje);
        repo.Adicionar(c);
        await repo.SalvarAsync(ct);
        return c.Id;
    }

    public async Task AtualizarAsync(Guid id, SalvarComunicadoRequest r, CancellationToken ct = default)
    {
        var c = await CarregarAsync(id, ct);
        await GarantirRegiaoAsync(r.RegiaoId, ct);
        c.Atualizar(r.Titulo, r.Resumo, r.Conteudo, r.Categoria, r.RegiaoId, r.ValidoAte, r.Destaque, r.FonteOficialUrl, relogio.Hoje);
        await repo.SalvarAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
    }

    private static ComunicadoDto Mapear(Comunicado c, IReadOnlyDictionary<Guid, RegiaoDto> mapa, DateOnly hoje)
    {
        var regiaoNome = c.RegiaoId is { } rid && mapa.TryGetValue(rid, out var r) ? r.Nome : null;
        return new ComunicadoDto(
            c.Id, c.Titulo, c.Resumo, c.Conteudo, c.Categoria, c.RegiaoId, regiaoNome,
            c.PublicadoEm, c.ValidoAte, c.Destaque, c.Vigente(hoje), c.FonteOficialUrl, c.AtualizadoEm);
    }

    /// <summary>Região é opcional no comunicado (nulo = distrito inteiro).</summary>
    private async Task GarantirRegiaoAsync(Guid? regiaoId, CancellationToken ct)
    {
        if (regiaoId is { } id && id != Guid.Empty) await regioes.GarantirExisteAsync(id, ct);
    }

    private async Task<Comunicado> CarregarAsync(Guid id, CancellationToken ct) =>
        await repo.ObterPorIdAsync(id, ct) ?? throw NaoEncontrado();

    private static NaoEncontradoException NaoEncontrado() => new("Comunicado não encontrado.");
}
