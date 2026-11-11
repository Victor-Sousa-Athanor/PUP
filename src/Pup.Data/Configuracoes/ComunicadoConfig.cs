using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pup.Domain.Comunicados;
using Pup.Domain.Regioes;

namespace Pup.Data.Configuracoes;

internal sealed class ComunicadoConfig : IEntityTypeConfiguration<Comunicado>
{
    public void Configure(EntityTypeBuilder<Comunicado> b)
    {
        b.ToTable("comunicados");
        b.HasKey(x => x.Id);
        b.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
        b.Property(x => x.Resumo).HasMaxLength(300).IsRequired();
        b.Property(x => x.Conteudo).HasMaxLength(4000).IsRequired();
        b.Property(x => x.FonteOficialUrl).HasMaxLength(500);
        b.HasIndex(x => x.PublicadoEm);
        b.HasIndex(x => new { x.Categoria, x.ValidoAte });

        b.HasOne<Regiao>().WithMany().HasForeignKey(x => x.RegiaoId).OnDelete(DeleteBehavior.SetNull);
    }
}
