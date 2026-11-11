using Pup.Domain.Coleta;
using Pup.Domain.Comum;

namespace Pup.Tests;

public class ColetaTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);
    private static readonly Guid Regiao = Guid.CreateVersion7();

    private static ColetaResiduos Nova() => ColetaResiduos.Criar(Regiao, ModalidadeColeta.Comum,
        DiasSemana.Terca | DiasSemana.Quinta, new TimeOnly(7, 0), new TimeOnly(15, 20), null, null);

    [Fact]
    public void Exige_pelo_menos_um_dia()
    {
        Assert.Throws<DomainException>(() => ColetaResiduos.Criar(Regiao, ModalidadeColeta.Comum, DiasSemana.Nenhum,
            new TimeOnly(7, 0), new TimeOnly(8, 0), null, null));
    }

    [Fact]
    public void Horario_final_precisa_ser_depois_do_inicial()
    {
        Assert.Throws<DomainException>(() => ColetaResiduos.Criar(Regiao, ModalidadeColeta.Comum, DiasSemana.Terca,
            new TimeOnly(10, 0), new TimeOnly(9, 0), null, null));
    }

    [Fact]
    public void Alteracao_vencida_volta_a_aparecer_como_regular()
    {
        var c = Nova();
        c.AlterarTemporariamente(SituacaoColeta.TemporariamenteAlterada, "Feriado", Hoje.AddDays(2), Hoje);

        Assert.Equal(SituacaoColeta.TemporariamenteAlterada, c.SituacaoEm(Hoje));
        Assert.Equal(SituacaoColeta.TemporariamenteAlterada, c.SituacaoEm(Hoje.AddDays(2)));
        Assert.Equal(SituacaoColeta.Regular, c.SituacaoEm(Hoje.AddDays(3)));
    }

    [Fact]
    public void Normalizar_limpa_o_aviso()
    {
        var c = Nova();
        c.AlterarTemporariamente(SituacaoColeta.Suspensa, "Chuva forte", null, Hoje);
        c.Normalizar();

        Assert.Equal(SituacaoColeta.Regular, c.Situacao);
        Assert.Null(c.AvisoAlteracao);
        Assert.Throws<DomainException>(() => c.Normalizar());
    }

    [Fact]
    public void Fonte_oficial_precisa_ser_link_http()
    {
        Assert.Throws<DomainException>(() => ColetaResiduos.Criar(Regiao, ModalidadeColeta.Comum, DiasSemana.Terca,
            new TimeOnly(7, 0), new TimeOnly(8, 0), null, "ftp://exemplo"));
    }
}
