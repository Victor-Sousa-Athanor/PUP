using Pup.Domain.Comum;

namespace Pup.Contracts;

/// <summary>Textos em português para exibir os enums (usados pela API e pelo front-end).</summary>
public static class Rotulos
{
    public static string Nome(this TipoRegiao v) => v switch
    {
        TipoRegiao.Urbana => "Área urbana",
        TipoRegiao.Rural => "Área rural",
        _ => v.ToString()
    };

    public static string Nome(this ModalidadeColeta v) => v switch
    {
        ModalidadeColeta.Comum => "Coleta comum",
        ModalidadeColeta.Seletiva => "Coleta seletiva (recicláveis)",
        _ => v.ToString()
    };

    public static string Nome(this SituacaoColeta v) => v switch
    {
        SituacaoColeta.Regular => "Regular",
        SituacaoColeta.TemporariamenteAlterada => "Temporariamente alterada",
        SituacaoColeta.Suspensa => "Suspensa",
        _ => v.ToString()
    };

    public static string Nome(this TipoPontoDescarte v) => v switch
    {
        TipoPontoDescarte.AbrigoEcologico => "Abrigo ecológico",
        TipoPontoDescarte.LixeiraComunitaria => "Lixeira comunitária",
        TipoPontoDescarte.Ecoponto => "Ecoponto",
        _ => v.ToString()
    };

    public static string Nome(this TipoIntervencao v) => v switch
    {
        TipoIntervencao.Obra => "Obra",
        TipoIntervencao.Manutencao => "Manutenção",
        _ => v.ToString()
    };

    public static string Nome(this StatusIntervencao v) => v switch
    {
        StatusIntervencao.Programado => "Programado",
        StatusIntervencao.EmAndamento => "Em andamento",
        StatusIntervencao.Concluido => "Concluído",
        StatusIntervencao.TemporariamenteAlterado => "Temporariamente alterado",
        StatusIntervencao.Cancelado => "Cancelado",
        _ => v.ToString()
    };

    public static string Nome(this CategoriaComunicado v) => v switch
    {
        CategoriaComunicado.Geral => "Geral",
        CategoriaComunicado.Coleta => "Coleta de lixo",
        CategoriaComunicado.Obras => "Obras",
        CategoriaComunicado.Manutencao => "Manutenção",
        CategoriaComunicado.Alerta => "Alerta",
        _ => v.ToString()
    };

    public static string Nome(this CategoriaServico v) => v switch
    {
        CategoriaServico.Coleta => "Coleta de lixo",
        CategoriaServico.PontoDescarte => "Ponto de descarte",
        CategoriaServico.Obra => "Obra",
        CategoriaServico.Manutencao => "Manutenção",
        CategoriaServico.Comunicado => "Comunicado",
        _ => v.ToString()
    };

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
