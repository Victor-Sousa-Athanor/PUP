using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pup.Domain.Acesso;

namespace Pup.Data.Configuracoes;

internal sealed class AdministradorConfig : IEntityTypeConfiguration<Administrador>
{
    public void Configure(EntityTypeBuilder<Administrador> b)
    {
        b.ToTable("administradores");
        b.HasKey(x => x.Id);
        b.Property(x => x.Nome).HasMaxLength(120).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.Property(x => x.SenhaHash).HasMaxLength(200).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
    }
}
