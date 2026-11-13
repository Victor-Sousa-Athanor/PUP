using Pup.Domain.Comum;

namespace Pup.Contracts;

/// <summary>Textos em português para exibir os enums (usados pela API e pelo front-end).</summary>
public static partial class Rotulos
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
}
