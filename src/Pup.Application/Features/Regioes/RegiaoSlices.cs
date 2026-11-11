using Pup.Application.Abstracoes;
using Pup.Contracts;

namespace Pup.Application.Features.Regioes;

public static class ListarRegioes
{
    public sealed record Query(bool IncluirInativas = false) : IQuery<IReadOnlyList<RegiaoDto>>;

    internal sealed class Handler(RegiaoService service) : IQueryHandler<Query, IReadOnlyList<RegiaoDto>>
    {
        public Task<IReadOnlyList<RegiaoDto>> HandleAsync(Query q, CancellationToken ct) => service.ListarAsync(q.IncluirInativas, ct);
    }
}

public static class ObterRegiao
{
    public sealed record Query(Guid Id) : IQuery<RegiaoDto>;

    internal sealed class Handler(RegiaoService service) : IQueryHandler<Query, RegiaoDto>
    {
        public Task<RegiaoDto> HandleAsync(Query q, CancellationToken ct) => service.ObterAsync(q.Id, ct);
    }
}

public static class CriarRegiao
{
    public sealed record Command(SalvarRegiaoRequest Dados) : ICommand<Guid>;

    internal sealed class Handler(RegiaoService service) : ICommandHandler<Command, Guid>
    {
        public Task<Guid> HandleAsync(Command c, CancellationToken ct) => service.CriarAsync(c.Dados, ct);
    }
}

public static class AtualizarRegiao
{
    public sealed record Command(Guid Id, SalvarRegiaoRequest Dados) : ICommand<Nada>;

    internal sealed class Handler(RegiaoService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.AtualizarAsync(c.Id, c.Dados, ct);
            return Nada.Valor;
        }
    }
}
