using Pup.Domain.Comum;
using Pup.Domain.Comunicados;

namespace Pup.Tests;

public class ComunicadoTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);

    [Fact]
    public void Vigencia_respeita_data_limite()
    {
        var c = Comunicado.Criar("Aviso", "Resumo", "Texto", CategoriaComunicado.Alerta, null, Hoje.AddDays(3), false, null, Hoje);
        Assert.True(c.Vigente(Hoje.AddDays(3)));
        Assert.False(c.Vigente(Hoje.AddDays(4)));
    }

    [Fact]
    public void Validade_nao_pode_estar_no_passado()
    {
        Assert.Throws<DomainException>(() =>
            Comunicado.Criar("Aviso", "Resumo", "Texto", CategoriaComunicado.Geral, null, Hoje.AddDays(-1), false, null, Hoje));
    }

    [Fact]
    public void Regiao_vazia_vira_distrito_inteiro()
    {
        var c = Comunicado.Criar("Aviso", "Resumo", "Texto", CategoriaComunicado.Geral, Guid.Empty, null, false, null, Hoje);
        Assert.Null(c.RegiaoId);
    }
}
