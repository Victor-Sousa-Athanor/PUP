using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Coleta;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Controllers;

/// <summary>Abrigos ecológicos, lixeiras comunitárias e ecopontos.</summary>
[ApiController]
[Route("api/pontos-descarte")]
public sealed class PontosDescarteController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<PontoDescarteDto>> Listar([FromQuery] Guid? regiaoId, [FromQuery] TipoPontoDescarte? tipo,
        [FromQuery] bool incluirInativos = false, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListarPontosDescarte.Query(regiaoId, tipo, incluirInativos), ct);

    [HttpGet("{id:guid}")]
    public Task<PontoDescarteDto> Obter(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new ObterPontoDescarte.Query(id), ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(SalvarPontoDescarteRequest r, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(new CriarPontoDescarte.Command(r), ct);
        return Created($"/api/pontos-descarte/{id}", new IdResponse(id));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, SalvarPontoDescarteRequest r, CancellationToken ct)
    {
        await dispatcher.SendAsync(new AtualizarPontoDescarte.Command(id, r), ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new RemoverPontoDescarte.Command(id), ct);
        return NoContent();
    }
}
