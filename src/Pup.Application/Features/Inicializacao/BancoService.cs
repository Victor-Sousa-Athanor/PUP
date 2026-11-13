using Pup.Data;

namespace Pup.Application.Features.Inicializacao;

public sealed class BancoService(BancoRepo repo)
{
    public Task PrepararAsync(CancellationToken ct = default) => repo.PrepararAsync(ct);
}
