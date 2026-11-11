using Microsoft.AspNetCore.Mvc;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Painel;
using Pup.Contracts;

namespace Pup.Api.Controllers;

/// <summary>Página inicial (últimas atualizações, destaques e contadores) e busca geral.</summary>
[ApiController]
[Route("api")]
public sealed class PainelController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("painel")]
    public Task<PainelDto> Inicio([FromQuery] int limite = 8, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ObterPainel.Query(limite), ct);

    [HttpGet("busca")]
    public Task<IReadOnlyList<ItemAtualizacaoDto>> Busca([FromQuery] string? termo, CancellationToken ct) =>
        dispatcher.QueryAsync(new Buscar.Query(termo), ct);
}
