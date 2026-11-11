using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Comunicados;
using Pup.Data.Regioes;
using Pup.Domain.Comum;
using Pup.Domain.Comunicados;

namespace Pup.Application.Features.Comunicados;

public sealed class ComunicadoService(ComunicadoRepo repo, ComunicadoQuery query, RegiaoRepo regioes, IRelogio relogio)
{
    public Task<IReadOnlyList<ComunicadoDto>> ListarAsync(CategoriaComunicado? categoria, Guid? regiaoId, bool somenteVigentes,
        bool somenteDestaques, CancellationToken ct) =>
        query.ListarAsync(categoria, regiaoId, somenteVigentes, somenteDestaques, relogio.Hoje, ct: ct);

    public async Task<ComunicadoDto> ObterAsync(Guid id, CancellationToken ct) =>
        await query.ObterAsync(id, relogio.Hoje, ct) ?? throw NaoEncontrado();

    public async Task<Guid> CriarAsync(SalvarComunicadoRequest r, CancellationToken ct)
    {
        await GarantirRegiaoAsync(r.RegiaoId, ct);
        var c = Comunicado.Criar(r.Titulo, r.Resumo, r.Conteudo, r.Categoria, r.RegiaoId, r.ValidoAte, r.Destaque,
            r.FonteOficialUrl, relogio.Hoje);
        repo.Adicionar(c);
        await repo.SalvarAsync(ct);
        return c.Id;
    }

    public async Task AtualizarAsync(Guid id, SalvarComunicadoRequest r, CancellationToken ct)
    {
        var c = await CarregarAsync(id, ct);
        await GarantirRegiaoAsync(r.RegiaoId, ct);
        c.Atualizar(r.Titulo, r.Resumo, r.Conteudo, r.Categoria, r.RegiaoId, r.ValidoAte, r.Destaque, r.FonteOficialUrl, relogio.Hoje);
        await repo.SalvarAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        repo.Remover(await CarregarAsync(id, ct));
        await repo.SalvarAsync(ct);
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
