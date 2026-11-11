using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pup.Domain.Regioes;

namespace Pup.Data.Configuracoes;

internal sealed class RegiaoConfig : IEntityTypeConfiguration<Regiao>
{
    public void Configure(EntityTypeBuilder<Regiao> b)
    {
        b.ToTable("regioes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(120).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(500);
        b.HasIndex(x => x.Nome).IsUnique();
    }
}
