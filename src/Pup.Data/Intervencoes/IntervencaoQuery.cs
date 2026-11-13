using Microsoft.EntityFrameworkCore;
using Pup.Domain.Comum;
using Pup.Domain.Intervencoes;

namespace Pup.Data.Intervencoes;

public sealed class IntervencaoQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<Intervencao>> ListarAsync(TipoIntervencao? tipo, StatusIntervencao? status,
        Guid? regiaoId, CancellationToken ct = default)
    {
        var q = db.Intervencoes.AsNoTracking();
        if (tipo is { } t) q = q.Where(i => i.Tipo == t);
        if (status is { } s) q = q.Where(i => i.Status == s);
        if (regiaoId is { } r) q = q.Where(i => i.RegiaoId == r);
        return await q.OrderByDescending(i => i.AtualizadoEm).ToListAsync(ct);
    }

    public Task<Intervencao?> ObterAsync(Guid id, CancellationToken ct = default) =>
        db.Intervencoes.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, ct);
}
