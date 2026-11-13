# Configuração para Produção e Outros Computadores

Este documento descreve todas as chaves de configuração do **Portal de Utilidades Públicas (PUP)** ao rodar fora do ambiente de desenvolvimento (`ASPNETCORE_ENVIRONMENT=Production`), seja neste computador ou em outro PC Windows.

## Ordem de leitura das configurações (.NET)

Ao iniciar, a aplicação (`Pup.Api`) lê nesta ordem (a última sobrescreve a anterior):

1. `appsettings.json` (valores padrão, sem segredos)
2. `appsettings.<Ambiente>.json` (`appsettings.Development.json` ou `appsettings.Production.json`)
3. `user-secrets` (apenas em `Development`)
4. Variáveis de ambiente do sistema (use `__` no lugar de `:`, por exemplo `Jwt__Chave`)
5. Argumentos de linha de comando (por exemplo `--urls http://localhost:5200`)

## Chaves principais

| Chave | Obrigatória | Padrão em `appsettings.json` | Observação |
|---|---|---|---|
| `ConnectionStrings:Postgres` | Sim | `Host=localhost;Port=5433;Database=pup;Username=pup;Password=pup` | Ajuste host, porta (`5433` no Docker deste projeto ou `5432` no Postgres nativo) e credenciais. |
| `Jwt:Chave` | Sim | `""` | Precisa ter **pelo menos 32 caracteres**. Se ficar menor ou vazia, a API não inicia. |
| `Jwt:Emissor` | Não | `pup-api` | Identificador de quem emite o token JWT. |
| `Jwt:Audiencia` | Não | `pup-admin` | Destinatário esperado do token JWT. |
| `Jwt:HorasValidade` | Não | `8` | Tempo de validade do token administrativo em horas. |
| `Pup:SemearDadosExemplo` | Não | `true` | Quando `true`, insere os dados iniciais `[Exemplo]` se o banco estiver vazio. Defina `false` quando usar apenas dados reais. |
| `Pup:AdminInicial:Nome` | Não | `Administrador` | Nome do administrador criado na inicialização caso não exista nenhum usuário. |
| `Pup:AdminInicial:Email` | Recomendado | `""` | E-mail de login do administrador inicial (ex.: `admin@pup.local` ou seu e-mail). |
| `Pup:AdminInicial:Senha` | Recomendado | `""` | Senha forte para o administrador inicial. |
| `Front:Pasta` | Sim (site) | `""` | Caminho da pasta estática do portal, relativo ao executável (`./front` no pacote publicado) ou absoluto. Se vazio, sobe apenas a API. |
| `Cors:Origens` | Opcional | `[]` | Lista de origens liberadas caso outro front-end externo consuma a API. Quando o front é servido pela própria API (`Front:Pasta`), tudo roda na mesma origem e não precisa configurar CORS. |
| `ASPNETCORE_URLS` | Opcional | `http://localhost:5000` (fora do VS) | Defina `http://localhost:5200` (ou `http://0.0.0.0:5200` para rede local) ao rodar o executável diretamente. |

## Modelo de `appsettings.Production.json`

Existe um modelo pronto em [appsettings.Production.exemplo.json](../ferramentas/appsettings.Production.exemplo.json). Copie para `appsettings.Production.json` ao lado de `Pup.Api.exe` (ou na pasta `src/Pup.Api` se rodar pelo código-fonte) e preencha os valores reais:

```json
{
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5433;Database=pup;Username=pup;Password=pup"
  },
  "Jwt": {
    "Chave": "TROQUE-POR-UMA-CHAVE-ALEATORIA-COM-NO-MINIMO-32-CARACTERES"
  },
  "Pup": {
    "SemearDadosExemplo": true,
    "AdminInicial": {
      "Nome": "Administrador",
      "Email": "admin@pup.local",
      "Senha": "TroquePorUmaSenhaForte123!"
    }
  },
  "Cors": {
    "Origens": []
  },
  "Front": {
    "Pasta": "./front"
  }
}
```

## Alternativa com variáveis de ambiente (PowerShell)

Se preferir não gravar senha nem chave JWT em arquivo JSON no servidor:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://localhost:5200"
$env:ConnectionStrings__Postgres = "Host=localhost;Port=5433;Database=pup;Username=pup;Password=pup"
$env:Jwt__Chave = "sua-chave-aleatoria-com-mais-de-32-caracteres-aqui"
$env:Pup__AdminInicial__Email = "admin@seu-dominio.local"
$env:Pup__AdminInicial__Senha = "SuaSenhaForteAqui!"
$env:Front__Pasta = "./front"
.\Pup.Api.exe
```

## Comportamento de cache e HTTPS em produção

* **Cache de estáticos:** Em `Development`, a API envia `Cache-Control: no-store, no-cache, must-revalidate` para facilitar testes locais. Em `Production`, esse cabeçalho não é forçado.
* **HTTPS atrás de túnel ou proxy:** Em `Production`, a API chama `UseHttpsRedirection()`. Quando o executável escuta apenas em `http://localhost:5200` (sem porta HTTPS vinculada), o redirecionamento é ignorado com segurança, permitindo que um túnel como o Cloudflare Tunnel faça o HTTPS na borda.
