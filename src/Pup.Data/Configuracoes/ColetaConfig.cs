using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pup.Domain.Coleta;
using Pup.Domain.Regioes;

namespace Pup.Data.Configuracoes;

internal sealed class ColetaConfig : IEntityTypeConfiguration<ColetaResiduos>
{
    public void Configure(EntityTypeBuilder<ColetaResiduos> b)
    {
        b.ToTable("coletas");
        b.HasKey(x => x.Id);
        b.Property(x => x.Observacao).HasMaxLength(500);
        b.Property(x => x.AvisoAlteracao).HasMaxLength(500);
        b.Property(x => x.FonteOficialUrl).HasMaxLength(500);
        b.HasIndex(x => new { x.RegiaoId, x.Modalidade });

        // Região não pode ser apagada com coletas vinculadas (desative a região em vez disso).
        b.HasOne<Regiao>().WithMany().HasForeignKey(x => x.RegiaoId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class PontoDescarteConfig : IEntityTypeConfiguration<PontoDescarte>
{
    public void Configure(EntityTypeBuilder<PontoDescarte> b)
    {
        b.ToTable("pontos_descarte");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(150).IsRequired();
        b.Property(x => x.Referencia).HasMaxLength(300).IsRequired();
        b.Property(x => x.Orientacoes).HasMaxLength(1000);
        b.Property(x => x.FonteOficialUrl).HasMaxLength(500);
        b.HasIndex(x => new { x.RegiaoId, x.Ativo });

        b.HasOne<Regiao>().WithMany().HasForeignKey(x => x.RegiaoId).OnDelete(DeleteBehavior.Restrict);
    }
}
