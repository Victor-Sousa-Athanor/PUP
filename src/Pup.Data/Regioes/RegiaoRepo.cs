using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Regioes;

namespace Pup.Data.Regioes;

public sealed class RegiaoRepo(AppDbContext db) : RepoBase<Regiao>(db)
{
    public Task<Regiao?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        Db.Regioes.FirstOrDefaultAsync(r => r.Id == id, ct);

    public Task<bool> ExisteAsync(Guid id, CancellationToken ct = default) =>
        Db.Regioes.AnyAsync(r => r.Id == id, ct);

    /// <summary>Nome é único (comparação sem diferenciar maiúsculas).</summary>
    public Task<bool> NomeEmUsoAsync(string nome, Guid? ignorarId, CancellationToken ct = default)
    {
        var n = nome.Trim().ToLower();
        return Db.Regioes.AnyAsync(r => r.Nome.ToLower() == n && r.Id != ignorarId, ct);
    }
}
