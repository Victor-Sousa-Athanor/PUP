# Como Expor o Portal Agora (Cloudflare Tunnel)

Como a API serve o front-end na mesma origem (`http://localhost:5200`), **um único túnel** expõe tanto as páginas HTML/CSS/JS quanto as rotas `/api/*` para qualquer dispositivo acessar pela internet.

## Opção 1: Exposição rápida (agora, sem domínio próprio)

Ideal para demonstrações e testes rápidos na hora.

### 1. Suba o banco de dados PostgreSQL

Na raiz do repositório:

```powershell
docker compose up -d
```

Confirme se o contêiner `pup-postgres` está ativo na porta `5433`:

```powershell
docker ps
```

### 2. Inicie a aplicação na porta `5200`

Pelo código-fonte (em uma janela do PowerShell):

```powershell
dotnet run --project src/Pup.Api
```

Ou, se estiver usando o pacote publicado (`Pup.Api.exe`), execute `iniciar-pup.bat` (ou `.\Pup.Api.exe --urls http://localhost:5200`).

Valide localmente antes de abrir o túnel:

* Portal: `http://localhost:5200/`
* Saúde da API: `http://localhost:5200/health`

### 3. Instale o `cloudflared` (apenas na primeira vez)

Em outra janela do PowerShell:

```powershell
winget install --id Cloudflare.cloudflared
```

> Feche e reabra o PowerShell após a instalação se o comando `cloudflared` não for reconhecido imediatamente.

### 4. Abra o túnel apontando para `http://localhost:5200`

```powershell
cloudflared tunnel --url http://localhost:5200
```

O terminal exibirá uma URL pública temporária no formato:

```text
https://algo-aleatorio.trycloudflare.com
```

Basta enviar esse link para qualquer pessoa testar o portal completo pelo celular ou outro computador. Ao fechar o terminal com `Ctrl+C`, o link para de funcionar.

---

## Opção 2: URL fixa com domínio próprio na Cloudflare

Se você tiver um domínio gerenciado no Cloudflare:

```powershell
cloudflared tunnel login
cloudflared tunnel create pup-sfx
cloudflared tunnel route dns pup-sfx pup.seudominio.com.br
cloudflared tunnel run --url http://localhost:5200 pup-sfx
```

## Cuidados importantes antes de expor publicamente

1. **Troque a credencial administrativa e a chave JWT** se for deixar o portal no ar fora de uma apresentação curta (veja [CONFIGURACAO-PRODUCAO.md](CONFIGURACAO-PRODUCAO.md)).
2. **Não exponha a porta do PostgreSQL (`5433`)**: apenas a porta HTTP local (`5200`) é encaminhada pelo túnel.
3. **Mantenha um único túnel:** não aponte para `https://localhost:7200` usando túnel HTTP padrão sem ignorar certificado local; usar `http://localhost:5200` evita erros de TLS entre o `cloudflared` e a API local, enquanto os usuários externos já acessam via HTTPS da Cloudflare.
