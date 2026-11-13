namespace Pup.Contracts;

public sealed record PaginaDto(
    Guid Id,
    string Titulo,
    string Rota,
    string? Resumo,
    string? Conteudo,
    int Ordem,
    bool ExibirNoMenu,
    bool Publicada,
    bool EhSistema,
    string? FonteOficialUrl,
    DateTime AtualizadoEm);

public sealed record MenuLinkDto(
    string Titulo,
    string Rota,
    int Ordem,
    bool EhSistema);

public sealed record CriarPaginaRequest(
    string Titulo,
    string Rota,
    string? Resumo,
    string? Conteudo,
    int Ordem,
    bool ExibirNoMenu,
    bool Publicada,
    string? FonteOficialUrl);

public sealed record AtualizarPaginaRequest(
    string Titulo,
    string Rota,
    string? Resumo,
    string? Conteudo,
    int Ordem,
    bool ExibirNoMenu,
    bool Publicada,
    string? FonteOficialUrl);
