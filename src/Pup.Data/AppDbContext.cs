using Microsoft.EntityFrameworkCore;
using Pup.Data.Comum;
using Pup.Domain.Acesso;
using Pup.Domain.Coleta;
using Pup.Domain.Comunicados;
using Pup.Domain.Intervencoes;
using Pup.Domain.Regioes;

namespace Pup.Data;

/// <summary>
/// Único ponto de acesso ao banco. Somente classes *Repo e *Query (neste projeto) usam o contexto.
/// </summary>
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Regiao> Regioes => Set<Regiao>();
    public DbSet<ColetaResiduos> Coletas => Set<ColetaResiduos>();
    public DbSet<PontoDescarte> PontosDescarte => Set<PontoDescarte>();
    public DbSet<Intervencao> Intervencoes => Set<Intervencao>();
    public DbSet<AtualizacaoIntervencao> AtualizacoesIntervencao => Set<AtualizacaoIntervencao>();
    public DbSet<Comunicado> Comunicados => Set<Comunicado>();
    public DbSet<Administrador> Administradores => Set<Administrador>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<string>().HaveMaxLength(2000);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Os Ids são gerados no domínio (Guid v7). ValueGeneratedNever faz o EF tratar
        // filhos novos encontrados em coleções (histórico da obra) como Added (e não Modified).
        foreach (var entity in modelBuilder.Model.GetEntityTypes().Where(e => !e.IsOwned()))
        {
            var id = entity.FindProperty("Id");
            if (id is not null && id.ClrType == typeof(Guid))
                id.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
        }
    }
}
