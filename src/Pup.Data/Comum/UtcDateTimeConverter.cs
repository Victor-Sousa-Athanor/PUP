using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Pup.Data.Comum;

/// <summary>Postgres timestamptz exige DateTime UTC no Npgsql. Normaliza na escrita e marca como UTC na leitura.</summary>
public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => ParaUtc(v),
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
{
    internal static DateTime ParaUtc(DateTime v) => v.Kind switch
    {
        DateTimeKind.Utc => v,
        DateTimeKind.Local => v.ToUniversalTime(),
        _ => DateTime.SpecifyKind(v, DateTimeKind.Utc)
    };
}
