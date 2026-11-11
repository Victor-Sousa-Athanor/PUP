namespace Pup.Domain.Comum;

/// <summary>
/// Base de tudo que é publicado no portal. O relatório exige que todo registro mostre
/// "data de atualização" para o cidadão não confundir informação antiga com atual.
/// </summary>
public abstract class Entidade
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public DateTime AtualizadoEm { get; protected set; } = DateTime.UtcNow;

    /// <summary>Marca o registro como atualizado agora (chamado por todo método que altera dados).</summary>
    protected void Tocar() => AtualizadoEm = DateTime.UtcNow;
}
