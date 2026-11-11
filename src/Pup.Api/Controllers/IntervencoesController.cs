using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Intervencoes;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Controllers;

/// <summary>Obras e manutenções. Atalhos de leitura: GET api/obras e GET api/manutencoes.</summary>
[ApiController]
[Route("api/intervencoes")]
public sealed class IntervencoesController(IDispatcher dispatcher) : ControllerBase
{
    /// <param name="abertas">true = esconde concluídas e canceladas.</param>
    [HttpGet]
    public Task<IReadOnlyList<IntervencaoResumoDto>> Listar([FromQuery] TipoIntervencao? tipo, [FromQuery] StatusIntervencao? status,
        [FromQuery] Guid? regiaoId, [FromQuery] string? termo, [FromQuery] bool abertas = false, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListarIntervencoes.Query(new FiltroIntervencoes(tipo, status, regiaoId, termo, abertas)), ct);

    [HttpGet("/api/obras")]
    public Task<IReadOnlyList<IntervencaoResumoDto>> Obras([FromQuery] StatusIntervencao? status, [FromQuery] Guid? regiaoId,
        [FromQuery] string? termo, [FromQuery] bool abertas = false, CancellationToken ct = default) =>
        Listar(TipoIntervencao.Obra, status, regiaoId, termo, abertas, ct);

    [HttpGet("/api/manutencoes")]
    public Task<IReadOnlyList<IntervencaoResumoDto>> Manutencoes([FromQuery] StatusIntervencao? status, [FromQuery] Guid? regiaoId,
        [FromQuery] string? termo, [FromQuery] bool abertas = false, CancellationToken ct = default) =>
        Listar(TipoIntervencao.Manutencao, status, regiaoId, termo, abertas, ct);

    /// <summary>Detalhe com o histórico de atualizações.</summary>
    [HttpGet("{id:guid}")]
    public Task<IntervencaoDetalheDto> Obter(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new ObterIntervencao.Query(id), ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(CriarIntervencaoRequest r, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(new CriarIntervencao.Command(r), ct);
        return Created($"/api/intervencoes/{id}", new IdResponse(id));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, AtualizarIntervencaoRequest r, CancellationToken ct)
    {
        await dispatcher.SendAsync(new AtualizarIntervencao.Command(id, r), ct);
        return NoContent();
    }

    /// <summary>Muda a situação e grava uma linha no histórico (a nota é obrigatória).</summary>
    [Authorize]
    [HttpPost("{id:guid}/status")]
    public async Task<IdResponse> MudarStatus(Guid id, MudarStatusRequest r, CancellationToken ct) =>
        new(await dispatcher.SendAsync(new MudarStatusIntervencao.Command(id, r), ct));

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new RemoverIntervencao.Command(id), ct);
        return NoContent();
    }
}
