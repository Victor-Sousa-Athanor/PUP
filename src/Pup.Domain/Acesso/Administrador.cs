using Pup.Domain.Comum;

namespace Pup.Domain.Acesso;

/// <summary>
/// Responsável por atualizar o portal manualmente (relatório, seção 2.3.4 E: "área administrativa"
/// enquanto não existir integração automática com a Prefeitura).
/// </summary>
public sealed class Administrador : Entidade
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public bool Ativo { get; private set; } = true;
    public DateTime? UltimoAcessoEm { get; private set; }

    private Administrador() { }

    public static Administrador Criar(string nome, string email, string senhaHash) => new()
    {
        Nome = Exigir.Texto(nome, "o nome", 120),
        Email = NormalizarEmail(email),
        SenhaHash = Exigir.Texto(senhaHash, "a senha", 200)
    };

    public void RegistrarAcesso() => UltimoAcessoEm = DateTime.UtcNow;

    public static string NormalizarEmail(string? email)
    {
        var e = Exigir.Texto(email, "o e-mail", 200).ToLowerInvariant();
        if (!e.Contains('@')) throw new DomainException("E-mail inválido.");
        return e;
    }
}
