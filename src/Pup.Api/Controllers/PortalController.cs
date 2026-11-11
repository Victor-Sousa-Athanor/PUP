using Microsoft.AspNetCore.Mvc;
using Pup.Application.Abstracoes;
using Pup.Application.Features.Portal;
using Pup.Contracts;

namespace Pup.Api.Controllers;

/// <summary>Rotas públicas no formato usado pelo front estático.</summary>
[ApiController]
[Route("api/portal")]
public sealed class PortalController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("servicos")]
    public Task<IReadOnlyList<PortalServicoDto>> Servicos(CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListarServicosDoPortal.Query(), ct);

    [HttpGet("comunicados")]
    public Task<IReadOnlyList<PortalComunicadoDto>> Comunicados(CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListarComunicadosDoPortal.Query(), ct);
}
