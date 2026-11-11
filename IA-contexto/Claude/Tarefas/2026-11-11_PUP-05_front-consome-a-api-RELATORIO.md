# Relatório — PUP-05: O front passa a ler da API

## Arquivos alterados/criados

Na pasta do front:
- `assets/js/data.js` (renomeados `demoServices` e `demoAnnouncements`, adicionados `apiBase()`, `getJson()`, `load()` com fallback para demonstração e `root.PUP.ready`)
- `assets/js/app.js` (`start()` convertido para `async function` aguardando `root.PUP.ready` após registrar o menu móvel)
- `assets/js/components.js` (`announcementItem` no modo compacto exibe o selo `Demonstração` apenas quando `item.isDemo` é verdadeiro)
- `assets/js/pages/home.js` (sufixo `· exemplos demonstrativos` exibido somente quando `root.PUP.data.source !== "api"`)
- `assets/js/pages/services.js` (opções de `#service-location` preenchidas dinamicamente a partir dos dados e sufixo `· demonstração` exibido somente quando `source !== "api"`)
- `assets/js/pages/announcements.js` (sufixo `· todos demonstrativos` exibido somente quando `source !== "api"`)
- `coleta.html`, `obras.html`, `manutencao.html` (adicionada `<option>Cancelado</option>` em `#service-status`)
- `comunicados.html` (adicionada `<option>Alerta</option>` em `#announcement-category`)
- `README.md` (adicionada seção "Executar com a API" sem citar o nome interno da pasta)
- `ORIGEM.md` intacto (não alterado)

Fora da pasta do front:
- `ferramentas/smoke.ps1` (adicionadas `/api/portal/servicos` e `/api/portal/comunicados` à lista `$publicas`)
- `IA-contexto/Claude/Tarefas/00_FILA.md` (status da PUP-05 marcado como **Feito**)
- `IA-contexto/Claude/Tarefas/2026-11-11_PUP-05_front-consome-a-api-RELATORIO.md` (este relatório)

## O que foi conferido (API no ar, Edge headless e modo reserva offline)

1. **API no ar (`http://localhost:5200/`):** `ferramentas/smoke.ps1` verde (`200 OK` nas 5 páginas, CSS/JS e `/api/portal/servicos` + `/api/portal/comunicados`, `404` em `/ORIGEM.md` e `/README.md`).
2. **Início (`/`):** DOM renderizado após `fetch` exibiu `12 serviços encontrados` (sem sufixo demonstrativo), os 4 cards recentes do banco sem a marca `[Exemplo]` (`Abrigo ecológico da capela`, `Abrigo ecológico do Varadouro`, `Adequação de via`, `Coleta comum`) e os 2 comunicados (`Solicitações pelo 156`, `Coleta seletiva também na zona rural`) sem o selo `Demonstração`.
3. **Coleta, Obras e Manutenção (`coleta.html`, `obras.html`, `manutencao.html`):** `<select id="service-location">` populado dinamicamente com `Centro`, `Estrada do Varadouro` e `Zona rural`; `#service-status` com `Cancelado`; contagens respectivas de `6`, `3` e `3` registros.
4. **Comunicados (`comunicados.html`):** `#announcement-category` com opção `Alerta`; `4 comunicados encontrados`, incluindo `Interdição parcial na Estrada do Varadouro` (`Alerta`).
5. **Modo reserva offline (API parada, `index.html` aberto via `file://`):** exibiu `8 serviços encontrados · exemplos demonstrativos`, os 4 primeiros itens de demonstração e os 2 comunicados com o selo `Demonstração`.
6. **Sigilo do nome interno da pasta:** varredura em todos os arquivos públicos, no código-fonte HTML/JS/CSS, nos títulos `<title>`, nos payloads de `/api/painel`, `/api/portal/servicos` e `/api/portal/comunicados`, e nos DOMs renderizados das 5 páginas confirmou zero ocorrências do nome interno (ele permanece apenas em `appsettings.Development.json` e dividido no `smoke.ps1`).

## O que o João deve conferir (inspeção visual no navegador)

- Abrir `http://localhost:5200/` no navegador e conferir visualmente F12 → Rede (`/api/portal/servicos` e `/api/portal/comunicados` com status 200), Console sem erros, título da aba e código-fonte (`Ctrl+U`) sem o nome interno da pasta.
- Testar interativamente os filtros de situação, localidade e categoria nas 5 telas e conferir `index.html` aberto direto do disco com a API parada.
