using Pup.Domain.Comum;

namespace Pup.Domain.Intervencoes;

/// <summary>Linha do histórico de uma obra/manutenção (o "histórico básico" citado no relatório).</summary>
public sealed class AtualizacaoIntervencao
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid IntervencaoId { get; private set; }
    public StatusIntervencao Status { get; private set; }
    public string Nota { get; private set; } = string.Empty;
    public DateTime Data { get; private set; } = DateTime.UtcNow;

    private AtualizacaoIntervencao() { }

    internal AtualizacaoIntervencao(Guid intervencaoId, StatusIntervencao status, string nota)
    {
        IntervencaoId = intervencaoId;
        Status = status;
        Nota = nota;
    }
}
