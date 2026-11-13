using Microsoft.AspNetCore.Mvc;
using Pup.Application.Features.Portal;
using Pup.Contracts;

namespace Pup.Api.Controllers;

/// <summary>Rotas públicas no formato usado pelo front estático.</summary>
[ApiController]
[Route("api/portal")]
public sealed class PortalController(PortalService service) : ControllerBase
{
    [HttpGet("servicos")]
    public Task<IReadOnlyList<PortalServicoDto>> Servicos(CancellationToken ct = default) =>
        service.ServicosAsync(ct);

    [HttpGet("comunicados")]
    public Task<IReadOnlyList<PortalComunicadoDto>> Comunicados(CancellationToken ct = default) =>
        service.ComunicadosAsync(ct);
}
