using Microsoft.AspNetCore.Mvc;
using Pup.Application.Features.Painel;
using Pup.Contracts;

namespace Pup.Api.Controllers;

/// <summary>Página inicial e busca geral.</summary>
[ApiController]
[Route("api")]
public sealed class PainelController(PainelService service) : ControllerBase
{
    [HttpGet("painel")]
    public Task<PainelDto> Inicio([FromQuery] int limite = 8, CancellationToken ct = default) =>
        service.ObterAsync(limite, ct);

    [HttpGet("busca")]
    public Task<IReadOnlyList<ItemAtualizacaoDto>> Busca([FromQuery] string? termo, CancellationToken ct) =>
        service.BuscarAsync(termo, ct);
}
