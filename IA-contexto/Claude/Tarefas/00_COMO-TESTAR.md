# Como testar o PUP (back + front juntos)

Pré-requisitos: .NET SDK 10, Docker Desktop, Git. O front fica dentro deste repositório, em `FrontEnzo/` (cópia do front da dupla).

## 1. Subir tudo

```powershell
cd C:\Users\jojob\source\repos\PortalUtilidadesPublicas
docker compose up -d                      # Postgres na porta 5433
dotnet test tests/Pup.Tests               # regras e mapeamento para o front
dotnet run --project src/Pup.Api          # API + site em http://localhost:5200
```

Primeira subida (ou depois de `docker compose down -v`): cria o banco, o admin de dev e os dados de exemplo.

## 2. Testar a API sozinha

- `powershell -ExecutionPolicy Bypass -File ferramentas\smoke.ps1` → tudo verde.
- Visual Studio: abra `src/Pup.Api/Pup.Api.http` e clique em "Send request" em cada bloco
  (login do admin: `admin@pup.local` / `Admin@12345`, só em desenvolvimento).
- Rotas que o front usa: `http://localhost:5200/api/portal/servicos` e `/api/portal/comunicados`.

## 3. Testar o front servido pela API

1. Abra `http://localhost:5200/`. F12 → Rede: `/api/portal/servicos` e `/api/portal/comunicados` com 200.
2. Navegue pelas 5 páginas e use busca, situação, localidade e categoria.
3. Mude um dado pela API (ex.: `POST /api/intervencoes/{id}/status` no `.http`) e recarregue a página: a mudança aparece.
4. Pare a API e abra o `FrontEnzo\index.html` direto do disco: o site mostra os dados de demonstração antigos (modo reserva).

## 4. Mostrar para fora (apresentação)

```powershell
cloudflared tunnel --url http://localhost:5200
```

Um único link `https://<algo>.trycloudflare.com` mostra o site e a API juntos (mesma origem, sem CORS).
