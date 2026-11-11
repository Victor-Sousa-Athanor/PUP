namespace Pup.Data.Comum;

/// <summary>Base dos repositórios (escrita). O SaveChanges é chamado explicitamente pelo Service.</summary>
public abstract class RepoBase<T>(AppDbContext db) where T : class
{
    protected AppDbContext Db { get; } = db;

    public void Adicionar(T entidade) => Db.Set<T>().Add(entidade);

    public void Remover(T entidade) => Db.Set<T>().Remove(entidade);

    /// <summary>Persiste todas as alterações pendentes do contexto (unit of work do request).</summary>
    public Task<int> SalvarAsync(CancellationToken ct = default) => Db.SaveChangesAsync(ct);
}
