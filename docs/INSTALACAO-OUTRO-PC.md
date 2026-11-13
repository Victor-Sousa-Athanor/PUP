# Como Transferir e Instalar em Outro PC (Windows)

É possível gerar um pacote autocontido ou um **instalador único (`PUP-Setup.exe`)** para copiar para outro computador Windows **sem precisar instalar Visual Studio nem .NET 10 SDK** na máquina de destino.

> **Importante sobre o banco de dados:** a aplicação exige um **PostgreSQL** acessível (Docker com o `docker-compose.yml` incluso **ou** PostgreSQL 17 instalado nativamente no Windows).

---

## Passo 1: Gerar o pacote instalável neste PC

Execute na raiz do repositório:

```powershell
powershell -ExecutionPolicy Bypass -File ferramentas\empacotar.ps1
```

O script faz o seguinte automaticamente:

1. Publica a API em modo **self-contained** para Windows x64 (`Pup.Api.exe` já com o runtime do .NET 10 embutido).
2. Copia **apenas os arquivos públicos do site** (`.html`, `.css`, `.js`, imagens e fontes) para `dist\pup-win-x64\front\`, excluindo arquivos internos `.md`.
3. Inclui `docker-compose.yml`, um `appsettings.Production.json` pronto para rodar (com `Front:Pasta = "./front"` e uma `Jwt:Chave` aleatória de 48 caracteres gerada na hora), o modelo `appsettings.Production.exemplo.json` e o atalho `iniciar-pup.bat`.
4. Compacta tudo em **`dist\pup-win-x64.zip`**.
5. Se o **Inno Setup 6** estiver instalado no PC, compila automaticamente o instalador único **`dist\PUP-Setup.exe`** a partir de [instalador.iss](../ferramentas/instalador.iss).

### Para gerar o instalador `.exe` único (`PUP-Setup.exe`)

Se o Inno Setup ainda não estiver instalado neste PC, instale com um comando e rode o empacotador novamente:

```powershell
winget install --id JRSoftware.InnoSetup
powershell -ExecutionPolicy Bypass -File ferramentas\empacotar.ps1
```

O arquivo final **`dist\PUP-Setup.exe`** é um único executável instalável pronto para enviar por pendrive, rede ou nuvem para outro PC Windows.

---

## Passo 2: Preparar o outro PC (apenas banco PostgreSQL)

O outro PC **não precisa** do .NET SDK nem do Visual Studio. Escolha **uma** das duas formas para o banco:

### Forma A — Com Docker Desktop (mais rápida, recomendada)

1. Instale o **Docker Desktop** no outro PC.
2. Na pasta onde o PUP foi instalado (ex.: `C:\Program Files\PUP` ou onde você extraiu o `.zip`), execute:

   ```powershell
   docker compose up -d
   ```

3. Isso sobe o `postgres:17` exatamente na porta `5433`, usuário `pup`, senha `pup` e banco `pup`, sem precisar editar `appsettings.Production.json`.

### Forma B — Com PostgreSQL nativo no Windows (sem Docker)

1. Instale o PostgreSQL 17 no outro PC (por exemplo `winget install --id PostgreSQL.PostgreSQL.17` ou instalador oficial da EDB).
2. Crie o usuário `pup` e o banco `pup` (no `psql` ou pgAdmin):

   ```sql
   CREATE USER pup WITH PASSWORD 'pup';
   CREATE DATABASE pup OWNER pup;
   ```

3. Edite o arquivo `appsettings.Production.json` na pasta de instalação do PUP e ajuste a porta em `ConnectionStrings:Postgres` de `5433` para `5432` (porta padrão do Postgres nativo no Windows):

   ```json
   "ConnectionStrings": {
     "Postgres": "Host=localhost;Port=5432;Database=pup;Username=pup;Password=pup"
   }
   ```

---

## Passo 3: Instalar e rodar no outro PC

1. Se gerou o instalador, execute **`PUP-Setup.exe`** no outro PC (ou descompacte **`pup-win-x64.zip`** em uma pasta de sua preferência, como `C:\PUP`).
2. Antes de expor para usuários reais, revise `appsettings.Production.json` na pasta instalada para definir `Pup:AdminInicial:Email` e `Pup:AdminInicial:Senha` fortes (veja [CONFIGURACAO-PRODUCAO.md](CONFIGURACAO-PRODUCAO.md)).
3. Execute **`iniciar-pup.bat`** (ou pelo atalho **Iniciar PUP** criado no Menu Iniciar).
4. Acesse no navegador do outro PC:
   * Portal: `http://localhost:5200/`
   * Verificação da API: `http://localhost:5200/health`

Na primeira inicialização com o banco vazio, a aplicação cria as tabelas automaticamente e preenche os dados iniciais de demonstração (`[Exemplo]`).

## Passo 4 (opcional): Expor pela internet no outro PC

No outro PC, com o portal rodando em `http://localhost:5200`, basta rodar em outro terminal:

```powershell
winget install --id Cloudflare.cloudflared
cloudflared tunnel --url http://localhost:5200
```

Veja mais detalhes em [EXPOR-COM-CLOUDFLARE.md](EXPOR-COM-CLOUDFLARE.md).
