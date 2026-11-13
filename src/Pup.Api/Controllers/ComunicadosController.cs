using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Features.Comunicados;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Controllers;

[ApiController]
[Route("api/comunicados")]
public sealed class ComunicadosController(ComunicadoService service) : ControllerBase
{
    /// <param name="regiaoId">Inclui sempre os comunicados do distrito inteiro.</param>
    /// <param name="vigentes">false = inclui os vencidos (histórico).</param>
    [HttpGet]
    public Task<IReadOnlyList<ComunicadoDto>> Listar([FromQuery] CategoriaComunicado? categoria, [FromQuery] Guid? regiaoId,
        [FromQuery] bool vigentes = true, [FromQuery] bool destaques = false, CancellationToken ct = default) =>
        service.ListarAsync(categoria, regiaoId, vigentes, destaques, ct);

    [HttpGet("{id:guid}")]
    public Task<ComunicadoDto> Obter(Guid id, CancellationToken ct) =>
        service.ObterAsync(id, ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(SalvarComunicadoRequest r, CancellationToken ct)
    {
        var id = await service.CriarAsync(r, ct);
        return Created($"/api/comunicados/{id}", new IdResponse(id));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, SalvarComunicadoRequest r, CancellationToken ct)
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
