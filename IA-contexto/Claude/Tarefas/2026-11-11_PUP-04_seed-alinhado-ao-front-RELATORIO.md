# Relatório — PUP-04: Dados de exemplo alinhados ao front

## Arquivos alterados/criados

- `src/Pup.Application/Features/Inicializacao/DadosExemplo.cs` (reescrito `SemearAsync` mantendo assinatura, constantes de notícias oficiais e único `SalvarAsync`)
- `IA-contexto/Claude/Tarefas/00_FILA.md` (status da PUP-04 marcado como **Feito**)
- `IA-contexto/Claude/Tarefas/2026-11-11_PUP-04_seed-alinhado-ao-front-RELATORIO.md` (este relatório)

## Erros encontrados e como foram resolvidos

- Nenhum erro de compilação, teste ou seed.
- `README.md` não precisou de alterações porque a descrição existente dos dados de exemplo continua exata.

## O que cada rota devolveu após `docker compose down -v` e recriação do banco

1. **`GET /api/regioes` — 3 itens:**
   - `Centro` (`Urbana`)
   - `Estrada do Varadouro` (`Rural`)
   - `Zona rural` (`Rural`)

2. **`GET /api/portal/servicos` — 12 itens (4 coletas + 2 pontos + 3 obras + 3 manutenções):**
   - `Abrigo ecológico da capela` (`Coleta de resíduos` · `Zona rural` · `Em andamento`)
   - `Abrigo ecológico do Varadouro` (`Coleta de resíduos` · `Estrada do Varadouro` · `Em andamento`)
   - `Adequação de via` (`Obras` · `Estrada do Varadouro` · `Em andamento`)
   - `Coleta comum` (`Coleta de resíduos` · `Centro` · `Programado`)
   - `Coleta comum` (`Coleta de resíduos` · `Estrada do Varadouro` · `Alterado`)
   - `Coleta seletiva` (`Coleta de resíduos` · `Centro` · `Programado`)
   - `Coleta seletiva` (`Coleta de resíduos` · `Zona rural` · `Programado`)
   - `Desobstrução de drenagem` (`Manutenção` · `Estrada do Varadouro` · `Em andamento`)
   - `Manutenção de iluminação` (`Manutenção` · `Centro` · `Programado`)
   - `Melhoria de drenagem` (`Obras` · `Zona rural` · `Programado`)
   - `Nivelamento de estrada` (`Manutenção` · `Zona rural` · `Concluído`)
   - `Revitalização da praça central` (`Obras` · `Centro` · `Programado`)

3. **`GET /api/portal/comunicados` — 4 itens:**
   - `Solicitações pelo 156` (`Geral` · `isDemo: false`)
   - `Coleta seletiva também na zona rural` (`Coleta` · `isDemo: false`)
   - `Sobre este portal` (`Geral` · `isDemo: false`)
   - `Interdição parcial na Estrada do Varadouro` (`Alerta` · `isDemo: true`)

## Verificações realizadas

- `dotnet build Pup.slnx`: 0 erros (apenas os 2 warnings `MSB3277` já registrados).
- `dotnet test tests/Pup.Tests`: 37/37 passando.
- `ferramentas/smoke.ps1`: verde.

## O que o João deve conferir

- Confirmar as contagens e títulos acima em `/api/regioes` (3), `/api/portal/servicos` (12) e `/api/portal/comunicados` (4).
