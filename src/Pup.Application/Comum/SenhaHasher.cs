using System.Security.Cryptography;

namespace Pup.Application.Comum;

/// <summary>PBKDF2-SHA256 (sem pacote externo). Formato gravado: pbkdf2$iteracoes$sal$hash (Base64).</summary>
public sealed class SenhaHasher
{
    private const int Iteracoes = 100_000;
    private const int TamanhoSal = 16;
    private const int TamanhoHash = 32;

    public string Gerar(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            throw new Domain.Comum.DomainException("A senha precisa ter pelo menos 8 caracteres.");

        var sal = RandomNumberGenerator.GetBytes(TamanhoSal);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, sal, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
        return $"pbkdf2${Iteracoes}${Convert.ToBase64String(sal)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string senha, string gravado)
    {
        var partes = gravado.Split('$');
        if (partes.Length != 4 || partes[0] != "pbkdf2" || !int.TryParse(partes[1], out var iteracoes)) return false;

        var sal = Convert.FromBase64String(partes[2]);
        var esperado = Convert.FromBase64String(partes[3]);
        var calculado = Rfc2898DeriveBytes.Pbkdf2(senha ?? string.Empty, sal, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}
