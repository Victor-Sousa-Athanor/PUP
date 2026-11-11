using Microsoft.EntityFrameworkCore;
using Pup.Contracts;

namespace Pup.Data.Regioes;

public sealed class RegiaoQuery(AppDbContext db)
{
    public async Task<IReadOnlyList<RegiaoDto>> ListarAsync(bool incluirInativas, CancellationToken ct = default) =>
        await db.Regioes.AsNoTracking()
            .Where(r => incluirInativas || r.Ativa)
            .OrderBy(r => r.Tipo).ThenBy(r => r.Nome)
            .Select(r => new RegiaoDto(r.Id, r.Nome, r.Tipo, r.Descricao, r.Ativa, r.AtualizadoEm))
            .ToListAsync(ct);

    public Task<RegiaoDto?> ObterAsync(Guid id, CancellationToken ct = default) =>
        db.Regioes.AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RegiaoDto(r.Id, r.Nome, r.Tipo, r.Descricao, r.Ativa, r.AtualizadoEm))
            .FirstOrDefaultAsync(ct);
}
