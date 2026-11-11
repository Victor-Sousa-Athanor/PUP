using Pup.Domain.Comum;

namespace Pup.Contracts;

public sealed record RegiaoDto(Guid Id, string Nome, TipoRegiao Tipo, string? Descricao, bool Ativa, DateTime AtualizadoEm);

public sealed record SalvarRegiaoRequest(string Nome, TipoRegiao Tipo, string? Descricao, bool Ativa = true);
