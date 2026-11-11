using Pup.Domain.Comum;

namespace Pup.Contracts;

/// <param name="Situacao">Situação efetiva hoje (alterações vencidas já aparecem como Regular).</param>
public sealed record ColetaDto(
    Guid Id,
    Guid RegiaoId,
    string RegiaoNome,
    TipoRegiao RegiaoTipo,
    ModalidadeColeta Modalidade,
    DiasSemana Dias,
    TimeOnly HorarioInicio,
    TimeOnly HorarioFim,
    string? Observacao,
    SituacaoColeta Situacao,
    string? AvisoAlteracao,
    DateOnly? AlteracaoAte,
    string? FonteOficialUrl,
    DateTime AtualizadoEm);

public sealed record SalvarColetaRequest(
    Guid RegiaoId,
    ModalidadeColeta Modalidade,
    DiasSemana Dias,
    TimeOnly HorarioInicio,
    TimeOnly HorarioFim,
    string? Observacao,
    string? FonteOficialUrl);

public sealed record AlterarColetaRequest(SituacaoColeta Situacao, string Aviso, DateOnly? Ate);

public sealed record PontoDescarteDto(
    Guid Id,
    string Nome,
    TipoPontoDescarte Tipo,
    Guid RegiaoId,
    string RegiaoNome,
    string Referencia,
    double? Latitude,
    double? Longitude,
    string? Orientacoes,
    bool Ativo,
    string? FonteOficialUrl,
    DateTime AtualizadoEm);

public sealed record SalvarPontoDescarteRequest(
    string Nome,
    TipoPontoDescarte Tipo,
    Guid RegiaoId,
    string Referencia,
    double? Latitude,
    double? Longitude,
    string? Orientacoes,
    string? FonteOficialUrl,
    bool Ativo = true);
