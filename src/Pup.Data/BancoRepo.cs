using Microsoft.EntityFrameworkCore;

namespace Pup.Data;

/// <summary>Criação e migração de schema na subida da API (sem acesso a entidades específicas).</summary>
public sealed class BancoRepo(AppDbContext db)
{
    public async Task PrepararAsync(CancellationToken ct = default)
    {
        if (db.Database.GetMigrations().Any())
            await db.Database.MigrateAsync(ct);
        else
            await db.Database.EnsureCreatedAsync(ct);

        // Garante tabela de páginas dinâmicas em bancos já criados antes da funcionalidade.
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "Paginas" (
                "Id" uuid NOT NULL,
                "Titulo" character varying(80) NOT NULL,
                "Rota" character varying(120) NOT NULL,
                "Resumo" character varying(300),
                "Conteudo" character varying(8000),
                "Ordem" integer NOT NULL,
                "ExibirNoMenu" boolean NOT NULL,
                "Publicada" boolean NOT NULL,
                "EhSistema" boolean NOT NULL,
                "FonteOficialUrl" character varying(500),
                "CriadoEm" timestamp with time zone NOT NULL,
                "AtualizadoEm" timestamp with time zone NOT NULL,
                CONSTRAINT "PK_Paginas" PRIMARY KEY ("Id")
            );
            CREATE UNIQUE INDEX IF NOT EXISTS "IX_Paginas_Rota" ON "Paginas" ("Rota");
            """, ct);
    }
}
