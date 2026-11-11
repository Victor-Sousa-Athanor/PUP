using Pup.Application.Abstracoes;
using Pup.Contracts;

namespace Pup.Application.Features.Portal;

public static class ListarServicosDoPortal
{
    public sealed record Query : IQuery<IReadOnlyList<PortalServicoDto>>;

    internal sealed class Handler(PortalService service) : IQueryHandler<Query, IReadOnlyList<PortalServicoDto>>
    {
        public Task<IReadOnlyList<PortalServicoDto>> HandleAsync(Query q, CancellationToken ct) =>
            service.ServicosAsync(ct);
    }
}

public static class ListarComunicadosDoPortal
{
    public sealed record Query : IQuery<IReadOnlyList<PortalComunicadoDto>>;

    internal sealed class Handler(PortalService service) : IQueryHandler<Query, IReadOnlyList<PortalComunicadoDto>>
    {
        public Task<IReadOnlyList<PortalComunicadoDto>> HandleAsync(Query q, CancellationToken ct) =>
            service.ComunicadosAsync(ct);
    }
}
