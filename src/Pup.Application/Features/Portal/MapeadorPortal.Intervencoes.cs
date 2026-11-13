using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Portal;

public static partial class MapeadorPortal
{
    public static string Situacao(StatusIntervencao s) => s switch
    {
        StatusIntervencao.Programado => PortalTextos.Programado,
        StatusIntervencao.EmAndamento => PortalTextos.EmAndamento,
        StatusIntervencao.Concluido => PortalTextos.Concluido,
        StatusIntervencao.TemporariamenteAlterado => PortalTextos.Alterado,
        _ => PortalTextos.Cancelado
    };

    public static PortalServicoDto DeIntervencao(IntervencaoResumoDto i, DateOnly atualizadoEm)
    {
        var titulo = SepararExemplo(i.Titulo);
        var descricao = SepararExemplo(i.Descricao);
        var local = SepararExemplo(i.Local);
        var texto = descricao.Texto;
        if (local.Texto.Length > 0) texto += $" Local: {local.Texto}.";
        if (!string.IsNullOrWhiteSpace(i.ImpactoNaVia)) texto += $" Impacto na via: {i.ImpactoNaVia.Trim()}.";

        return new PortalServicoDto(
            i.Id.ToString(),
            titulo.Texto,
            i.Tipo == TipoIntervencao.Obra ? PortalTextos.CategoriaObras : PortalTextos.CategoriaManutencao,
            texto.Trim(),
            i.RegiaoNome,
            Situacao(i.Status),
            atualizadoEm,
            Programacao(i),
            null,
            titulo.Exemplo || descricao.Exemplo || local.Exemplo);
    }

    private static string Programacao(IntervencaoResumoDto i)
    {
        if (i.Status == StatusIntervencao.Concluido && i.DataConclusao is { } fim) return $"Concluído em {Data(fim)}";
        if (i.Status == StatusIntervencao.Cancelado) return PortalTextos.Cancelado;
        if (i.PrevisaoConclusao is { } previsao) return $"Previsão: {Data(previsao)}" + (i.Atrasada ? " (vencida)" : string.Empty);
        return "Previsão não informada";
    }
}
