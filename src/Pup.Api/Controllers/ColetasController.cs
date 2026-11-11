using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Coleta;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Api.Controllers;

/// <summary>Dias e horários de coleta (comum e seletiva) por região, com alterações temporárias.</summary>
[ApiController]
[Route("api/coletas")]
public sealed class ColetasController(IDispatcher dispatcher) : ControllerBase
{
    /// <param name="dia">Ex.: Terca (só coletas que passam nesse dia).</param>
    /// <param name="hoje">true = só as coletas que passam hoje.</param>
    [HttpGet]
    public Task<IReadOnlyList<ColetaDto>> Listar([FromQuery] Guid? regiaoId, [FromQuery] ModalidadeColeta? modalidade,
        [FromQuery] DiasSemana? dia, [FromQuery] bool hoje = false, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListarColetas.Query(regiaoId, modalidade, dia, hoje), ct);

    [HttpGet("{id:guid}")]
    public Task<ColetaDto> Obter(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new ObterColeta.Query(id), ct);

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<IdResponse>> Criar(SalvarColetaRequest r, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(new CriarColeta.Command(r), ct);
        return Created($"/api/coletas/{id}", new IdResponse(id));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, SalvarColetaRequest r, CancellationToken ct)
    {
        await dispatcher.SendAsync(new AtualizarColeta.Command(id, r), ct);
        return NoContent();
    }

    /// <summary>Registra alteração temporária ou suspensão (com aviso aos moradores).</summary>
    [Authorize]
    [HttpPut("{id:guid}/alteracao")]
    public async Task<IActionResult> Alterar(Guid id, AlterarColetaRequest r, CancellationToken ct)
    {
        await dispatcher.SendAsync(new AlterarColeta.Command(id, r), ct);
        return NoContent();
    }

    /// <summary>Encerra a alteração temporária: a coleta volta a ser exibida como regular.</summary>
    [Authorize]
    [HttpDelete("{id:guid}/alteracao")]
    public async Task<IActionResult> Normalizar(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new NormalizarColeta.Command(id), ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new RemoverColeta.Command(id), ct);
        return NoContent();
    }
}
