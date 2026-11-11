namespace Pup.Application.Comum;

public interface IRelogio
{
    DateTime AgoraUtc { get; }

    /// <summary>Data de hoje em São Francisco Xavier (horário de Brasília), usada em vigência e previsões.</summary>
    DateOnly Hoje { get; }
}

internal sealed class Relogio : IRelogio
{
    private static readonly TimeZoneInfo Fuso = CarregarFuso();

    public DateTime AgoraUtc => DateTime.UtcNow;

    public DateOnly Hoje => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Fuso));

    private static TimeZoneInfo CarregarFuso()
    {
        // IANA funciona no Linux e no Windows (.NET 6+ com ICU); o id do Windows fica de reserva.
        foreach (var id in new[] { "America/Sao_Paulo", "E. South America Standard Time" })
            if (TimeZoneInfo.TryFindSystemTimeZoneById(id, out var fuso)) return fuso;
        return TimeZoneInfo.CreateCustomTimeZone("BRT", TimeSpan.FromHours(-3), "Brasília", "Brasília");
    }
}
