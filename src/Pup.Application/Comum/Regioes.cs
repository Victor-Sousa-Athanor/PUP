using Pup.Data.Regioes;
using Pup.Domain.Comum;

namespace Pup.Application.Comum;

internal static class RegiaoRepoExtensions
{
    /// <summary>Usado pelos services antes de vincular um registro a uma região.</summary>
    public static async Task GarantirExisteAsync(this RegiaoRepo repo, Guid regiaoId, CancellationToken ct)
    {
        if (regiaoId == Guid.Empty || !await repo.ExisteAsync(regiaoId, ct))
            throw new DomainException("Região não encontrada. Selecione uma região da lista.");
    }
}
