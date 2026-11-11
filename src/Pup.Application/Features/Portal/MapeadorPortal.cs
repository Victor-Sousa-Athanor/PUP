using System.Globalization;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Portal;

/// <summary>
/// Converte os DTOs da API para o formato do front estático (assets/js/data.js).
/// Funções puras: sem banco e sem relógio (a data local chega pronta do PortalService).
/// </summary>
public static class MapeadorPortal
{
    private const string MarcaExemplo = "[Exemplo]";
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    /// <summary>Tira a marca "[Exemplo]" do texto e diz se ela existia (o front mostra o aviso de demonstração).</summary>
    public static (string Texto, bool Exemplo) SepararExemplo(string? texto)
    {
        var t = texto?.Trim() ?? string.Empty;
        return t.StartsWith(MarcaExemplo, StringComparison.OrdinalIgnoreCase)
            ? (t[MarcaExemplo.Length..].Trim(), true)
            : (t, false);
    }

    public static string Situacao(SituacaoColeta s) =>
        s == SituacaoColeta.Regular ? PortalTextos.Programado : PortalTextos.Alterado;

    public static string Situacao(StatusIntervencao s) => s switch
    {
        StatusIntervencao.Programado => PortalTextos.Programado,
        StatusIntervencao.EmAndamento => PortalTextos.EmAndamento,
        StatusIntervencao.Concluido => PortalTextos.Concluido,
        StatusIntervencao.TemporariamenteAlterado => PortalTextos.Alterado,
        _ => PortalTextos.Cancelado
    };

    public static string Categoria(CategoriaComunicado c) => c switch
    {
        CategoriaComunicado.Coleta => "Coleta",
        CategoriaComunicado.Obras => "Obras",
        CategoriaComunicado.Manutencao => "Manutenção",
        CategoriaComunicado.Alerta => "Alerta",
        _ => "Geral"
    };

    public static PortalServicoDto DeColeta(ColetaDto c, DateOnly atualizadoEm)
    {
        var observacao = SepararExemplo(c.Observacao);
        var aviso = SepararExemplo(c.AvisoAlteracao);
        var alterada = c.Situacao != SituacaoColeta.Regular;
        var programacao = $"{Maiuscula(c.Dias.Nome())}, das {Hora(c.HorarioInicio)} às {Hora(c.HorarioFim)}";

        return new PortalServicoDto(
            c.Id.ToString(),
            c.Modalidade == ModalidadeColeta.Comum ? "Coleta comum" : "Coleta seletiva",
            PortalTextos.CategoriaColeta,
            alterada && aviso.Texto.Length > 0 ? aviso.Texto : observacao.Texto,
            c.RegiaoNome,
            Situacao(c.Situacao),
            atualizadoEm,
            programacao,
            c.FonteOficialUrl,
            observacao.Exemplo || aviso.Exemplo);
    }

    /// <summary>Ponto de descarte ativo aparece como serviço de coleta "Em andamento" (disponível).</summary>
    public static PortalServicoDto DePonto(PontoDescarteDto p, DateOnly atualizadoEm)
    {
        var nome = SepararExemplo(p.Nome);
        var referencia = SepararExemplo(p.Referencia);
        var orientacoes = SepararExemplo(p.Orientacoes);
        var descricao = orientacoes.Texto.Length == 0 ? referencia.Texto : $"{referencia.Texto} — {orientacoes.Texto}";

        return new PortalServicoDto(p.Id.ToString(), nome.Texto, PortalTextos.CategoriaColeta, descricao, p.RegiaoNome,
            PortalTextos.EmAndamento, atualizadoEm, p.Tipo.Nome(), p.FonteOficialUrl,
            nome.Exemplo || referencia.Exemplo || orientacoes.Exemplo);
    }

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

    public static PortalComunicadoDto DeComunicado(ComunicadoDto c, DateOnly publicadoEm)
    {
        var titulo = SepararExemplo(c.Titulo);
        var conteudo = SepararExemplo(c.Conteudo);
        return new PortalComunicadoDto(c.Id.ToString(), titulo.Texto, conteudo.Texto, Categoria(c.Categoria), publicadoEm,
            c.FonteOficialUrl, titulo.Exemplo || conteudo.Exemplo);
    }

    private static string Programacao(IntervencaoResumoDto i)
    {
        if (i.Status == StatusIntervencao.Concluido && i.DataConclusao is { } fim) return $"Concluído em {Data(fim)}";
        if (i.Status == StatusIntervencao.Cancelado) return PortalTextos.Cancelado;
        if (i.PrevisaoConclusao is { } previsao) return $"Previsão: {Data(previsao)}" + (i.Atrasada ? " (vencida)" : string.Empty);
        return "Previsão não informada";
    }

    private static string Data(DateOnly d) => d.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    private static string Hora(TimeOnly h) => h.ToString("HH:mm", CultureInfo.InvariantCulture);

    private static string Maiuscula(string t) => t.Length == 0 ? t : char.ToUpper(t[0], PtBr) + t[1..];
}
