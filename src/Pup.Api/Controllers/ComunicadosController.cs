using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Comunicados;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Controllers;

[ApiController]
[Route("api/comunicados")]
public sealed class ComunicadosController(IDispatcher dispatcher) : ControllerBase
{
    /// <param name="regiaoId">Inclui sempre os comunicados do distrito inteiro.</param>
    /// <param name="vigentes">false = inclui os vencidos (histórico).</param>
    [HttpGet]
    public Task<IReadOnlyList<ComunicadoDto>> Listar([FromQuery] CategoriaComunicado? categoria, [FromQuery] Guid? regiaoId,
        [FromQuery] bool vigentes = true, [FromQuery] bool destaques = false, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListarComunicados.Query(categoria, regiaoId, vigentes, destaques), ct);

    [HttpGet("{id:guid}")]
    public Task<ComunicadoDto> Obter(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new ObterComunicado.Query(id), ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(SalvarComunicadoRequest r, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(new CriarComunicado.Command(r), ct);
        return Created($"/api/comunicados/{id}", new IdResponse(id));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, SalvarComunicadoRequest r, CancellationToken ct)
    {
        await dispatcher.SendAsync(new AtualizarComunicado.Command(id, r), ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new RemoverComunicado.Command(id), ct);
        return NoContent();
    }
}
