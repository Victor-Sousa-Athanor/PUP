using Pup.Application.Comum;
using Pup.Data.Acesso;
using Pup.Domain.Acesso;
using Pup.Domain.Comum;

namespace Pup.Application.Features.Acesso;

/// <summary>Dados mínimos do administrador logado (o token JWT é gerado na API).</summary>
public sealed record AdministradorAutenticado(Guid Id, string Nome, string Email);

public sealed class AdministradorService(AdministradorRepo repo, SenhaHasher hasher)
{
    public async Task<AdministradorAutenticado> AutenticarAsync(string? email, string? senha, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha)) throw new NaoAutenticadoException();

        string emailNormalizado;
        try { emailNormalizado = Administrador.NormalizarEmail(email); }
        catch (DomainException) { throw new NaoAutenticadoException(); }

        var admin = await repo.ObterPorEmailAsync(emailNormalizado, ct);
        // Mesma mensagem para e-mail inexistente e senha errada (não revela quais e-mails existem).
        if (admin is null || !admin.Ativo || !hasher.Verificar(senha, admin.SenhaHash))
            throw new NaoAutenticadoException();

        admin.RegistrarAcesso();
        await repo.SalvarAsync(ct);
        return new AdministradorAutenticado(admin.Id, admin.Nome, admin.Email);
    }

    /// <summary>Cria o primeiro administrador a partir da configuração, se ainda não existir nenhum.</summary>
    public async Task<bool> GarantirAdminInicialAsync(AdminInicialOptions opcoes, CancellationToken ct)
    {
        if (await repo.ExisteAlgumAsync(ct)) return false;
        if (string.IsNullOrWhiteSpace(opcoes.Email) || string.IsNullOrWhiteSpace(opcoes.Senha)) return false;

        repo.Adicionar(Administrador.Criar(opcoes.Nome, opcoes.Email, hasher.Gerar(opcoes.Senha)));
        await repo.SalvarAsync(ct);
        return true;
    }
}
