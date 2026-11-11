# AGENTS.md — Portal de Utilidades Públicas (PUP)

Back-end do Projeto Integrador II (UNIVESP): API que centraliza informações de coleta de lixo, obras,
manutenção e comunicados de São Francisco Xavier (São José dos Campos - SP).

## Regras (sempre)
- C# / .NET 10. **Sem Python.** Datas e números gravados/lidos com `CultureInfo.InvariantCulture`.
- **Nenhum controller acessa o banco.** Controller → `IDispatcher` → slice (Command/Query + Handler) → `<Entidade>Service`
  → `<Entidade>Repo` (escrita) / `<Entidade>Query` (leitura) → `AppDbContext`.
- Só `*Repo` e `*Query` (projeto `Pup.Data`) usam o `AppDbContext`.
- Regras de negócio ficam no Domínio (`DomainException` com mensagem amigável — ela vai direto para a tela).
- Nas `*Query`, filtre/ordene sobre as entidades e projete para DTO **por último** (o EF não traduz filtro depois de `new Record(...)`).
- Todo registro publicado tem `AtualizadoEm` e, quando houver, `FonteOficialUrl` (o portal complementa os canais oficiais, não os substitui).
- Dado inventado (seed) começa com "[Exemplo]".
- Arquivos com mais de ~150 linhas: `partial class` em `Classe.Assunto.cs`.
- Nomes, comentários e textos em **português**. Comentário explica o *porquê*.

## Mapa rápido
| Onde | O quê |
|---|---|
| `src\Pup.Domain` | Entidades e regras (sem dependências) |
| `src\Pup.Contracts` | DTOs/requests da API e `Rotulos` (textos pt-BR dos enums) |
| `src\Pup.Data\<Feature>\` | `XRepo`, `XQuery`, configurações EF |
| `src\Pup.Application\Features\<Feature>\` | `XService` + slices (`static class Slice { record Command/Query; class Handler }`) |
| `src\Pup.Api\Controllers` | Rotas HTTP (só chamam o `IDispatcher`) |

## Glossário
Intervenção = obra **ou** manutenção (campo `Tipo`) · Histórico = linhas gravadas a cada mudança de situação ·
Coleta alterada = mudança temporária no cronograma (some sozinha depois de `AlteracaoAte`) ·
Comunicado vigente = sem `ValidoAte` ou com `ValidoAte` >= hoje (horário de Brasília).
