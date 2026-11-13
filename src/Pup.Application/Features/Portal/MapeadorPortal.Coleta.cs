using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Portal;

public static partial class MapeadorPortal
{
    public static string Situacao(SituacaoColeta s) =>
        s == SituacaoColeta.Regular ? PortalTextos.Programado : PortalTextos.Alterado;

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
}
