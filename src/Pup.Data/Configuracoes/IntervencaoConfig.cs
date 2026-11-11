using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pup.Domain.Intervencoes;
using Pup.Domain.Regioes;

namespace Pup.Data.Configuracoes;

internal sealed class IntervencaoConfig : IEntityTypeConfiguration<Intervencao>
{
    public void Configure(EntityTypeBuilder<Intervencao> b)
    {
        b.ToTable("intervencoes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(2000).IsRequired();
        b.Property(x => x.Local).HasMaxLength(300);
        b.Property(x => x.ImpactoNaVia).HasMaxLength(300);
        b.Property(x => x.FonteOficialUrl).HasMaxLength(500);
        b.HasIndex(x => new { x.Tipo, x.Status });
        b.HasIndex(x => x.RegiaoId);
        b.HasIndex(x => x.AtualizadoEm);

        b.HasOne<Regiao>().WithMany().HasForeignKey(x => x.RegiaoId).OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Historico).WithOne().HasForeignKey(a => a.IntervencaoId).OnDelete(DeleteBehavior.Cascade);
        b.Navigation(x => x.Historico).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

internal sealed class AtualizacaoIntervencaoConfig : IEntityTypeConfiguration<AtualizacaoIntervencao>
{
    public void Configure(EntityTypeBuilder<AtualizacaoIntervencao> b)
    {
        b.ToTable("atualizacoes_intervencao");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nota).HasMaxLength(500).IsRequired();
        b.HasIndex(x => new { x.IntervencaoId, x.Data });
    }
}
