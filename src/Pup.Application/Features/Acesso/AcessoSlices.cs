using Pup.Application.Abstracoes;

namespace Pup.Application.Features.Acesso;

public static class Entrar
{
    public sealed record Command(string Email, string Senha) : ICommand<AdministradorAutenticado>;

    internal sealed class Handler(AdministradorService service) : ICommandHandler<Command, AdministradorAutenticado>
    {
        public Task<AdministradorAutenticado> HandleAsync(Command c, CancellationToken ct) =>
            service.AutenticarAsync(c.Email, c.Senha, ct);
    }
}
