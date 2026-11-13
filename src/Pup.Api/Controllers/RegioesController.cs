using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Features.Regioes;
using Pup.Contracts;

namespace Pup.Api.Controllers;

[ApiController]
[Route("api/regioes")]
public sealed class RegioesController(RegiaoService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<RegiaoDto>> Listar([FromQuery] bool incluirInativas = false, CancellationToken ct = default) =>
        service.ListarAsync(incluirInativas, ct);

    [HttpGet("{id:guid}")]
    public Task<RegiaoDto> Obter(Guid id, CancellationToken ct) =>
        service.ObterAsync(id, ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(SalvarRegiaoRequest r, CancellationToken ct)
    {
        var id = await service.CriarAsync(r, ct);
        return Created($"/api/regioes/{id}", new IdResponse(id));
    }

    /// <summary>Para "apagar" uma região, envie Ativa = false (ela pode ter coletas e obras vinculadas).</summary>
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, SalvarRegiaoRequest r, CancellationToken ct)
    {
        await service.AtualizarAsync(id, r, ct);
        return NoContent();
    }
}
