using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Features.PontosDescarte;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Controllers;

/// <summary>Abrigos ecológicos, lixeiras comunitárias e ecopontos.</summary>
[ApiController]
[Route("api/pontos-descarte")]
public sealed class PontosDescarteController(PontoDescarteService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<PontoDescarteDto>> Listar([FromQuery] Guid? regiaoId, [FromQuery] TipoPontoDescarte? tipo,
        [FromQuery] bool incluirInativos = false, CancellationToken ct = default) =>
        service.ListarAsync(regiaoId, tipo, incluirInativos, ct);

    [HttpGet("{id:guid}")]
    public Task<PontoDescarteDto> Obter(Guid id, CancellationToken ct) =>
        service.ObterAsync(id, ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(SalvarPontoDescarteRequest r, CancellationToken ct)
    {
        var id = await service.CriarAsync(r, ct);
        return Created($"/api/pontos-descarte/{id}", new IdResponse(id));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, SalvarPontoDescarteRequest r, CancellationToken ct)
    {
        await service.AtualizarAsync(id, r, ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await service.RemoverAsync(id, ct);
        return NoContent();
    }
}
