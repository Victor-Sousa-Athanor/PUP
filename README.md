# Portal de Utilidades Públicas (PUP) — Back-end

API do **Projeto Integrador II (UNIVESP, 2026)**. Ela reúne num lugar só as informações de serviços públicos de
**São Francisco Xavier** (distrito de São José dos Campos - SP): coleta de lixo, pontos de descarte, obras,
manutenção e comunicados.

A plataforma **não substitui** os canais oficiais da Prefeitura (156, site, Urbam). Ela organiza as informações
públicas e mostra a **data de atualização** e a **fonte oficial** de cada registro.

## Arquitetura (Figura 2 do relatório)

```
Front-end (Blazor, outro repo/etapa) ──HTTP/JSON──▶ Pup.Api (ASP.NET Core Web API) ──▶ PostgreSQL
                                                       │
                     Controller → IDispatcher → Slice (Command/Query) → Service → Repo/Query → AppDbContext
```

| Projeto | Papel |
|---|---|
| `Pup.Domain` | Entidades e regras: `Regiao`, `ColetaResiduos`, `PontoDescarte`, `Intervencao` (obra/manutenção + histórico), `Comunicado`, `Administrador` |
| `Pup.Contracts` | DTOs e requests da API + `Rotulos` (textos em português dos enums) |
| `Pup.Data` | EF Core + Npgsql. Só `*Repo` (escrita) e `*Query` (leitura) usam o `AppDbContext` |
| `Pup.Application` | Vertical slices (CQRS) e um `Service` por entidade; dados de exemplo |
| `Pup.Api` | Controllers, JWT da área administrativa, tratamento de erros |
| `Pup.Tests` | Testes das regras de domínio (xUnit) |

## Como rodar

Pré-requisitos: .NET SDK 10 e Docker (ou um PostgreSQL local).

```powershell
docker compose up -d                 # Postgres na porta 5433
dotnet build
dotnet test
dotnet run --project src/Pup.Api     # http://localhost:5200
```

Na primeira execução a API cria o schema e preenche **dados de exemplo** (regiões, coletas, obras, comunicados).
Os dias e horários da coleta no Centro vêm das notícias oficiais citadas no relatório. Tudo que foi inventado começa
com **"[Exemplo]"**. Para desligar: `Pup:SemearDadosExemplo = false`.

Para testar as rotas, abra `src/Pup.Api/Pup.Api.http` no Visual Studio. O OpenAPI fica em `/openapi/v1.json`.

### Migrations (recomendado antes de publicar)

Sem migrations a API usa `EnsureCreated` (bom para o protótipo). Para versionar o banco:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef migrations add Inicial -p src/Pup.Data -s src/Pup.Api
```

Depois disso a API aplica as migrations sozinha ao subir.

> Se o banco já tiver sido criado pelo `EnsureCreated`, apague o volume antes (`docker compose down -v`).

## Área administrativa

Enquanto não existe integração automática com a Prefeitura, os responsáveis atualizam os dados à mão
(relatório, seção 2.3.4 E). Para isso:

1. Faça login em `POST /api/auth/login` e receba um token JWT.
2. Envie o token em `Authorization: Bearer <token>` nas rotas de escrita (POST/PUT/DELETE).

Em desenvolvimento, o admin inicial é `admin@pup.local` / `Admin@12345` (veja `appsettings.Development.json`).
**Em produção**, defina `Jwt:Chave` (32+ caracteres) e `Pup:AdminInicial` com user-secrets ou variáveis de ambiente:

```powershell
dotnet user-secrets set "Jwt:Chave" "<uma-chave-grande-e-aleatoria>" --project src/Pup.Api
dotnet user-secrets set "Pup:AdminInicial:Email" "voce@exemplo.com" --project src/Pup.Api
dotnet user-secrets set "Pup:AdminInicial:Senha" "<senha-forte>" --project src/Pup.Api
```

## Rotas

Leitura (públicas):

| Método | Rota | Para quê |
|---|---|---|
| GET | `/api/painel?limite=8` | Página inicial: últimas atualizações, destaques e contadores |
| GET | `/api/busca?termo=` | Busca em todas as categorias |
| GET | `/api/regioes` | Regiões, bairros e estradas |
| GET | `/api/coletas?regiaoId=&modalidade=&dia=&hoje=` | Dias e horários de coleta (`hoje=true` mostra só as de hoje) |
| GET | `/api/pontos-descarte?regiaoId=&tipo=` | Abrigos ecológicos, lixeiras e ecopontos |
| GET | `/api/intervencoes?tipo=&status=&regiaoId=&termo=&abertas=` | Obras e manutenções |
| GET | `/api/obras`, `/api/manutencoes` | Atalhos das intervenções por tipo |
| GET | `/api/intervencoes/{id}` | Detalhe com histórico |
| GET | `/api/comunicados?categoria=&regiaoId=&vigentes=&destaques=` | Avisos e notícias |

Escrita (precisam de token):

| Método | Rota |
|---|---|
| POST/PUT | `/api/regioes`, `/api/regioes/{id}` (desative com `ativa: false`) |
| POST/PUT/DELETE | `/api/coletas`, `/api/coletas/{id}` |
| PUT/DELETE | `/api/coletas/{id}/alteracao` (alteração temporária / normalizar) |
| POST/PUT/DELETE | `/api/pontos-descarte`, `/api/pontos-descarte/{id}` |
| POST/PUT/DELETE | `/api/intervencoes`, `/api/intervencoes/{id}` |
| POST | `/api/intervencoes/{id}/status` (muda a situação e grava no histórico) |
| POST/PUT/DELETE | `/api/comunicados`, `/api/comunicados/{id}` |

Formatos: enums como texto (`"EmAndamento"`, `"Seletiva"`, `"Terca, Quinta"`), datas `"2026-12-01"`,
horários `"07:00:00"`. Erros de negócio voltam como `{ "mensagem": "..." }` com 400, 401, 403 ou 404.

Situações das obras/manutenções: `Programado`, `EmAndamento`, `Concluido`, `TemporariamenteAlterado`, `Cancelado`.

## Expor com Cloudflare Tunnel (demonstração)

Se o protótipo rodar num PC sem IP público:

```powershell
winget install --id Cloudflare.cloudflared
cloudflared tunnel --url http://localhost:5200
```

O `cloudflared` mostra uma URL `https://<algo>.trycloudflare.com` que encaminha para a API local.
Lembre de colocar a URL do front-end em `Cors:Origens`.

## Front-end

O front-end estático é uma cópia do repositório da dupla guardada neste repositório. A API serve os arquivos dessa pasta configurada em `Front:Pasta` (caminho relativo ao projeto `src/Pup.Api` em desenvolvimento, ou caminho absoluto em produção), o que mantém site e API na mesma origem, sem CORS.

Apenas arquivos do site (`.html`, `.css`, `.js`, imagens e fontes) são servidos; arquivos internos da pasta devolvem 404. Com a API rodando, `http://localhost:5200/` abre o site, e um único túnel do Cloudflare (`cloudflared tunnel --url http://localhost:5200`) expõe site e API juntos.

## Referências usadas nos dados

- Prefeitura de SJC — [Coleta de lixo é ampliada em São Francisco Xavier (2023)](https://www.sjc.sp.gov.br/noticias/2023/fevereiro/03/coleta-de-lixo-e-ampliada-em-sao-francisco-xavier/)
- Prefeitura de SJC — [Coleta seletiva chega na zona rural de São Francisco Xavier (2024)](https://www.sjc.sp.gov.br/noticias/2024/junho/27/coleta-seletiva-chega-na-zona-rural-de-sao-francisco-xavier/)

Obs.: as duas notícias divergem nos dias da coleta seletiva no Centro (seg/qua/sex em 2023, ter/qui/sáb em 2024).
O seed usa a mais recente. Confirme com a Urbam antes da apresentação.
