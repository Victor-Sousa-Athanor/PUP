using Pup.Domain.Comum;

namespace Pup.Contracts;

public sealed record IntervencaoResumoDto(
    Guid Id,
    TipoIntervencao Tipo,
    string Titulo,
    string Descricao,
    Guid RegiaoId,
    string RegiaoNome,
    string? Local,
    StatusIntervencao Status,
    DateOnly? DataInicio,
    DateOnly? PrevisaoConclusao,
    DateOnly? DataConclusao,
    string? ImpactoNaVia,
    bool Atrasada,
    DateTime AtualizadoEm);

public sealed record AtualizacaoIntervencaoDto(Guid Id, StatusIntervencao Status, string Nota, DateTime Data);

public sealed record IntervencaoDetalheDto(
    Guid Id,
    TipoIntervencao Tipo,
    string Titulo,
    string Descricao,
    Guid RegiaoId,
    string RegiaoNome,
    string? Local,
    StatusIntervencao Status,
    DateOnly? DataInicio,
    DateOnly? PrevisaoConclusao,
    DateOnly? DataConclusao,
    string? ImpactoNaVia,
    bool Atrasada,
    string? FonteOficialUrl,
    DateTime CriadoEm,
    DateTime AtualizadoEm,
    IReadOnlyList<AtualizacaoIntervencaoDto> Historico);

public sealed record FiltroIntervencoes(
    TipoIntervencao? Tipo = null,
    StatusIntervencao? Status = null,
    Guid? RegiaoId = null,
    string? Termo = null,
    bool SomenteAbertas = false);

public sealed record CriarIntervencaoRequest(
    TipoIntervencao Tipo,
    string Titulo,
    string Descricao,
    Guid RegiaoId,
    string? Local,
    StatusIntervencao Status,
    DateOnly? DataInicio,
    DateOnly? PrevisaoConclusao,
    string? ImpactoNaVia,
    string? FonteOficialUrl);

public sealed record AtualizarIntervencaoRequest(
    string Titulo,
    string Descricao,
    Guid RegiaoId,
    string? Local,
    DateOnly? DataInicio,
    DateOnly? PrevisaoConclusao,
    string? ImpactoNaVia,
    string? FonteOficialUrl);

public sealed record MudarStatusRequest(StatusIntervencao Status, string Nota);
