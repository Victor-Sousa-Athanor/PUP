namespace Pup.Domain.Comum;

/// <summary>Regra de negócio violada. A mensagem é amigável: vai direto para a tela do usuário.</summary>
public sealed class DomainException(string mensagem) : Exception(mensagem);
