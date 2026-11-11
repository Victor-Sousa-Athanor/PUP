using Pup.Application.Abstracoes;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Coleta;

public static class ListarColetas
{
    public sealed record Query(Guid? RegiaoId = null, ModalidadeColeta? Modalidade = null, DiasSemana? Dia = null, bool SomenteHoje = false)
        : IQuery<IReadOnlyList<ColetaDto>>;

    internal sealed class Handler(ColetaService service) : IQueryHandler<Query, IReadOnlyList<ColetaDto>>
    {
        public Task<IReadOnlyList<ColetaDto>> HandleAsync(Query q, CancellationToken ct) =>
            service.ListarAsync(q.RegiaoId, q.Modalidade, q.Dia, q.SomenteHoje, ct);
    }
}

public static class ObterColeta
{
    public sealed record Query(Guid Id) : IQuery<ColetaDto>;

    internal sealed class Handler(ColetaService service) : IQueryHandler<Query, ColetaDto>
    {
        public Task<ColetaDto> HandleAsync(Query q, CancellationToken ct) => service.ObterAsync(q.Id, ct);
    }
}

public static class CriarColeta
{
    public sealed record Command(SalvarColetaRequest Dados) : ICommand<Guid>;

    internal sealed class Handler(ColetaService service) : ICommandHandler<Command, Guid>
    {
        public Task<Guid> HandleAsync(Command c, CancellationToken ct) => service.CriarAsync(c.Dados, ct);
    }
}

public static class AtualizarColeta
{
    public sealed record Command(Guid Id, SalvarColetaRequest Dados) : ICommand<Nada>;

    internal sealed class Handler(ColetaService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.AtualizarAsync(c.Id, c.Dados, ct);
            return Nada.Valor;
        }
    }
}

/// <summary>Registra uma alteração temporária (ou suspensão) no cronograma com aviso aos moradores.</summary>
public static class AlterarColeta
{
    public sealed record Command(Guid Id, AlterarColetaRequest Dados) : ICommand<Nada>;

    internal sealed class Handler(ColetaService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.AlterarTemporariamenteAsync(c.Id, c.Dados, ct);
            return Nada.Valor;
        }
    }
}

public static class NormalizarColeta
{
    public sealed record Command(Guid Id) : ICommand<Nada>;

    internal sealed class Handler(ColetaService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.NormalizarAsync(c.Id, ct);
            return Nada.Valor;
        }
    }
}

public static class RemoverColeta
{
    public sealed record Command(Guid Id) : ICommand<Nada>;

    internal sealed class Handler(ColetaService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.RemoverAsync(c.Id, ct);
            return Nada.Valor;
        }
    }
}
