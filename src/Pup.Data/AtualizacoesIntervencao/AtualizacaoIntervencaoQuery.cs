using Microsoft.EntityFrameworkCore;
using Pup.Contracts;

namespace Pup.Data.AtualizacoesIntervencao;

public sealed class AtualizacaoIntervencaoQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<AtualizacaoIntervencaoDto>> ListarPorIntervencaoAsync(Guid intervencaoId,
        CancellationToken ct = default) =>
        await db.AtualizacoesIntervencao.AsNoTracking()
            .Where(a => a.IntervencaoId == intervencaoId)
            .OrderByDescending(a => a.Data)
            .Select(a => new AtualizacaoIntervencaoDto(a.Id, a.Status, a.Nota, a.Data))
            .ToListAsync(ct);
}
