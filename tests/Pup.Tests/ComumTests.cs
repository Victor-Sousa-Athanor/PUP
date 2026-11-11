using Pup.Application.Comum;
using Pup.Contracts;
using Pup.Domain.Comum;

namespace Pup.Tests;

public class ComumTests
{
    [Fact]
    public void Senha_confere_com_o_hash()
    {
        var hasher = new SenhaHasher();
        var hash = hasher.Gerar("Senha@Forte1");

        Assert.True(hasher.Verificar("Senha@Forte1", hash));
        Assert.False(hasher.Verificar("senha@forte1", hash));
        Assert.DoesNotContain("Senha@Forte1", hash);
    }

    [Fact]
    public void Senha_curta_e_recusada()
    {
        Assert.Throws<DomainException>(() => new SenhaHasher().Gerar("123"));
    }

    [Theory]
    [InlineData(DiasSemana.Terca, "terça")]
    [InlineData(DiasSemana.Terca | DiasSemana.Quinta | DiasSemana.Sabado, "terça, quinta e sábado")]
    [InlineData(DiasSemana.Domingo | DiasSemana.Segunda | DiasSemana.Terca | DiasSemana.Quarta | DiasSemana.Quinta |
                DiasSemana.Sexta | DiasSemana.Sabado, "todos os dias")]
    public void Dias_da_semana_por_extenso(DiasSemana dias, string esperado)
    {
        Assert.Equal(esperado, dias.Nome());
    }

    [Fact]
    public void Dia_do_calendario_vira_flag()
    {
        Assert.Equal(DiasSemana.Sabado, new DateOnly(2026, 11, 7).DayOfWeek.ParaFlag());
    }
}
