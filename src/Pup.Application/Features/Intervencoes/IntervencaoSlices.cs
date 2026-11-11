using Pup.Application.Abstracoes;
using Pup.Contracts;

namespace Pup.Application.Features.Intervencoes;

public static class ListarIntervencoes
{
    public sealed record Query(FiltroIntervencoes Filtro) : IQuery<IReadOnlyList<IntervencaoResumoDto>>;

    internal sealed class Handler(IntervencaoService service) : IQueryHandler<Query, IReadOnlyList<IntervencaoResumoDto>>
    {
        public Task<IReadOnlyList<IntervencaoResumoDto>> HandleAsync(Query q, CancellationToken ct) => service.ListarAsync(q.Filtro, ct);
    }
}

public static class ObterIntervencao
{
    public sealed record Query(Guid Id) : IQuery<IntervencaoDetalheDto>;

    internal sealed class Handler(IntervencaoService service) : IQueryHandler<Query, IntervencaoDetalheDto>
    {
        public Task<IntervencaoDetalheDto> HandleAsync(Query q, CancellationToken ct) => service.ObterAsync(q.Id, ct);
    }
}

public static class CriarIntervencao
{
    public sealed record Command(CriarIntervencaoRequest Dados) : ICommand<Guid>;

    internal sealed class Handler(IntervencaoService service) : ICommandHandler<Command, Guid>
    {
        public Task<Guid> HandleAsync(Command c, CancellationToken ct) => service.CriarAsync(c.Dados, ct);
    }
}

public static class AtualizarIntervencao
{
    public sealed record Command(Guid Id, AtualizarIntervencaoRequest Dados) : ICommand<Nada>;

    internal sealed class Handler(IntervencaoService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.AtualizarAsync(c.Id, c.Dados, ct);
            return Nada.Valor;
        }
    }
}

/// <summary>Muda a situação (programado → em andamento → concluído...) e grava no histórico. Retorna o id da atualização.</summary>
public static class MudarStatusIntervencao
{
    public sealed record Command(Guid Id, MudarStatusRequest Dados) : ICommand<Guid>;

    internal sealed class Handler(IntervencaoService service) : ICommandHandler<Command, Guid>
    {
        public Task<Guid> HandleAsync(Command c, CancellationToken ct) => service.MudarStatusAsync(c.Id, c.Dados, ct);
    }
}

public static class RemoverIntervencao
{
    public sealed record Command(Guid Id) : ICommand<Nada>;

    internal sealed class Handler(IntervencaoService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.RemoverAsync(c.Id, ct);
            return Nada.Valor;
        }
    }
}
