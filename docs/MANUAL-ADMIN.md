# Manual do Administrador — Portal de Utilidades Públicas (PUP)

Este guia explica como acessar a área administrativa do portal e como inserir, editar, atualizar a situação ou remover informações em todas as abas (`Coleta`, `Obras`, `Manutenção`, `Comunicados` e `Regiões`).

---

## 1. Como acessar a área administrativa

Você pode abrir o painel administrativo de três formas:

1. **Pelo rodapé de qualquer página do portal:** clique em **Área administrativa** no canto inferior direito (`index.html`, `coleta.html`, `obras.html`, `manutencao.html` ou `comunicados.html`).
2. **Direto pelo endereço no navegador:**
   - Localmente (API rodando): `http://localhost:5200/admin.html`
   - Pela internet (quando exposto via Cloudflare Tunnel): `https://seu-endereco.trycloudflare.com/admin.html` (ou o seu domínio configurado).
3. **Pela API (integração / testes):** as rotas protegidas exigem cabeçalho `Authorization: Bearer <token>` obtido em `POST /api/auth/login`. Em ambiente de desenvolvimento, o contrato OpenAPI fica em `/openapi/v1.json`.

> **Importante:** o portal público (leitura) é aberto a qualquer cidadão sem login. Somente a página `admin.html` e as rotas de escrita (`POST`, `PUT`, `DELETE`) exigem autenticação.

---

## 2. Login e credenciais iniciais

Na tela inicial de `admin.html`, informe **E-mail** e **Senha** do administrador:

| Ambiente | Onde está configurado | Credencial padrão sugerida |
|---|---|---|
| Desenvolvimento local | `src/Pup.Api/appsettings.Development.json` (`Pup:AdminInicial`) | `admin@pup.local` / `Admin@12345` |
| Produção / Pacote instalado | `appsettings.Production.json` na pasta do executável | Definida no arquivo (altere antes de publicar) |

- A sessão utiliza um token JWT válido por **8 horas** por padrão (`Jwt:HorasValidade`).
- Se o token expirar ou for inválido, o sistema encerra a sessão automaticamente e solicita novo login.
- Ao terminar o trabalho em computadores compartilhados, clique em **Sair**.

---

## 3. Como gerenciar cada aba

### 3.1. Aba `Coleta e Pontos` (alimenta `coleta.html` e `Início`)

Dividida em duas seções:

1. **Coleta de resíduos (calendário semanal)**
   - **Inserir:** escolha a **Região**, a **Modalidade** (`Coleta comum` ou `Coleta seletiva`), marque os **Dias da semana** (pelo menos um), informe **Horário inicial** e **Horário final** (o final deve ser maior que o inicial), e opcionalmente **Observação** e **Fonte oficial (URL)**; clique em **Salvar coleta**.
   - **Editar:** clique em **Editar** no card da coleta, altere os dados no formulário no topo e salve (ou clique em **Cancelar edição**).
   - **Alteração temporária / suspensão:** clique em **Alterar situação**, escolha `Temporariamente alterada` ou `Suspensa`, informe uma data opcional em **Válido até** e escreva o **Aviso aos moradores**. Depois da data limite, o portal volta a mostrar a coleta como regular automaticamente.
   - **Normalizar:** quando a coleta estiver alterada/suspensa, clique em **Normalizar** para voltar ao cronograma normal imediatamente.
   - **Excluir:** remove o registro definitivamente após confirmação.

2. **Pontos de descarte (abrigos ecológicos, lixeiras comunitárias e ecopontos)**
   - **Inserir / Editar:** informe **Nome**, **Tipo**, **Região**, **Endereço / Referência** e opcionalmente **Latitude/Longitude** (sempre as duas juntas), **Orientações**, **Fonte oficial** e marque se está **Ativo no portal público**.
   - Pontos ativos aparecem automaticamente na aba pública **Coleta**.

---

### 3.2. Aba `Obras` (alimenta `obras.html` e `Início`)

1. **Inserir nova obra:** preencha **Título**, **Região**, **Local / Via** (opcional), **Situação inicial** (`Programado`, `Em andamento` ou `Alterado`), **Data de início**, **Previsão de conclusão**, **Impacto na via**, **Fonte oficial** e **Descrição**; clique em **Salvar obra**.
   - *Regra do sistema:* uma obra nova não pode ser criada já como `Concluído` ou `Cancelado`; cadastre como `Programado` ou `Em andamento` e depois mude a situação.
2. **Editar dados:** clique em **Editar dados** para corrigir título, local, descrição, datas, impacto na via ou fonte oficial.
3. **Mudar situação e registrar atualização:** clique em **Mudar situação**, selecione o novo status (`Programado`, `Em andamento`, `Temporariamente alterado`, `Concluído` ou `Cancelado`) e informe uma **Nota de atualização** (obrigatória). Cada mudança grava uma linha permanente no histórico da obra e define `DataConclusao` automaticamente ao concluir.
4. **Ver histórico / Excluir:** clique em **Histórico** para conferir todas as atualizações gravadas, ou **Excluir** para remover o registro.

---

### 3.3. Aba `Manutenção` (alimenta `manutencao.html` e `Início`)

Possui exatamente o mesmo fluxo da aba **Obras**, filtrado para intervenções do tipo **Manutenção** (nivelamento de estradas, iluminação pública, poda, drenagem, etc.):

- **Salvar manutenção** (`Programado`, `Em andamento` ou `Alterado`)
- **Editar dados**
- **Mudar situação** (com nota obrigatória gravada no histórico)
- **Histórico** e **Excluir**

---

### 3.4. Aba `Comunicados` (alimenta `comunicados.html` e `Início`)

1. **Inserir / Editar comunicado:** preencha **Título**, **Categoria** (`Geral`, `Coleta`, `Obras`, `Manutenção` ou `Alerta`), **Região** (deixe em **Distrito inteiro** para valer para toda São Francisco Xavier), **Válido até** (opcional; não pode estar no passado), **Resumo curto**, **Conteúdo completo**, **Fonte oficial** e marque **Exibir como destaque na página inicial** se desejar priorizá-lo na Home.
2. **Vigência automática:** comunicados sem data em **Válido até** ou com data maior/igual a hoje aparecem marcados como **Vigente** e são listados no portal público. Após a data de validade, ficam marcados como **Vencido** apenas na área administrativa (histórico).
3. **Excluir:** remove o comunicado definitivamente após confirmação.

---

### 3.5. Aba `Regiões` (alimenta os seletores de todas as abas)

Quase todos os cadastros exigem uma região de São Francisco Xavier (ex.: *Centro*, *Zona rural*, *Estrada do Varadouro*).

1. **Inserir região:** informe **Nome da região**, **Tipo** (`Urbana` ou `Rural`) e **Descrição** opcional.
2. **Editar / Ativar ou desativar:** clique em **Editar** e desmarque **Região ativa** para ocultá-la sem apagar coletas, obras ou manutenções já vinculadas a ela.
3. Ao salvar uma nova região, todos os formulários das demais abas atualizam a lista de regiões automaticamente.

---

## 4. Como as alterações aparecem no portal público

- Toda gravação feita em `admin.html` vai direto para o banco PostgreSQL.
- Ao abrir ou recarregar `index.html`, `coleta.html`, `obras.html`, `manutencao.html` ou `comunicados.html`, o site consulta `/api/portal/servicos` e `/api/portal/comunicados` e passa a exibir os registros atualizados.
- **Aviso de demonstração:** registros cujo texto comece com `[Exemplo]` continuam sendo sinalizados como demonstrativos. Ao cadastrar registros reais pelo painel administrativo sem `[Exemplo]`, eles aparecem normalmente como registros ativos (e você pode excluir os registros de exemplo conforme for publicando os dados reais).

---

## 5. Regras de validação comuns (erros que o sistema bloqueia)

- **Link de fonte oficial:** é opcional, mas quando preenchido deve começar com `http://` ou `https://`.
- **Coleta:** exige ao menos 1 dia da semana e `Horário final > Horário inicial`.
- **Pontos de descarte:** informe `Latitude` e `Longitude` juntas, ou deixe ambas em branco.
- **Obras e Manutenções:** a **Previsão de conclusão** não pode ser anterior à **Data de início**; mudar a situação sempre exige uma **Nota de atualização**.
- **Comunicados:** a data **Válido até** não pode estar no passado no momento do salvamento.
