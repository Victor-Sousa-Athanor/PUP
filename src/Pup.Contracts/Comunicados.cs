using Pup.Domain.Comum;

namespace Pup.Contracts;

public sealed record ComunicadoDto(
    Guid Id,
    string Titulo,
    string Resumo,
    string Conteudo,
    CategoriaComunicado Categoria,
    Guid? RegiaoId,
    string? RegiaoNome,
    DateTime PublicadoEm,
    DateOnly? ValidoAte,
    bool Destaque,
    bool Vigente,
    string? FonteOficialUrl,
    DateTime AtualizadoEm);

public sealed record SalvarComunicadoRequest(
    string Titulo,
    string Resumo,
    string Conteudo,
    CategoriaComunicado Categoria,
    Guid? RegiaoId,
    DateOnly? ValidoAte,
    bool Destaque,
    string? FonteOficialUrl);
