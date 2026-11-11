using Pup.Application.Abstracoes;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Comunicados;

public static class ListarComunicados
{
    public sealed record Query(
        CategoriaComunicado? Categoria = null,
        Guid? RegiaoId = null,
        bool SomenteVigentes = true,
        bool SomenteDestaques = false) : IQuery<IReadOnlyList<ComunicadoDto>>;

    internal sealed class Handler(ComunicadoService service) : IQueryHandler<Query, IReadOnlyList<ComunicadoDto>>
    {
        public Task<IReadOnlyList<ComunicadoDto>> HandleAsync(Query q, CancellationToken ct) =>
            service.ListarAsync(q.Categoria, q.RegiaoId, q.SomenteVigentes, q.SomenteDestaques, ct);
    }
}

public static class ObterComunicado
{
    public sealed record Query(Guid Id) : IQuery<ComunicadoDto>;

    internal sealed class Handler(ComunicadoService service) : IQueryHandler<Query, ComunicadoDto>
    {
        public Task<ComunicadoDto> HandleAsync(Query q, CancellationToken ct) => service.ObterAsync(q.Id, ct);
    }
}

public static class CriarComunicado
{
    public sealed record Command(SalvarComunicadoRequest Dados) : ICommand<Guid>;

    internal sealed class Handler(ComunicadoService service) : ICommandHandler<Command, Guid>
    {
        public Task<Guid> HandleAsync(Command c, CancellationToken ct) => service.CriarAsync(c.Dados, ct);
    }
}

public static class AtualizarComunicado
{
    public sealed record Command(Guid Id, SalvarComunicadoRequest Dados) : ICommand<Nada>;

    internal sealed class Handler(ComunicadoService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.AtualizarAsync(c.Id, c.Dados, ct);
            return Nada.Valor;
        }
    }
}

public static class RemoverComunicado
{
    public sealed record Command(Guid Id) : ICommand<Nada>;

    internal sealed class Handler(ComunicadoService service) : ICommandHandler<Command, Nada>
    {
        public async Task<Nada> HandleAsync(Command c, CancellationToken ct)
        {
            await service.RemoverAsync(c.Id, ct);
            return Nada.Valor;
        }
    }
}
