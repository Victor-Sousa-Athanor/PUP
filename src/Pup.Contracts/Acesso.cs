namespace Pup.Contracts;

public sealed record LoginRequest(string Email, string Senha);

public sealed record TokenResponse(string Token, DateTime ExpiraEm, string Nome);
