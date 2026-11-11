using Pup.Domain.Comum;
using Pup.Domain.Intervencoes;

namespace Pup.Tests;

public class IntervencaoTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);

    private static Intervencao Nova(StatusIntervencao status = StatusIntervencao.Programado, DateOnly? previsao = null) =>
        Intervencao.Criar(TipoIntervencao.Obra, "Estrada", "Cascalhamento", Guid.CreateVersion7(), "km 2",
            status, null, previsao, null, null);

    [Fact]
    public void Cadastro_gera_primeira_linha_do_historico()
    {
        var i = Nova();
        Assert.Single(i.Historico);
        Assert.Equal(StatusIntervencao.Programado, i.Historico[0].Status);
    }

    [Fact]
    public void Nao_cadastra_ja_concluida()
    {
        Assert.Throws<DomainException>(() => Nova(StatusIntervencao.Concluido));
    }

    [Fact]
    public void Mudar_status_registra_historico_e_datas()
    {
        var i = Nova();

        i.MudarStatus(StatusIntervencao.EmAndamento, "Iniciada", Hoje);
        Assert.Equal(Hoje, i.DataInicio);

        i.MudarStatus(StatusIntervencao.Concluido, "Finalizada", Hoje.AddDays(5));
        Assert.Equal(Hoje.AddDays(5), i.DataConclusao);
        Assert.Equal(3, i.Historico.Count);
    }

    [Fact]
    public void Nota_e_obrigatoria()
    {
        Assert.Throws<DomainException>(() => Nova().MudarStatus(StatusIntervencao.EmAndamento, " ", Hoje));
    }

    [Fact]
    public void Registro_encerrado_nao_repete_status()
    {
        var i = Nova();
        i.MudarStatus(StatusIntervencao.Concluido, "Finalizada", Hoje);
        Assert.Throws<DomainException>(() => i.MudarStatus(StatusIntervencao.Concluido, "De novo", Hoje));
    }

    [Fact]
    public void Previsao_vencida_marca_atraso_so_enquanto_aberta()
    {
        var i = Nova(StatusIntervencao.EmAndamento, Hoje.AddDays(-1));
        Assert.True(i.Atrasada(Hoje));

        i.MudarStatus(StatusIntervencao.Concluido, "Finalizada", Hoje);
        Assert.False(i.Atrasada(Hoje));
    }
}
