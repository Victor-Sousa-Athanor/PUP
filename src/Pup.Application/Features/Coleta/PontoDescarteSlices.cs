using Pup.Application.Abstracoes;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Coleta;

public static class ListarPontosDescarte
{
    public sealed record Query(Guid? RegiaoId = null, TipoPontoDescarte? Tipo = null, bool IncluirInativos = false)
        : IQuery<IReadOnlyList<PontoDescarteDto>>;

    internal sealed class Handler(PontoDescarteService service) : IQueryHandler<Query, IReadOnlyList<PontoDescarteDto>>
    {
        public Task<IReadOnlyList<PontoDescarteDto>> HandleAsync(Query q, CancellationToken ct) =>
            service.ListarAsync(q.RegiaoId, q.Tipo, q.IncluirInativos, ct);
    }
}

public static class ObterPontoDescarte
{
    public sealed record Query(Guid Id) : IQuery<PontoDescarteDto>;

    internal sealed class Handler(PontoDescarteService service) : IQueryHandler<Query, PontoDescarteDto>
    {
        public Task<PontoDescarteDto> HandleAsync(Query q, CancellationToken ct) => service.ObterAsync(q.Id, ct);
    }
}

public static class CriarPontoDescarte
{
    public sealed record Command(SalvarPontoDescarteRequest Dados) : ICommand<Guid>;

    internal sealed class Handler(PontoDescarteService service) : ICommandHandler<Command, Guid>
    {
        public Task<Guid> HandleAsync(Command c, CancellationToken ct) => service.CriarAsync(c.Dados, ct);
    }
}

public static class AtualizarPontoDescarte
{
    public sealed record Command(Guid Id, SalvarPontoDescarteRequest Dados) : ICommand<Nada>;

    internal sealed class Handler(PontoDescarteService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.AtualizarAsync(c.Id, c.Dados, ct);
            return Nada.Valor;
        }
    }
}

public static class RemoverPontoDescarte
{
    public sealed record Command(Guid Id) : ICommand<Nada>;

    internal sealed class Handler(PontoDescarteService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.RemoverAsync(c.Id, ct);
            return Nada.Valor;
        }
    }
}
