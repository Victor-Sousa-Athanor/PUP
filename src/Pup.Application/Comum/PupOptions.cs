namespace Pup.Application.Comum;

public sealed class PupOptions
{
    public const string Secao = "Pup";

    /// <summary>Cria regiões, coletas, obras e comunicados de exemplo quando o banco está vazio.</summary>
    public bool SemearDadosExemplo { get; set; } = true;

    /// <summary>Administrador criado na primeira execução (troque a senha em produção!).</summary>
    public AdminInicialOptions AdminInicial { get; set; } = new();
}

public sealed class AdminInicialOptions
{
    public string Nome { get; set; } = "Administrador";
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
