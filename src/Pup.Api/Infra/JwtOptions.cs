namespace Pup.Api.Infra;

public sealed class JwtOptions
{
    public const string Secao = "Jwt";
    public string Emissor { get; set; } = "pup-api";
    public string Audiencia { get; set; } = "pup-admin";
    /// <summary>Mínimo de 32 caracteres (HMAC-SHA256). Em produção, use user-secrets ou variável de ambiente.</summary>
    public string Chave { get; set; } = string.Empty;
    public int HorasValidade { get; set; } = 8;
}
