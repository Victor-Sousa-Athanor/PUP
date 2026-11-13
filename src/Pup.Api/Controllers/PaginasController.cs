using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Features.Paginas;
using Pup.Contracts;

namespace Pup.Api.Controllers;

[ApiController]
[Route("api/paginas")]
public sealed class PaginasController(PaginaService service) : ControllerBase
{
    [HttpGet("menu")]
    public Task<IReadOnlyList<MenuLinkDto>> ListarMenu(CancellationToken ct) =>
        service.ListarMenuAsync(ct);

    [HttpGet("por-rota")]
    public Task<PaginaDto> ObterPorRota([FromQuery] string rota, CancellationToken ct) =>
        service.ObterPorRotaAsync(rota, ct);

    [Authorize]
    [HttpGet]
    public Task<IReadOnlyList<PaginaDto>> Listar([FromQuery] bool apenasPublicadas = false, CancellationToken ct = default) =>
        service.ListarAsync(apenasPublicadas, ct);

    [Authorize]
    [HttpGet("{id:guid}")]
    public Task<PaginaDto> ObterPorId(Guid id, CancellationToken ct) =>
        service.ObterPorIdAsync(id, ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PaginaDto>> Criar(CriarPaginaRequest req, CancellationToken ct)
    {
        var criada = await service.CriarAsync(req, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public Task<PaginaDto> Atualizar(Guid id, AtualizarPaginaRequest req, CancellationToken ct) =>
        service.AtualizarAsync(id, req, ct);

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await service.RemoverAsync(id, ct);
        return NoContent();
    }
}
