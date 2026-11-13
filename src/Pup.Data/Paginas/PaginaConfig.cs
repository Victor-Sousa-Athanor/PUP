using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pup.Domain.Paginas;

namespace Pup.Data.Paginas;

public sealed class PaginaConfig : IEntityTypeConfiguration<Pagina>
{
    public void Configure(EntityTypeBuilder<Pagina> b)
    {
        b.Property(x => x.Titulo).HasMaxLength(80);
        b.Property(x => x.Rota).HasMaxLength(120);
        b.Property(x => x.Resumo).HasMaxLength(300);
        b.Property(x => x.Conteudo).HasMaxLength(8000);
        b.Property(x => x.FonteOficialUrl).HasMaxLength(500);
        b.HasIndex(x => x.Rota).IsUnique();
    }
}
