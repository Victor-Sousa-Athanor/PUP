namespace Pup.Application.Comum;

/// <summary>Vira HTTP 404.</summary>
public sealed class NaoEncontradoException(string mensagem) : Exception(mensagem);

/// <summary>Vira HTTP 401 (login inválido ou ausente).</summary>
public sealed class NaoAutenticadoException(string mensagem = "E-mail ou senha inválidos.") : Exception(mensagem);

/// <summary>Vira HTTP 403.</summary>
public sealed class AcessoNegadoException(string mensagem = "Você não tem acesso a este recurso.") : Exception(mensagem);
