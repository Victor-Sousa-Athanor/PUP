using Pup.Application.Abstracoes;
using Pup.Contracts;

namespace Pup.Application.Features.Painel;

public static class ObterPainel
{
    public sealed record Query(int Limite = 8) : IQuery<PainelDto>;

    internal sealed class Handler(PainelService service) : IQueryHandler<Query, PainelDto>
    {
        public Task<PainelDto> HandleAsync(Query q, CancellationToken ct) => service.ObterAsync(q.Limite, ct);
    }
}

public static class Buscar
{
    public sealed record Query(string? Termo) : IQuery<IReadOnlyList<ItemAtualizacaoDto>>;

    internal sealed class Handler(PainelService service) : IQueryHandler<Query, IReadOnlyList<ItemAtualizacaoDto>>
    {
        public Task<IReadOnlyList<ItemAtualizacaoDto>> HandleAsync(Query q, CancellationToken ct) => service.BuscarAsync(q.Termo, ct);
    }
}
