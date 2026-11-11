using Pup.Domain.Comum;

namespace Pup.Contracts;

/// <summary>Item genérico usado em "Últimas atualizações" e nos resultados da busca.</summary>
public sealed record ItemAtualizacaoDto(
    CategoriaServico Categoria,
    Guid Id,
    string Titulo,
    string? Descricao,
    string? Situacao,
    DateTime AtualizadoEm);

public sealed record PainelDto(
    IReadOnlyList<ItemAtualizacaoDto> UltimasAtualizacoes,
    IReadOnlyList<ComunicadoDto> Destaques,
    int ColetasAlteradas,
    int ObrasEmAndamento,
    int ManutencoesAbertas,
    int ComunicadosVigentes);
