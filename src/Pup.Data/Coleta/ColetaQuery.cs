using Microsoft.EntityFrameworkCore;
using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Data.Coleta;

public sealed class ColetaQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<ColetaResiduos>> ListarAsync(Guid? regiaoId, ModalidadeColeta? modalidade,
        DiasSemana? dia, CancellationToken ct = default)
    {
        var q = db.Coletas.AsNoTracking();
        if (regiaoId is { } r) q = q.Where(c => c.RegiaoId == r);
        if (modalidade is { } m) q = q.Where(c => c.Modalidade == m);
        if (dia is { } d && d != DiasSemana.Nenhum)
        {
            var bits = (int)d;
            q = q.Where(c => ((int)c.Dias & bits) != 0);
        }

        return await q.OrderBy(c => c.Modalidade).ThenBy(c => c.HorarioInicio).ToListAsync(ct);
    }

    public Task<ColetaResiduos?> ObterAsync(Guid id, CancellationToken ct = default) =>
        db.Coletas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
}
