using Pup.Data.Comum;
using Pup.Domain.Intervencoes;

namespace Pup.Data.AtualizacoesIntervencao;

public sealed class AtualizacaoIntervencaoRepo(AppDbContext db) : RepoBase<AtualizacaoIntervencao>(db)
{
}
