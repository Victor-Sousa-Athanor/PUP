using Pup.Domain.Comum;

namespace Pup.Domain.Regioes;

/// <summary>
/// Região, bairro ou estrada de São Francisco Xavier. O distrito tem área urbana pequena e
/// grande extensão rural, então quase toda informação do portal é filtrada por região.
/// </summary>
public sealed class Regiao : Entidade
{
    public string Nome { get; private set; } = string.Empty;
    public TipoRegiao Tipo { get; private set; }
    public string? Descricao { get; private set; }
    public bool Ativa { get; private set; } = true;

    private Regiao() { }

    public static Regiao Criar(string nome, TipoRegiao tipo, string? descricao)
    {
        var r = new Regiao();
        r.Preencher(nome, tipo, descricao);
        return r;
    }

    public void Atualizar(string nome, TipoRegiao tipo, string? descricao, bool ativa)
    {
        Preencher(nome, tipo, descricao);
        Ativa = ativa;
        Tocar();
    }

    private void Preencher(string nome, TipoRegiao tipo, string? descricao)
    {
        Exigir.Definido(tipo, "tipo de região");
        Nome = Exigir.Texto(nome, "o nome da região", 120);
        Tipo = tipo;
        Descricao = Exigir.TextoOpcional(descricao, "a descrição", 500);
    }
}
