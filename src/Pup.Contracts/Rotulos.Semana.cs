using Pup.Domain.Comum;

namespace Pup.Contracts;

public static partial class Rotulos
{
    private static readonly (DiasSemana Dia, string Curto, string Longo)[] Semana =
    [
        (DiasSemana.Domingo, "Dom", "domingo"),
        (DiasSemana.Segunda, "Seg", "segunda"),
        (DiasSemana.Terca, "Ter", "terça"),
        (DiasSemana.Quarta, "Qua", "quarta"),
        (DiasSemana.Quinta, "Qui", "quinta"),
        (DiasSemana.Sexta, "Sex", "sexta"),
        (DiasSemana.Sabado, "Sáb", "sábado")
    ];

    /// <summary>Ex.: "terça, quinta e sábado".</summary>
    public static string Nome(this DiasSemana dias)
    {
        var nomes = Semana.Where(s => dias.HasFlag(s.Dia) && s.Dia != DiasSemana.Nenhum).Select(s => s.Longo).ToList();
        return nomes.Count switch
        {
            0 => "—",
            1 => nomes[0],
            7 => "todos os dias",
            _ => string.Join(", ", nomes.Take(nomes.Count - 1)) + " e " + nomes[^1]
        };
    }

    public static IReadOnlyList<(DiasSemana Dia, string Curto)> DiasDaSemana { get; } =
        Semana.Select(s => (s.Dia, s.Curto)).ToList();

    /// <summary>Converte o dia do calendário para o flag correspondente (para destacar "coleta hoje").</summary>
    public static DiasSemana ParaFlag(this DayOfWeek dia) => dia switch
    {
        DayOfWeek.Sunday => DiasSemana.Domingo,
        DayOfWeek.Monday => DiasSemana.Segunda,
        DayOfWeek.Tuesday => DiasSemana.Terca,
        DayOfWeek.Wednesday => DiasSemana.Quarta,
        DayOfWeek.Thursday => DiasSemana.Quinta,
        DayOfWeek.Friday => DiasSemana.Sexta,
        _ => DiasSemana.Sabado
    };
}
