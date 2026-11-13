using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Data.Paginas;
using Pup.Domain.Comum;
using Pup.Domain.Paginas;

namespace Pup.Application.Features.Paginas;

public sealed class PaginaService(PaginaRepo repo, PaginaQuery query)
{
    public async Task<IReadOnlyList<PaginaDto>> ListarAsync(bool apenasPublicadas, CancellationToken ct) =>
        (await query.ListarAsync(apenasPublicadas, ct)).Select(ParaDto).ToList();

    public async Task<IReadOnlyList<MenuLinkDto>> ListarMenuAsync(CancellationToken ct) =>
        (await query.ListarMenuAsync(ct))
            .Select(p => new MenuLinkDto(p.Titulo, p.Rota, p.Ordem, p.EhSistema))
            .ToList();

    public async Task<PaginaDto> ObterPorIdAsync(Guid id, CancellationToken ct) =>
        ParaDto(await query.ObterPorIdAsync(id, ct) ?? throw new NaoEncontradoException("Página não encontrada."));

    public async Task<PaginaDto> ObterPorRotaAsync(string rota, CancellationToken ct)
    {
        var normalizada = Pagina.NormalizarRota(rota);
        var pagina = await query.ObterPorRotaAsync(normalizada, ct);
        if (pagina is null || !pagina.Publicada)
            throw new NaoEncontradoException("Página não encontrada ou não publicada.");
        return ParaDto(pagina);
    }

    public async Task<PaginaDto> CriarAsync(CriarPaginaRequest req, CancellationToken ct)
    {
        var pagina = Pagina.Criar(
            req.Titulo, req.Rota, req.Resumo, req.Conteudo,
            req.Ordem, req.ExibirNoMenu, req.Publicada, ehSistema: false, req.FonteOficialUrl);

        if (await query.RotaEmUsoAsync(pagina.Rota, null, ct))
            throw new DomainException($"Já existe uma página cadastrada na rota '/{pagina.Rota}'.");

        repo.Adicionar(pagina);
        await repo.SalvarAsync(ct);
        return ParaDto(pagina);
    }

    public async Task<PaginaDto> AtualizarAsync(Guid id, AtualizarPaginaRequest req, CancellationToken ct)
    {
        var pagina = await repo.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException("Página não encontrada.");

        var rotaNormalizada = Pagina.NormalizarRota(req.Rota);
        if (await query.RotaEmUsoAsync(rotaNormalizada, id, ct))
            throw new DomainException($"Já existe outra página cadastrada na rota '/{rotaNormalizada}'.");

        pagina.Atualizar(
            req.Titulo, req.Rota, req.Resumo, req.Conteudo,
            req.Ordem, req.ExibirNoMenu, req.Publicada, req.FonteOficialUrl);

        await repo.SalvarAsync(ct);
        return ParaDto(pagina);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        var pagina = await repo.ObterPorIdAsync(id, ct)
            ?? throw new NaoEncontradoException("Página não encontrada.");
        if (pagina.EhSistema)
            throw new DomainException("Páginas fixas do sistema não podem ser removidas; desmarque 'Exibir no menu' para ocultá-la.");

        repo.Remover(pagina);
        await repo.SalvarAsync(ct);
    }

    public async Task SemearPadraoAsync(CancellationToken ct)
    {
        if (await query.ExisteAlgumaAsync(ct)) return;

        var padrao = new[]
        {
            Pagina.Criar("Início", "index.html", "Página inicial do portal.", null, 1, true, true, true, null),
            Pagina.Criar("Coleta", "coleta.html", "Cronograma de coleta e pontos de descarte.", null, 2, true, true, true, null),
            Pagina.Criar("Obras", "obras.html", "Acompanhamento de obras no distrito.", null, 3, true, true, true, null),
            Pagina.Criar("Manutenção", "manutencao.html", "Serviços de manutenção urbana e rural.", null, 4, true, true, true, null),
            Pagina.Criar("Comunicados", "comunicados.html", "Avisos e comunicados vigentes.", null, 5, true, true, true, null)
        };
        foreach (var item in padrao) repo.Adicionar(item);
        await repo.SalvarAsync(ct);
    }

    private static PaginaDto ParaDto(Pagina p) =>
        new(p.Id, p.Titulo, p.Rota, p.Resumo, p.Conteudo, p.Ordem, p.ExibirNoMenu, p.Publicada, p.EhSistema, p.FonteOficialUrl, p.AtualizadoEm);
}
