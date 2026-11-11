using Pup.Application.Features.Portal;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Tests;

public class MapeadorPortalTests
{
    private static readonly DateOnly Dia = new(2026, 11, 10);

    [Fact]
    public void SepararExemplo_ComMarca_RemoveMarcaEMarcaComoExemplo()
    {
        var (texto, exemplo) = MapeadorPortal.SepararExemplo("[Exemplo] Estrada do Varadouro");
        Assert.Equal("Estrada do Varadouro", texto);
        Assert.True(exemplo);
    }

    [Fact]
    public void SepararExemplo_SemMarca_MantemTexto()
    {
        Assert.Equal(("Centro", false), MapeadorPortal.SepararExemplo(" Centro "));
        Assert.Equal((string.Empty, false), MapeadorPortal.SepararExemplo(null));
    }

    [Theory]
    [InlineData(StatusIntervencao.Programado, "Programado")]
    [InlineData(StatusIntervencao.EmAndamento, "Em andamento")]
    [InlineData(StatusIntervencao.Concluido, "Concluído")]
    [InlineData(StatusIntervencao.TemporariamenteAlterado, "Alterado")]
    [InlineData(StatusIntervencao.Cancelado, "Cancelado")]
    public void Situacao_Intervencao_UsaTextoDoFront(StatusIntervencao status, string esperado) =>
        Assert.Equal(esperado, MapeadorPortal.Situacao(status));

    [Theory]
    [InlineData(SituacaoColeta.Regular, "Programado")]
    [InlineData(SituacaoColeta.TemporariamenteAlterada, "Alterado")]
    [InlineData(SituacaoColeta.Suspensa, "Alterado")]
    public void Situacao_Coleta_UsaTextoDoFront(SituacaoColeta situacao, string esperado) =>
        Assert.Equal(esperado, MapeadorPortal.Situacao(situacao));

    [Fact]
    public void DeColeta_Regular_MontaProgramacaoComDiasEHorario()
    {
        var c = new ColetaDto(Guid.NewGuid(), Guid.NewGuid(), "Centro", TipoRegiao.Urbana, ModalidadeColeta.Seletiva,
            DiasSemana.Terca | DiasSemana.Quinta | DiasSemana.Sabado, new TimeOnly(7, 0), new TimeOnly(15, 20),
            "Separe os recicláveis.", SituacaoColeta.Regular, null, null, "https://www.sjc.sp.gov.br/", DateTime.UtcNow);

        var s = MapeadorPortal.DeColeta(c, Dia);

        Assert.Equal(c.Id.ToString(), s.Id);
        Assert.Equal("Coleta seletiva", s.Titulo);
        Assert.Equal("Coleta de resíduos", s.Categoria);
        Assert.Equal("Centro", s.Localidade);
        Assert.Equal("Programado", s.Situacao);
        Assert.Equal("Terça, quinta e sábado, das 07:00 às 15:20", s.Programacao);
        Assert.Equal("Separe os recicláveis.", s.Descricao);
        Assert.Equal(Dia, s.AtualizadoEm);
        Assert.False(s.Demonstracao);
    }

    [Fact]
    public void DeColeta_Alterada_MostraOAvisoNaDescricao()
    {
        var c = new ColetaDto(Guid.NewGuid(), Guid.NewGuid(), "Zona rural", TipoRegiao.Rural, ModalidadeColeta.Comum,
            DiasSemana.Terca, new TimeOnly(8, 0), new TimeOnly(12, 0), "[Exemplo] Coleta nos abrigos.",
            SituacaoColeta.TemporariamenteAlterada, "[Exemplo] Coleta passa para quarta.", Dia.AddDays(3), null, DateTime.UtcNow);

        var s = MapeadorPortal.DeColeta(c, Dia);

        Assert.Equal("Coleta comum", s.Titulo);
        Assert.Equal("Alterado", s.Situacao);
        Assert.Equal("Coleta passa para quarta.", s.Descricao);
        Assert.True(s.Demonstracao);
    }

    [Fact]
    public void DePonto_JuntaReferenciaEOrientacoes()
    {
        var p = new PontoDescarteDto(Guid.NewGuid(), "[Exemplo] Abrigo da capela", TipoPontoDescarte.AbrigoEcologico,
            Guid.NewGuid(), "Zona rural", "Em frente à capela", null, null, "Recicláveis limpos.", true, null, DateTime.UtcNow);

        var s = MapeadorPortal.DePonto(p, Dia);

        Assert.Equal("Abrigo da capela", s.Titulo);
        Assert.Equal("Coleta de resíduos", s.Categoria);
        Assert.Equal("Em andamento", s.Situacao);
        Assert.Equal("Abrigo ecológico", s.Programacao);
        Assert.Equal("Em frente à capela — Recicláveis limpos.", s.Descricao);
        Assert.True(s.Demonstracao);
    }

    [Fact]
    public void DeIntervencao_ComPrevisaoVencida_AvisaNaProgramacao()
    {
        var i = new IntervencaoResumoDto(Guid.NewGuid(), TipoIntervencao.Obra, "[Exemplo] Adequação de via", "Cascalhamento.",
            Guid.NewGuid(), "Estrada do Varadouro", "km 2", StatusIntervencao.EmAndamento, new DateOnly(2026, 10, 1),
            new DateOnly(2026, 11, 5), null, "Meia pista", true, DateTime.UtcNow);

        var s = MapeadorPortal.DeIntervencao(i, Dia);

        Assert.Equal("Adequação de via", s.Titulo);
        Assert.Equal("Obras", s.Categoria);
        Assert.Equal("Em andamento", s.Situacao);
        Assert.Equal("Estrada do Varadouro", s.Localidade);
        Assert.Equal("Previsão: 05/11/2026 (vencida)", s.Programacao);
        Assert.Equal("Cascalhamento. Local: km 2. Impacto na via: Meia pista.", s.Descricao);
        Assert.True(s.Demonstracao);
    }

    [Fact]
    public void DeIntervencao_Concluida_MostraDataDeConclusao()
    {
        var i = new IntervencaoResumoDto(Guid.NewGuid(), TipoIntervencao.Manutencao, "Nivelamento", "Nivelamento da estrada.",
            Guid.NewGuid(), "Zona rural", null, StatusIntervencao.Concluido, null, null, new DateOnly(2026, 11, 3), null, false,
            DateTime.UtcNow);

        var s = MapeadorPortal.DeIntervencao(i, Dia);

        Assert.Equal("Manutenção", s.Categoria);
        Assert.Equal("Concluído", s.Situacao);
        Assert.Equal("Concluído em 03/11/2026", s.Programacao);
        Assert.Equal("Nivelamento da estrada.", s.Descricao);
        Assert.False(s.Demonstracao);
    }

    [Fact]
    public void DeComunicado_Alerta_UsaCategoriaETextoCompleto()
    {
        var c = new ComunicadoDto(Guid.NewGuid(), "[Exemplo] Interdição", "Resumo", "[Exemplo] Meia pista das 8h às 16h.",
            CategoriaComunicado.Alerta, null, null, DateTime.UtcNow, null, false, true, null, DateTime.UtcNow);

        var p = MapeadorPortal.DeComunicado(c, Dia);

        Assert.Equal("Interdição", p.Titulo);
        Assert.Equal("Meia pista das 8h às 16h.", p.Mensagem);
        Assert.Equal("Alerta", p.Categoria);
        Assert.Equal(Dia, p.PublicadoEm);
        Assert.True(p.Demonstracao);
    }

    [Fact]
    public void Json_UsaOsNomesDoFront()
    {
        var p = new PortalComunicadoDto("1", "T", "M", "Geral", Dia, null, false);

        var json = System.Text.Json.JsonSerializer.Serialize(p);

        Assert.Contains("\"publishedAt\":\"2026-11-10\"", json);
        Assert.Contains("\"message\":\"M\"", json);
        Assert.Contains("\"isDemo\":false", json);
    }
}
